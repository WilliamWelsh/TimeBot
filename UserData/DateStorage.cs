using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace TimeBot.UserData
{
    public static class DateStorage
    {
        // Save user accounts
        public static void SaveUserAccounts(IEnumerable<UserAccount> accounts, string filePath)
        {
            var json = JsonConvert.SerializeObject(accounts, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        // Load User Accounts
        public static IEnumerable<UserAccount> LoadUserAccounts(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<UserAccount>>(json);
        }

        // Check if a save exists
        public static bool SaveExists(string filePath) => File.Exists(filePath);
    }
}
