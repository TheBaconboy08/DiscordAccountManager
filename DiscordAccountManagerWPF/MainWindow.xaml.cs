using DiscordAccountManagerWPF.Properties;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;


namespace DiscordAccountManagerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();
        public static MainWindow Instance { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            Instance = this; // Store the instance
            //AllocConsole();
            LoadAccounts();
            TopMostCheckBox.IsChecked = Properties.Settings.Default.UITopMost;
            this.Topmost = TopMostCheckBox.IsChecked.Value;
        }
        private void DropdownButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as FrameworkElement;

            var contextMenu = new ContextMenu();
            contextMenu.Items.Add(new MenuItem { Header = "Create Account", Tag = "Create" });

            //contextMenu.Items.Add(new MenuItem { Header = "Add via Email/Password", Tag = "email" });
            //contextMenu.Items.Add(new MenuItem { Header = "Add via Token", Tag = "token" });
            //contextMenu.Items.Add(new MenuItem { Header = "Import from File", Tag = "file" });

            foreach (MenuItem item in contextMenu.Items)
            {
                item.Click += (s, args) =>
                {
                    switch ((s as MenuItem)?.Tag?.ToString())
                    {
                        case "Create":
                            CreateAccount_Click(sender, e); // or open form
                            break;
                    }
                };
            }

            contextMenu.PlacementTarget = button;
            contextMenu.IsOpen = true;
            e.Handled = true; // Prevents main button click
        }
        // ... rest of your code

        protected override void OnClosed(EventArgs e)
        {
            Instance = null; // Clean up
            base.OnClosed(e);
        }
        // In MainWindow.xaml.cs
        public void RefreshAccountList()
        {
            var accounts = AccountManager.LoadAccounts();
            AccountList.ItemsSource = accounts;
        }
        // Keep track of selected account
        private Account _selectedAccount;

        // Handle account selection
        //private void AccountItem_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (sender is System.Windows.Controls.Border border && border.DataContext is Account account)
        //    {
        //        // Deselect previous
        //        if (_selectedAccount != null)
        //            _selectedAccount.IsSelected = false;

        //        // Select new
        //        _selectedAccount = account;
        //        _selectedAccount.IsSelected = true;
        //    }
        //}

        // On startup


