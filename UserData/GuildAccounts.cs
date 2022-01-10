using System.Linq;
using System.Collections.Generic;

namespace TimeBot.UserData
{
    public class GuildAccounts
    {
        public static List<GuildAccount> accounts;

        private static readonly string accountsFile = "Resources/guild_data.json";

        static GuildAccounts()
        {
            if (DateStorage.SaveExists(accountsFile))
                accounts = DateStorage.LoadGuildAccounts(accountsFile).ToList();
            else
            {
                accounts = new List<GuildAccount>();
                SaveAccounts();
            }
        }

        public static void SaveAccounts() => DateStorage.SaveGuildAccounts(accounts, accountsFile);

        public static GuildAccount GetAccount(ulong Id) => GetOrCreateGuildAccount(Id);

        private static GuildAccount GetOrCreateGuildAccount(ulong id)
        {
            var result = from a in accounts
                         where a.guildId == id
                         select a;
            var account = result.FirstOrDefault();
            if (account == null) account = CreateGuildAccount(id);
            return account;
        }

        private static GuildAccount CreateGuildAccount(ulong id)
        {
            var newAccount = new GuildAccount
            {
                guildId = id,
                timeZoneId = "Not set."
            };
            accounts.Add(newAccount);
            SaveAccounts();
            return newAccount;
        }
    }
}