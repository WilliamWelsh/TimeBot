using Discord;
using TimeBot.UserData;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TimeBot
{
    public class StatsHandler
    {
        // Color of the embeds
        private readonly Color embedColor = new Color(127, 166, 208);
        private readonly Color errorColor = new Color(231, 76, 60);
        private readonly Color successColor = new Color(31, 139, 76);

        private List<string> Countries;

        // The main embed that displays time for a user
        // If they don't have a country set, then the footer will be blank
        // This is to avoid a constant "no country set" message for users that don't want to set their country
        private Embed statsEmbed(UserAccount account, SocketGuildUser user)
        {
            var embed = new EmbedBuilder()
                .WithAuthor(new EmbedAuthorBuilder()
                .WithName(user.Nickname ?? user.Username)
                .WithIconUrl(user.GetAvatarUrl()))
                .WithDescription(GetTime(account, user))
                .WithColor(embedColor)
                .WithFooter(GetCountry(account, user))
                .Build();
            return embed;
        }

        // Display a User's local time
        private string GetTime(UserAccount account, SocketGuildUser user)
        {
            if (account.localTime == 999)
                return $"No time set for {user.Nickname ?? user.Username}.\nType `!timesetup` to set up your time and/or country.";
            DateTime localTime = DateTime.Now.AddHours(account.localTime);
            return $"It's {localTime.ToString("h:mm tt")} for {user.Nickname ?? user.Username}.\n{localTime.ToString("dddd, MMMM d.")}";
        }

        // Display a User's country
        private string GetCountry(UserAccount account, SocketGuildUser user) => account.country == "Not set." ? "" : account.country;

        // Display the time (and possibly country) for a user
        public async Task DisplayStats(ISocketMessageChannel channel, SocketGuildUser user) => await channel.SendMessageAsync("", false, statsEmbed(UserAccounts.GetAccount(user), user));

        // Display the !timesetup information
        public async Task DisplayTimeSetup(ISocketMessageChannel channel)
        {
            StringBuilder description = new StringBuilder();
            description.AppendLine("To set up your time, please calculate the difference in hours it is from you and my time:").AppendLine();
            description.AppendLine($"My time: {DateTime.Now.ToString("h:mm tt")}").AppendLine();
            description.AppendLine("Example, if it's 12 pm for me, and 10 am for you, then type `!time set -2` because the difference is 2 hours.");
            await channel.SendMessageAsync("", false, new EmbedBuilder()
                .WithTitle("Stats Setup")
                .AddField("Time Setup", description.ToString())
                .AddField("Country Setup", "You can also set your country.\nType `!country set [country name]` to set your country.\n\nExample: `!country set United States`")
                .WithColor(embedColor)
                .Build());
        }

        // Display an error message
        private async Task PrintError(ISocketMessageChannel channel, string errorMessage)
        {
            await channel.SendMessageAsync("", false, new EmbedBuilder()
                .WithTitle("Error")
                .WithColor(errorColor)
                .WithDescription(errorMessage)
                .Build());
        }

        // Display a success message
        private async Task PrintSuccessMessage(ISocketMessageChannel channel, string message)
        {
            await channel.SendMessageAsync("", false, new EmbedBuilder()
                .WithTitle("Success")
                .WithColor(successColor)
                .WithDescription(message)
                .Build());
        }

        // Set the time for yourself
        public async Task SetTime(ISocketMessageChannel channel, SocketUser user, int hourDifference)
        {
            if (hourDifference < -7 || hourDifference > 16)
            {
                await PrintError(channel, "Invalid hour difference. The input must be between -7 and 16. Please run `!timesetup` for more help.");
                return;
            }

            UserAccount account = UserAccounts.GetAccount(user);
            account.localTime = hourDifference;
            UserAccounts.SaveAccounts();

            await PrintSuccessMessage(channel, $"You have succesfully set your time.\n\n{GetTime(account, (SocketGuildUser)user)}\n\nIf the time is wrong, try again. Type `!timesetup` for more help.");
        }

        // Set the country for yourself
        public async Task SetCountry(ISocketMessageChannel channel, SocketUser user, string country)
        {
            if (string.IsNullOrEmpty(country))
            {
                await PrintError(channel, "The country name cannot be empty.\n\nSuccessful Example: `!country set United States`");
                return;
            }

            if (!Countries.Contains(country, StringComparer.CurrentCultureIgnoreCase))
            {
                await PrintError(channel, "Country not valid. Please try again.\n\nExamples: `!country set united states` or `!country set united kingdom` or `!country set canada`");
                return;
            }

            // Find the country input and set it to the capitlized version
            int index = Countries.FindIndex(x => x.Equals(country, StringComparison.OrdinalIgnoreCase));
            country = Countries.ElementAt(index);

            UserAccount account = UserAccounts.GetAccount(user);
            account.country = country;
            UserAccounts.SaveAccounts();

            await PrintSuccessMessage(channel, $"You have successfully set your country to {country}.\n\nIf this is an error, you can run `!country set [country name]` again.");
        }

        // Set up the countries list to the list of valid countries in countries.txt
        public void SetupCountryList() => Countries = File.ReadAllLines("countries.txt").ToList();

        // Help page
        public async Task DisplayHelp(ISocketMessageChannel channel)
        {
            await channel.SendMessageAsync("", false, new EmbedBuilder()
                .WithTitle("Time Bot Help")
                .WithColor(embedColor)
                .WithDescription($"Hello, I am TimeBot. I can provide the local time and country for other users. Data is saved across all servers.")
                .AddField("Commands", "`!timesetup` Help on setting up your time (and country if you want)\n`!time` View your time.\n`!time @mentionedUser` View a user's local time.\n`!time set [number]` Set your local time.\n`!country set [country name]`Set your country.")
                .AddField("Additional Help", "You can ask on GitHub or the support server (https://discord.gg/qsc8YMS) for additional help.")
                .AddField("GitHub", "TODO")
                .Build());
        }
    }
}
