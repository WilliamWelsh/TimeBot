using System;
using Discord;
using System.IO;
using System.Text;
using System.Linq;
using TimeBot.UserData;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Commands;

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
        private Embed statsEmbed(UserAccount account, SocketGuildUser user) => new EmbedBuilder()
                .WithAuthor(new EmbedAuthorBuilder()
                .WithName(user.Nickname ?? user.Username)
                .WithIconUrl(user.GetAvatarUrl()))
                .WithDescription(GetTime(account, user))
                .WithColor(embedColor)
                .WithFooter(GetCountry(account, user))
                .Build();

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

        // Display the time for users in a certain role
        public async Task DisplayStats(SocketCommandContext Context, SocketRole Role)
        {
            var Users = Context.Guild.Users;
            StringBuilder text = new StringBuilder();
            foreach (var User in Users)
            {
                if (User.Roles.Contains(Role))
                {
                    var account = UserAccounts.GetAccount(User);
                    if (account.localTime == 999)
                        text.AppendLine($"{User.Nickname ?? User.Username} - No Time Set");
                    else
                        text.AppendLine($"{User.Nickname ?? User.Username} - {DateTime.Now.AddHours(account.localTime).ToString("h:mm tt")}");
                }
            }
            await PrintBasicEmbed(Context.Channel, $"Time for {Role}", text.ToString(), Role.Color);
        }

        // Display the !timesetup information
        public async Task DisplayTimeSetup(ISocketMessageChannel channel)
        {
            StringBuilder description = new StringBuilder()
                .AppendLine("To set up your time, please calculate the difference in hours it is from you and my time:").AppendLine()
                .AppendLine($"My time: {DateTime.Now.ToString("h:mm tt")}").AppendLine()
                .AppendLine("Example, if it's 12 pm for me, and 10 am for you, then type `!time set -2` because the difference is 2 hours.").AppendLine()
                .AppendLine("Example, if it's 12 pm for me, and 1:30 pm for you, then type `!time set 1.5` because the difference is 1 hour and 30 minutes.");
            await channel.SendMessageAsync("", false, new EmbedBuilder()
                .WithTitle("Stats Setup")
                .AddField("Time Setup", description.ToString())
                .AddField("Country Setup", "You can also set your country.\nType `!country set [country name]` to set your country.\n\nExample: `!country set United States`")
                .WithColor(embedColor)
                .Build());
        }

        // Print a basic embed, like an error message or success message
        private async Task PrintBasicEmbed(ISocketMessageChannel channel, string title, string message, Color color) => await channel.SendMessageAsync("", false, new EmbedBuilder()
                .WithTitle(title)
                .WithColor(color)
                .WithDescription(message)
                .Build());

        // Set the time for yourself
        public async Task SetTime(ISocketMessageChannel channel, SocketUser user, double hourDifference)
        {
            if (hourDifference < -7 || hourDifference > 16)
            {
                await PrintBasicEmbed(channel, "Error", "Invalid hour difference. The input must be between -24 and 24. Please run `!timesetup` for more help.", errorColor);
                return;
            }

            if (((int)((decimal)hourDifference % 1 * 100)) != 50 && ((int)((decimal)hourDifference % 1 * 100)) != 0)
            {
                await PrintBasicEmbed(channel, "Error", "Invalid minute difference. The minute offset can only be 0 or 0.5, such as 1.5 or 5.5. Please run `!timesetup` for more help.", errorColor);
                return;
            }

            UserAccount account = UserAccounts.GetAccount(user);
            account.localTime = hourDifference;
            UserAccounts.SaveAccounts();

            await PrintBasicEmbed(channel, "Success", $"You have succesfully set your time.\n\n{GetTime(account, (SocketGuildUser)user)}\n\nIf the time is wrong, try again. Type `!timesetup` for more help.", successColor);
        }

        // Set the country for yourself
        public async Task SetCountry(ISocketMessageChannel channel, SocketUser user, string country)
        {
            if (string.IsNullOrEmpty(country))
            {
                await PrintBasicEmbed(channel, "Error", "The country name cannot be empty.\n\nSuccessful Example: `!country set United States`", errorColor);
                return;
            }

            if (!Countries.Contains(country, StringComparer.CurrentCultureIgnoreCase))
            {
                await PrintBasicEmbed(channel, "Error", "Country not valid. Please try again.\n\nExamples:\n`!country set united states`\n`!country set united kingdom`\n`!country set canada`\n\nList of valid countries: https://raw.githubusercontent.com/WilliamWelsh/TimeBot/master/TimeBot/countries.txt", errorColor);
                return;
            }

            // Find the country input and set it to the capitlized version
            int index = Countries.FindIndex(x => x.Equals(country, StringComparison.OrdinalIgnoreCase));
            country = Countries.ElementAt(index);

            UserAccount account = UserAccounts.GetAccount(user);
            account.country = country;
            UserAccounts.SaveAccounts();

            await PrintBasicEmbed(channel, "Success", $"You have successfully set your country to {country}.\n\nIf this is an error, you can run `!country set [country name]` again.", successColor);
        }

        // Set up the countries list to the list of valid countries in countries.txt
        public void SetupCountryList() => Countries = File.ReadAllLines("countries.txt").ToList();

        // Help page
        public async Task DisplayHelp(ISocketMessageChannel channel) => await channel.SendMessageAsync("", false, new EmbedBuilder()
                .WithTitle("Time Bot Help")
                .WithColor(embedColor)
                .WithDescription($"Hello, I am TimeBot. I can provide the local time and country for other users. Data is saved across all servers.")
                .AddField("Commands", "`!timesetup` Help on setting up your time (and country if you want)\n`!time` View your time.\n`!time @mentionedUser` View a user's local time.\n`!time set [number]` Set your local time.\n`!country set [country name]`Set your country.\n`!timeinvite` Get an invite link for the bot.")
                .AddField("Additional Help", "You can ask on GitHub or the support server (https://discord.gg/qsc8YMS) for additional help.")
                .AddField("GitHub", "https://github.com/WilliamWelsh/TimeBot")
                .Build());

        // Get Invite Link
        public async Task DMInviteLink(ISocketMessageChannel channel, SocketUser user)
        {
            await user.SendMessageAsync("https://discordapp.com/api/oauth2/authorize?client_id=529569000028373002&permissions=68608&scope=bot");
            await PrintBasicEmbed(channel, "Success", "The invite linked has been DMed to you!", successColor);
        }

        // Display Time for everyone (requested feature)
        public async Task DisplayAllTime(SocketCommandContext Context)
        {
            var Users = Context.Guild.Users;
            StringBuilder text = new StringBuilder();
            foreach (var User in Users)
            {
                if (!User.IsBot)
                {
                    var account = UserAccounts.GetAccount(User);
                    if (account.localTime == 999)
                        text.AppendLine($"{User.Nickname ?? User.Username} - No Time Set");
                    else
                        text.AppendLine($"{User.Nickname ?? User.Username} - {DateTime.Now.AddHours(account.localTime).ToString("h:mm tt")}");
                }
            }
            
            if (text.ToString().Length > 2048)
            {
                int size = 2048;
                string str = text.ToString();
                List<string> strings = new List<string>();

                for (int i = 0; i < str.Length; i += size)
                {
                    if (i + size > str.Length)
                        size = str.Length - i;
                    strings.Add(str.Substring(i, size));
                }

                foreach(string s in strings)
                    await PrintBasicEmbed(Context.Channel, "Time for All", s, embedColor);
            }
            else
                await PrintBasicEmbed(Context.Channel, "Time for All", text.ToString(), embedColor);
        }
    }
}
