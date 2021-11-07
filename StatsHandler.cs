﻿using Discord;
using TimeBot.UserData;
using System.Threading.Tasks;

namespace TimeBot
{
    public static class StatsHandler
    {
        public static async Task<Embed> StatsEmbed(UserAccount account, string name, string avatarURL) => new EmbedBuilder()
                .WithAuthor(new EmbedAuthorBuilder()
                .WithName(name)
                .WithIconUrl(avatarURL))
                .WithDescription($"{GetTime(account, name)}")
                .WithColor(await Utilities.GetUserColor(avatarURL))
                .WithFooter($"{GetCountry(account)} | {account.timeZoneId}")
                .Build();

        // Display a User's local time
        public static string GetTime(UserAccount account, string name)
        {
            if (account.timeZoneId == "Not set.")
                return $"No time set for {name}.\nType `/timesetup` to set up your time and/or country.";
            var localTime = TimeZones.GetRawTimeByTimeZone(account.timeZoneId);
            return $"It's {localTime:h:mm tt} for {name}.\n{localTime:dddd, MMMM d.}";
        }

        // Display a User's country
        public static string GetCountry(UserAccount account)
            => account.country == "Not set." ? "" : account.country;
    }
}