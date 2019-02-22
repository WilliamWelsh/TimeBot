using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace TimeBot.UserData
{
    public static class DateStorage
    {
        #region User Accounts
        // Save user accounts
        public static void SaveUserAccounts(IEnumerable<UserAccount> accounts, string filePath)
        {
            string json = JsonConvert.SerializeObject(accounts, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        // Load User Accounts
        public static IEnumerable<UserAccount> LoadUserAccounts(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<UserAccount>>(json);
        }
        #endregion

        #region Server Data
        // Save user accounts
        public static void SaveServerData(IEnumerable<ServerData> servers, string filePath)
        {
            string json = JsonConvert.SerializeObject(servers, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        // Load User Accounts
        public static IEnumerable<ServerData> LoadServerData(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<ServerData>>(json);
        }
        #endregion

        // Check if a save exists
        public static bool SaveExists(string filePath) => File.Exists(filePath);
    }
}