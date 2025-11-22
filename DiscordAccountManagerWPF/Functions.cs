using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace DiscordAccountManagerWPF
{





        public class Functions
        {


        private static DataTable userTable = new DataTable("Data");

            static public string CurrentTokenData = "";
            static public string CurrectAcTokenData = "";
            static public string AddedDisplayName = "";

        public static async void WaitForPageToLoad(IWebDriver driver)
            {
                var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(30));
                wait.Until(drv => ((IJavaScriptExecutor)drv).ExecuteScript("return document.readyState").Equals("complete"));
            }
        public static async void ShowErrorOnUIThread(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }
        public static async Task<string> GetToken(string username, string password)
            {
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri("https://discord.com/api/v9/auth/login"),
                    Content = new StringContent($"{{\"login\":\"{username}\",\"password\":\"{password}\"}}", Encoding.UTF8, "application/json")
                };

                using (var client = new HttpClient())
                {
                    var response = await client.SendAsync(request);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    CurrentTokenData = responseContent;
                    return responseContent;

                }
            }


    }
}

