using System;
using System.Collections.Generic;
using System.IO;
using System.Xml; // You can keep this if needed elsewhere
using Newtonsoft.Json;

namespace DiscordAccountManagerWPF
{
    public static class AccountManager
    {
        private static readonly string FilePath = "accounts.json";
        private static readonly object _lock = new object(); // For thread safety

        public static void SaveAccount(Account account)
        {
            lock (_lock)
            {
                var accounts = LoadAccounts();
                accounts.Add(account);
                File.WriteAllText(FilePath, JsonConvert.SerializeObject(accounts, Newtonsoft.Json.Formatting.Indented));
            }
        }

        public static List<Account> LoadAccounts()
        {
            lock (_lock)
            {
                if (!File.Exists(FilePath))
                    return new List<Account>();

                var json = File.ReadAllText(FilePath);
                return JsonConvert.DeserializeObject<List<Account>>(json) ?? new List<Account>();
            }
        }

        public static void SaveAccounts(List<Account> accounts)
        {
            lock (_lock)
            {
                File.WriteAllText(FilePath, JsonConvert.SerializeObject(accounts, Newtonsoft.Json.Formatting.Indented));
            }
        }
    }
}