// In handler, also save:
private void TopMostCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (TopMostCheckBox.IsChecked.HasValue)
            {
                bool isTop = TopMostCheckBox.IsChecked.Value;
                this.Topmost = isTop;
                Properties.Settings.Default.UITopMost = isTop;
                Properties.Settings.Default.Save();
            }
        }
        // 🔴 Remove Selected Account (via bottom button)
        private void RemoveSelectedAccount_Click(object sender, RoutedEventArgs e)
        {
            var account = AccountList.SelectedItem as Account;

            if (account?.Email == null)
            {
                MessageBox.Show("Could not identify account to remove.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show(
                $"Remove account {account.Email}?",
                "Confirm Remove",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var accounts = AccountManager.LoadAccounts();

                // ✅ Remove by EMAIL
                accounts.RemoveAll(acc => acc.Email == account.Email);

                // ✅ SAVE TO FILE
                AccountManager.SaveAccounts(accounts);

                // ✅ Refresh UI
                LoadAccounts();

                // Clear selection if it matches
                if (_selectedAccount?.Email == account.Email)
                    _selectedAccount = null;
            }
        }

        // 🌐 Open Selected Account in Browser Using Token
        private void OpenSelectedAccountInBrowser_Click(object sender, RoutedEventArgs e)
        {
            var account = AccountList.SelectedItem as Account;

            if (account?.Token == null)
            {
                MessageBox.Show("Selected account has no token or isn't selected.", "Invalid Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var core = new Core();
                core.TokenLogin(account.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open browser:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ToggleOptions_Click(object sender, RoutedEventArgs e)
        {
            if (MainGrid.Visibility == Visibility.Visible)
            {
                MainGrid.Visibility = Visibility.Collapsed;
                OptionsGrid.Visibility = Visibility.Visible;
            }
            else
            {
                MainGrid.Visibility = Visibility.Visible;
                OptionsGrid.Visibility = Visibility.Collapsed;
            }
        }
        //Theme Stuff


        private void CopyEmail_Click(object sender, RoutedEventArgs e)
        {
            var account = (sender as Button)?.Tag as Account;
            if (account != null)
            {
                Clipboard.SetText(account.Email);
                MessageBox.Show("Email copied to clipboard!", "Copied", MessageBoxButton.OK);
            }
        }

        private void CopyPassword_Click(object sender, RoutedEventArgs e)
        {
            var account = (sender as Button)?.Tag as Account;
            if (account != null)
            {
                Clipboard.SetText(account.Password);
                MessageBox.Show("Password copied to clipboard!", "Copied", MessageBoxButton.OK);
            }
        }

        // Right-click context menu handlers
        private void CopySelectedEmail_Click(object sender, RoutedEventArgs e)
        {
            var account = AccountList.SelectedItem as Account;
            if (account != null)
            {
                Clipboard.SetText(account.Email);
                MessageBox.Show("Email copied to clipboard!", "Copied", MessageBoxButton.OK);
            }
        }

        private void CopySelectedPassword_Click(object sender, RoutedEventArgs e)
        {
            var account = AccountList.SelectedItem as Account;
            if (account != null)
            {
                Clipboard.SetText(account.Password);
                MessageBox.Show("Password copied to clipboard!", "Copied", MessageBoxButton.OK);
            }
        }
        private void CopySelectedUserId_Click(object sender, RoutedEventArgs e)
        {
            var account = AccountList.SelectedItem as Account;
            if (account != null)
            {
                Clipboard.SetText(account.UserId);
                MessageBox.Show("UserId copied to clipboard!", "Copied", MessageBoxButton.OK);
            }
        }
        private void CopyTokenFromContextMenu_Click(object sender, RoutedEventArgs e)
        {
            var account = AccountList.SelectedItem as Account;
            if (account?.Token != null)
            {
                Clipboard.SetText(account.Token);
                MessageBox.Show("Token copied to clipboard!", "Copied", MessageBoxButton.OK);
            }
            else
            {
                MessageBox.Show("No token available for this account!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void RemoveAccountFromContextMenu_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as MenuItem;
            var account = menuItem?.DataContext as Account;

            if (account?.Email == null)
            {
                MessageBox.Show("Could not identify account to remove.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show(
                $"Remove account {account.Email}?",
                "Confirm Remove",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var accounts = AccountManager.LoadAccounts();

                // ✅ Remove by EMAIL
                accounts.RemoveAll(acc => acc.Email == account.Email);

                // ✅ SAVE TO FILE
                AccountManager.SaveAccounts(accounts);

                // ✅ Refresh UI
                LoadAccounts();

                // Clear selection if it matches
                if (_selectedAccount?.Email == account.Email)
                    _selectedAccount = null;
            }
        }
        // Helper method to show errors on UI thread



        // Implement this to save tokens securely (use DPAPI encryption!)
        private void SaveTokenSecurely(string email, string token)
        {
            // TODO: Save encrypted token to your accounts.json
            // Example:
            // var account = new Account { Email = email, Token = SecureStorage.Encrypt(token) };
            // AccountManager.SaveAccount(account);
        }
        private void LoadAccounts()
        {
            var accounts = AccountManager.LoadAccounts();
            AccountList.ItemsSource = accounts; // AccountList is your ItemsControl
        }

        private void AddAccount_Click(object sender, RoutedEventArgs e)
        {
            var core = new Core();
            core.ManualLogin();
        }
        private void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            var core = new Core();
            core.ManualRegister();
        }
        // Add these to your MainWindow class
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OpenBrowser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Open Discord login page in default browser
                Process.Start("https://discord.com/login");
            }
            catch
            {
                // Fallback for older .NET Framework versions (like 4.7.2)
                Process.Start("explorer.exe", "https://discord.com/login");
            }
        }


    }
}
