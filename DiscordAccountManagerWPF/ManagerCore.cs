// ManagerCore.cs
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace DiscordAccountManagerWPF
{
    public class Core
    {
        static void WaitForPageToLoad(IWebDriver driver)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            wait.Until(drv => ((IJavaScriptExecutor)drv).ExecuteScript("return document.readyState").Equals("complete"));
        }

        public async void ManualLogin()
        {
            await Task.Run(async () =>
            {
                string emailValue = "";
                string passwordValue = "";

                new DriverManager().SetUpDriver(new ChromeConfig());

                var options = new ChromeOptions();
                options.AddArgument("--disable-blink-features=AutomationControlled");
                options.AddArgument("--disable-extensions");
                options.AddArgument("--no-sandbox");
                options.AddArgument("--disable-dev-shm-usage");
                options.AddArgument("--start-maximized");
                options.AddExcludedArgument("enable-automation");
                options.AddUserProfilePreference("useAutomationExtension", false);
                options.AddArgument("--disable-web-security");
                options.AddArgument("--disable-features=IsolateOrigins,site-per-process");
                var service = ChromeDriverService.CreateDefaultService();
                service.HideCommandPromptWindow = true;
                service.SuppressInitialDiagnosticInformation = true;
                ChromeDriver driver = null;
                try
                {
                    driver = new ChromeDriver(service,options);
                    IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

                    js.ExecuteScript("Object.defineProperty(navigator, 'webdriver', {get: () => undefined})");
                    js.ExecuteScript("window.chrome = {runtime: {}};");
                    js.ExecuteScript("navigator.permissions.query = (parameters) => (parameters.name === 'notifications' ? Promise.resolve({state: Notification.permission}) : Promise.resolve({}));");

                    driver.Navigate().GoToUrl("https://discord.com/login");
                    WaitForPageToLoad(driver);

                    // Check if JS is blocked
                    string pageText = driver.FindElement(By.TagName("body")).Text;
                    if (pageText.Contains("You need to enable JavaScript"))
                    {
                        Functions.ShowErrorOnUIThread("Discord blocked automation. Please use 'Token Login' instead.");
                        return;
                    }

                    // Wait for fields
                    var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
                    wait.Until(drv => drv.FindElements(By.Name("email")).Count > 0);

                    // Inject input listeners
                    js.ExecuteScript(@"
                window.__DISCORD_EMAIL__ = '';
                window.__DISCORD_PASSWORD__ = '';
                const e = document.querySelector('input[name=""email""]');
                const p = document.querySelector('input[name=""password""]');
                if(e) {
                    e.addEventListener('input', ev => window.__DISCORD_EMAIL__ = ev.target.value);
                    window.__DISCORD_EMAIL__ = e.value;
                }
                if(p) {
                    p.addEventListener('input', ev => window.__DISCORD_PASSWORD__ = ev.target.value);
                    window.__DISCORD_PASSWORD__ = p.value;
                }
            ");

                    Console.WriteLine("✅ Ready. Enter credentials manually in the browser...");

                    bool loginDetected = false;
                    var start = DateTime.Now;
                    while (!loginDetected /*&& (DateTime.Now - start).TotalSeconds < 3000*/)
                    {
                        try
                        {
                            emailValue = (js.ExecuteScript("return window.__DISCORD_EMAIL__") ?? "").ToString();
                            passwordValue = (js.ExecuteScript("return window.__DISCORD_PASSWORD__") ?? "").ToString();

                            if (driver.Url.StartsWith("https://discord.com/channels/@me", StringComparison.OrdinalIgnoreCase))
                            {
                                loginDetected = true;
                                break;
                            }
                        }
                        catch { }
                        await Task.Delay(300);
                    }

                    if (!loginDetected)
                    {
                        Functions.ShowErrorOnUIThread("Login not detected. Use Token Login instead.");
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(emailValue) || string.IsNullOrWhiteSpace(passwordValue))
                    {
                        Functions.ShowErrorOnUIThread("No credentials captured. Save failed.");
                        return;
                    }

                    // Save account
                    string result = await Functions.GetToken(emailValue, passwordValue);
                    var json = JObject.Parse(result);
                    string token = json["token"]?.ToString();
                    string userId = json["user_id"]?.ToString();

                    if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userId))
                        throw new Exception("Invalid token response");

                    AccountManager.SaveAccount(new Account
                    {
                        Email = emailValue,
                        Password = passwordValue,
                        Token = token,
                        UserId = userId
                    });

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MainWindow.Instance?.RefreshAccountList();
                        //MessageBox.Show($"✅ Saved: {emailValue}", "Success", MessageBoxButton.OK);
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    Functions.ShowErrorOnUIThread("Error: " + ex.Message);
                }
                finally
                {
                    driver?.Quit();
                }
            });
        }
        public async void ManualRegister()
{
    await Task.Run(async () =>
    {
        string emailValue = "";
        string passwordValue = "";
        string usernameValue = "";

        new DriverManager().SetUpDriver(new ChromeConfig());

        var options = new ChromeOptions();
        options.AddArgument("--disable-blink-features=AutomationControlled");
        options.AddArgument("--disable-extensions");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--start-maximized");
        options.AddExcludedArgument("enable-automation");
        options.AddUserProfilePreference("useAutomationExtension", false);
        options.AddArgument("--disable-web-security");
        options.AddArgument("--disable-features=IsolateOrigins,site-per-process");
        options.AddArgument("--disable-background-timer-throttling");
        options.AddArgument("--disable-renderer-backgrounding");
        var service = ChromeDriverService.CreateDefaultService();
        service.HideCommandPromptWindow = true;
        service.SuppressInitialDiagnosticInformation = true;
        ChromeDriver driver = null;
        try
        {
            driver = new ChromeDriver(service,options);
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            // Stealth patches
            js.ExecuteScript("Object.defineProperty(navigator, 'webdriver', {get: () => undefined})");
            js.ExecuteScript("window.chrome = {runtime: {}};");
            js.ExecuteScript("navigator.permissions.query = (parameters) => (parameters.name === 'notifications' ? Promise.resolve({state: Notification.permission}) : Promise.resolve({}));");

            driver.Navigate().GoToUrl("https://discord.com/register");
            WaitForPageToLoad(driver);
            // Check if JS is blocked
            try
            {
                string bodyText = driver.FindElement(By.TagName("body")).Text;
                if (bodyText.Contains("You need to enable JavaScript"))
                {
                    Functions.ShowErrorOnUIThread("Discord blocked automation on register page.");
                    return;
                }
            }
            catch { /* ignore */ }

            // Wait for email field (first field in signup)
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            wait.Until(drv => drv.FindElements(By.CssSelector("input[name='email']")).Count > 0);

            // Inject real-time input listeners for all 3 fields
            js.ExecuteScript(@"
                window.__DISCORD_REG_EMAIL__ = '';
                window.__DISCORD_REG_PASSWORD__ = '';
                window.__DISCORD_REG_USERNAME__ = '';

                const email = document.querySelector('input[name=""email""]');
                const password = document.querySelector('input[name=""password""]');
                const username = document.querySelector('input[placeholder*=""username""]') || 
                                 document.querySelector('input[aria-label*=""username""]') ||
                                 document.querySelector('input[name=""username""]');

                if (email) {
                    email.addEventListener('input', e => window.__DISCORD_REG_EMAIL__ = e.target.value);
                    window.__DISCORD_REG_EMAIL__ = email.value;
                }
                if (password) {
                    password.addEventListener('input', e => window.__DISCORD_REG_PASSWORD__ = e.target.value);
                    window.__DISCORD_REG_PASSWORD__ = password.value;
                }
                if (username) {
                    username.addEventListener('input', e => window.__DISCORD_REG_USERNAME__ = e.target.value);
                    window.__DISCORD_REG_USERNAME__ = username.value;
                }
            ");

            Console.WriteLine("✅ Signup form detected. Fill it out manually...");

            bool registered = false;
            var startTime = DateTime.Now;
            const int timeoutSeconds = 99999; // 10 minutes for manual signup + captcha

            while (!registered /*&& (DateTime.Now - startTime).TotalSeconds < timeoutSeconds*/)
            {
                try
                {
                    emailValue = (js.ExecuteScript("return window.__DISCORD_REG_EMAIL__") ?? "").ToString();
                    passwordValue = (js.ExecuteScript("return window.__DISCORD_REG_PASSWORD__") ?? "").ToString();
                    usernameValue = (js.ExecuteScript("return window.__DISCORD_REG_USERNAME__") ?? "").ToString();

                    string currentUrl = driver.Url;
                    if (currentUrl.StartsWith("https://discord.com/channels/@me", StringComparison.OrdinalIgnoreCase))
                    {
                        registered = true;
                        break;
                    }
                }
                catch (WebDriverException) { /* navigation in progress */ }

                await Task.Delay(400);
            }

            if (!registered)
            {
                Functions.ShowErrorOnUIThread("Signup not completed within time limit.");
                return;
            }

            if (string.IsNullOrWhiteSpace(emailValue) || string.IsNullOrWhiteSpace(passwordValue))
            {
                Functions.ShowErrorOnUIThread("Email or password was not captured.");
                return;
            }

            Console.WriteLine($"📧 Email: {emailValue}");
            //Console.WriteLine($"🔑 Password length: {passwordValue.Length}");
            Console.WriteLine($"🧑 Username: {usernameValue}");

            // Get token using credentials
            string result = await Functions.GetToken(emailValue, passwordValue);
            var json = JObject.Parse(result);

            string token = json["token"]?.ToString();
            string userId = json["user_id"]?.ToString();

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userId))
                throw new Exception("Failed to retrieve token after signup.");

            AccountManager.SaveAccount(new Account
            {
                Email = emailValue,
                Password = passwordValue,
                Username = usernameValue,
                Token = token,
                UserId = userId
            });

            Application.Current.Dispatcher.Invoke(() =>
            {
                MainWindow.Instance?.RefreshAccountList();
                //MessageBox.Show($"✅ New account saved!\nEmail: {emailValue}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            });

            Console.WriteLine("✅ Account saved successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("	Register error: " + ex.Message);
            Functions.ShowErrorOnUIThread("Register error: " + ex.Message);
        }
        finally
        {
            driver?.Quit();
        }
    });
}
        public async void TokenLogin(string Token)
        {
            Task.Run(() =>
            {

                new DriverManager().SetUpDriver(new ChromeConfig());

                var options = new ChromeOptions();
                options.AddArgument("--disable-blink-features=AutomationControlled");
                options.AddArgument("--disable-extensions");
                options.AddArgument("--no-sandbox");
                options.AddArgument("--disable-dev-shm-usage");
                options.AddArgument("--start-maximized");
                options.AddExcludedArgument("enable-automation");
                options.AddUserProfilePreference("useAutomationExtension", false);

                options.AddUserProfilePreference("profile.default_content_setting_values.notifications", 2);
                options.AddArgument("--disable-infobars");
                var service = ChromeDriverService.CreateDefaultService();
                service.HideCommandPromptWindow = true;
                service.SuppressInitialDiagnosticInformation = true;
                var chromeDriver = new ChromeDriver(service,options);
                var js = (IJavaScriptExecutor)chromeDriver;
                js.ExecuteScript("Object.defineProperty(navigator, 'webdriver', {get: () => undefined})");

                try
                {
                    chromeDriver.Navigate().GoToUrl("https://discord.com/login");

                    WaitForPageToLoad(chromeDriver);

                    // Inject the token into local storage using JavaScript
                    var script = $@"
            function login(token) {{
                setInterval(() => {{
                    document.body.appendChild(document.createElement('iframe')).contentWindow.localStorage.token = `""${{token}}""`;
                }}, 50);
                setTimeout(() => {{
                    location.reload();
                }}, 2500);
            }}
            login('{Token}');
            ";

                    chromeDriver.ExecuteScript(script);

                    // Wait for a few seconds to ensure the script has time to execute
                    Thread.Sleep(5000);

                    Console.WriteLine("Logged in using token.");

                    // Monitor the ChromeDriver process
                    while (!chromeDriver.CurrentWindowHandle.Equals(string.Empty))
                    {
                        Thread.Sleep(1000); // Check every second
                    }

                    Console.WriteLine("Chrome browser closed.");
                }
                catch (NoSuchWindowException)
                {
                    // Handle the case where the browser window is closed
                    Console.WriteLine("Chrome browser closed.");
                }
                finally
                {
                    // Dispose of the ChromeDriver instance when done
                    chromeDriver.Quit();
                }
            });
        }/// fUNCTION END
    }
}