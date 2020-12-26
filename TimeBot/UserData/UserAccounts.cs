using System.Linq;
using System.Collections.Generic;

namespace TimeBot.UserData
{
    public static class UserAccounts
    {
        public static List<UserAccount> accounts;

        private static readonly string accountsFile = "Resources/user_data.json";

        static UserAccounts()
        {
            if (DateStorage.SaveExists(accountsFile))
                accounts = DateStorage.LoadUserAccounts(accountsFile).ToList();
            else
            {
                accounts = new List<UserAccount>();
                SaveAccounts();
            }
        }

        public static void SaveAccounts() => DateStorage.SaveUserAccounts(accounts, accountsFile);

        public static UserAccount GetAccount(ulong Id) => GetOrCreateUserAccount(Id);

        private static UserAccount GetOrCreateUserAccount(ulong id)
        {
            var result = from a in accounts
                         where a.userID == id
                         select a;
            var account = result.FirstOrDefault();
            if (account == null) account = CreateUserAccount(id);
            return account;
        }

        private static UserAccount CreateUserAccount(ulong id)
        {
            var newAccount = new UserAccount
            {
                userID = id,
                localTime = 999,
                country = "Not set.",
            };
            accounts.Add(newAccount);
            SaveAccounts();
            return newAccount;
        }
    }
}
