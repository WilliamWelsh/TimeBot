using Discord;
using TimeBot.UserData;
using System.Threading.Tasks;

namespace TimeBot
{
    public static class StatsHandler
    {
        // Get their stats as an embed
        public static async Task<Embed> UserStatsEmbed(UserAccount account, string name, string avatarURL, bool appendRefreshTime = false) => new EmbedBuilder()
            .WithAuthor(new EmbedAuthorBuilder()
                .WithName(name)
                .WithIconUrl(avatarURL))
            .WithDescription($"{(appendRefreshTime ? $"{Utilities.GetRefreshedTimeText()}\n\n" : "")}{GetTime(account, name)}")
            .WithColor(await Utilities.GetImageColor(avatarURL))
            .WithFooter($"{GetCountry(account, true)}{account.timeZoneId}")
            .Build();

        // Get server stats as an embed
        public static async Task<Embed> ServerStatsEmbed(GuildAccount account, IGuild guild, bool appendRefreshTime = false) => new EmbedBuilder()
            .WithAuthor(new EmbedAuthorBuilder()
                .WithName(guild.Name)
                .WithIconUrl(guild.IconUrl))
            .WithDescription($"{(appendRefreshTime ? $"{Utilities.GetRefreshedTimeText()}\n\n" : "")}{GetTime(account, guild.Name)}")
            .WithColor(await Utilities.GetImageColor(guild.IconUrl))
            .WithFooter(account.timeZoneId)
            .Build();

        // Display a User's local time
        public static string GetTime(UserAccount account, string name)
        {
            if (account.timeZoneId == "Not set.")
                return $"No time set for {name}.\nType `/timesetup` to set up your time and/or country.";
            var localTime = TimeZones.GetRawTimeByTimeZone(account.timeZoneId);
            return $"It's {localTime:h:mm tt} for {name}.\n{localTime:dddd, MMMM d.}";
        }

        // Display a Servers's local time
        public static string GetTime(GuildAccount account, string name, bool showTimeZone = false)
        {
            if (account.timeZoneId == "Not set.")
                return $"No time set for {name}.\nClick the `Edit Time` button to edit the server time.";
            var localTime = TimeZones.GetRawTimeByTimeZone(account.timeZoneId);
            return $"It's {localTime:h:mm tt} for {name}.\n{localTime:dddd, MMMM d.}{(showTimeZone ? $"\n{account.timeZoneId}" : "")}";
        }

        // Display a User's country
        public static string GetCountry(UserAccount account, bool separate = false)
            => account.country == "Not set." ? "" : $"{account.country}{(separate ? " | " : "")}";
    }
}