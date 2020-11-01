using System;
using Discord;
using System.IO;
using System.Text;
using System.Linq;
using TimeBot.UserData;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TimeBot
{
    public static class StatsHandler
    {
        private static List<string> Countries;

        // The main embed that displays time for a target
        // If they don't have a country set, then the footer will be blank
        // This is to avoid a constant "no country set" message for users that don't want to set their country
        private static Embed StatsEmbed(UserAccount account, SocketGuildUser user) => new EmbedBuilder()
                .WithAuthor(new EmbedAuthorBuilder()
                .WithName(user.Nickname ?? user.Username)
                .WithIconUrl(user.GetAvatarUrl()))
                //.WithDescription(GetTime(account, user))
                .WithDescription($"{GetTime(account, user)}\n\nTime may not be working for some users, this is a known issue due to Discord's new security measures for bots that are in a lot of servers. This will be fixed asap.")
                .WithColor(Utilities.GetUserColor(user.GetAvatarUrl()))
                .WithFooter(GetCountry(account))
                .Build();

        // Display a User's local time
        public static string GetTime(UserAccount account, SocketGuildUser user)
        {
            if (account.localTime == 999)
                return $"No time set for {user.Nickname ?? user.Username}.\nType `!timesetup` to set up your time and/or country.";
            var localTime = DateTime.Now.AddHours(account.localTime);
            return $"It's {localTime:h:mm tt} for {user.Nickname ?? user.Username}.\n{localTime:dddd, MMMM d.}";
        }

        // Display a User's country
        public static string GetCountry(UserAccount account) => account.country == "Not set." ? "" : account.country;

        // Display the time (and possibly country) for a target
        public static async Task DisplayStats(ISocketMessageChannel channel, SocketGuildUser user) => await channel.SendMessageAsync("", false, StatsEmbed(UserAccounts.GetAccount(user), user));

        // Display the time for users in a certain role
        public static async Task DisplayStats(SocketCommandContext Context, SocketRole Role)
        {
            var Users = Context.Guild.Users;
            var text = new StringBuilder();
            foreach (var User in Users)
            {
                if (User.Roles.Contains(Role))
                {
                    var account = UserAccounts.GetAccount(User);
                    if (account.localTime == 999)
                        text.AppendLine($"{User.Nickname ?? User.Username} - No Time Set");
                    else
                        text.AppendLine($"{User.Nickname ?? User.Username} - {Utilities.GetTime(account.localTime)}");
                }
            }
            //await Context.Channel.PrintEmbed($"Time for {Role}", text.ToString(), Role.Color);
            await Context.Channel.PrintEmbed($"Time for {Role}", $"{text}\n\nTime may not be working for some users, this is a known issue due to Discord's new security measures for bots that are in a lot of servers. This will be fixed asap.", Role.Color);
        }

        // Display the !timesetup information
        public static async Task DisplayTimeSetup(ISocketMessageChannel channel)
        {
            var description = new StringBuilder()
                .AppendLine("To set up your time, please calculate the difference in hours it is from you and my time:").AppendLine()
                .AppendLine($"My time: {DateTime.Now.ToString("h:mm tt")}").AppendLine()
                .AppendLine("Example, if it's 12 pm for me, and 10 am for you, then type `!time set -2` because the difference is 2 hours.").AppendLine()
                .AppendLine("Example, if it's 12 pm for me, and 1:30 pm for you, then type `!time set 1.5` because the difference is 1 hour and 30 minutes.");
            await channel.SendMessageAsync("", false, new EmbedBuilder()
                .WithTitle("Stats Setup")
                .AddField("Time Setup", description.ToString())
                .AddField("Country Setup", "You can also set your country.\nType `!country set [country name]` to set your country.\n\nExample: `!country set United States`")
                .WithColor(Utilities.Blue)
                .Build());
        }

        // Set the time for yourself
        public static async Task SetTime(SocketCommandContext context, double hourDifference)
        {
            if (hourDifference < -24 || hourDifference > 24)
            {
                await context.Channel.PrintError("Invalid hour difference. The input must be between -24 and 24. Please run `!timesetup` for more help.");
                return;
            }

            var minuteDifference = (int)((decimal)hourDifference % 1 * 100);
            if (minuteDifference != 50 && minuteDifference != 0 && minuteDifference != -50)
            {
                await context.Channel.PrintError("Invalid minute difference. The minute offset can only be 0 or 0.5, such as 1.5 or 5.5. Please run `!timesetup` for more help.");
                return;
            }

            var account = UserAccounts.GetAccount(context.User);
            account.localTime = hourDifference;
            UserAccounts.SaveAccounts();

            await context.Channel.PrintSuccess($"You have succesfully set your time.\n\n{GetTime(account, (SocketGuildUser)context.User)}\n\nIf the time is wrong, try again. Type `!timesetup` for more help.");
        }

        // Set the time for yourself
        public static async Task SetSomeonesTime(SocketCommandContext context, SocketUser target, double hourDifference)
        {
            // If the target is talking about their self.. just set your own time..?
            if (context.User.Id == target.Id)
            {
                await SetTime(context, hourDifference);
                return;
            }

            // Make sure the commanding target is an admin
            if (!((SocketGuildUser)context.User).GuildPermissions.Administrator)
            {
                await context.Channel.PrintError("Sorry, only administrators can set other people's time.");
                return;
            }

            if (hourDifference < -24 || hourDifference > 24)
            {
                await context.Channel.PrintError("Invalid hour difference. The input must be between -24 and 24. Please run `!timesetup` for more help.");
                return;
            }

            var minuteDifference = (int)((decimal)hourDifference % 1 * 100);
            if (minuteDifference != 50 && minuteDifference != 0 && minuteDifference != -50)
            {
                await context.Channel.PrintError("Invalid minute difference. The minute offset can only be 0 or 0.5, such as 1.5 or 5.5. Please run `!timesetup` for more help.");
                return;
            }

            var account = UserAccounts.GetAccount(target);
            account.localTime = hourDifference;
            UserAccounts.SaveAccounts();

            await context.Channel.PrintSuccess($"{context.User.Mention} has succesfully set {target.Mention}'s time.\n\n{GetTime(account, (SocketGuildUser)target)}\n\nIf the time is wrong, try again. Type `!timesetup` for more help.");
        }

        // Set the country for yourself
        public static async Task SetCountry(ISocketMessageChannel channel, SocketUser user, string country)
        {
            if (string.IsNullOrEmpty(country))
            {
                await channel.PrintError("The country name cannot be empty.\n\nSuccessful Example: `!country set United States`");
                return;
            }

            if (!Countries.Contains(country, StringComparer.CurrentCultureIgnoreCase))
            {
                await channel.PrintError("Country not valid. Please try again.\n\nExamples:\n`!country set united states`\n`!country set united kingdom`\n`!country set canada`\n\nList of valid countries: https://raw.githubusercontent.com/WilliamWelsh/TimeBot/master/TimeBot/countries.txt");
                return;
            }

            // Find the country input and set it to the capitlized version
            var index = Countries.FindIndex(x => x.Equals(country, StringComparison.OrdinalIgnoreCase));

            // Save the target's country
            var account = UserAccounts.GetAccount(user);
            account.country = Countries.ElementAt(index);
            UserAccounts.SaveAccounts();

            await channel.PrintSuccess($"You have successfully set your country to {account.country}.\n\nIf this is an error, you can run `!country set [country name]` again.");
        }

        // Set up the countries list to the list of valid countries in countries.txt
        public static void SetupCountryList() => Countries = File.ReadAllLines("countries.txt").ToList();

        // Help page
        public static async Task DisplayHelp(ISocketMessageChannel channel) => await channel.SendMessageAsync("", false, new EmbedBuilder()
                .WithTitle("Time Bot Help")
                .WithColor(Utilities.Blue)
                .WithDescription($"Hello, I am TimeBot. I can provide the local time and country for other users. Data is saved across all servers.")
                .AddField("Commands", "`!timesetup` Help on setting up your time (and country if you want)\n`!time` View your time.\n`!time @mentionedUser` View a target's local time.\n`!time set [number]` Set your local time.\n`!country set [country name]`Set your country.\n`!timeinvite` Get an invite link for the bot.")
                .AddField("Additional Help", "You can ask on GitHub or the support server (https://discord.gg/ga9V5pa) for additional help.\n\nOr add the Developer: Reverse#1193")
                .AddField("GitHub", "https://github.com/WilliamWelsh/TimeBot")
                .Build());

        // Get Invite Link
        public static async Task DMInviteLink(ISocketMessageChannel channel, SocketUser user)
        {
            await user.SendMessageAsync("https://discordapp.com/api/oauth2/authorize?client_id=529569000028373002&permissions=68608&scope=bot");
            await channel.PrintSuccess("The invite linked has been DMed to you!");
        }

        // Display Time for everyone (requested feature)
        public static async Task DisplayEveryonesTime(SocketCommandContext Context)
        {
            var Users = Context.Guild.Users;
            var text = new StringBuilder();
            foreach (var User in Users)
            {
                if (!User.IsBot)
                {
                    var account = UserAccounts.GetAccount(User);
                    if (account.localTime == 999)
                        text.AppendLine($"{User.Nickname ?? User.Username} - No Time Set");
                    else
                        text.AppendLine($"{User.Nickname ?? User.Username} - {Utilities.GetTime(account.localTime)}");
                }
            }

            await SendPossiblyLongEmbed(Context.Channel, "Time for All", text.ToString()).ConfigureAwait(false);
        }

        // Display Country for everyone (requested feature)
        public static async Task DisplayEveryonesCountry(SocketCommandContext Context)
        {
            var Users = Context.Guild.Users;
            var text = new StringBuilder();

            foreach (var User in Users)
                if (!User.IsBot)
                    text.AppendLine($"{User.Nickname ?? User.Username} - {UserAccounts.GetAccount(User).country}");

            await SendPossiblyLongEmbed(Context.Channel, "Everyone's Country", text.ToString()).ConfigureAwait(false);
        }

        // Send an embed that might have over 2048 characters, so send it as multiple messages if it is
        public static async Task SendPossiblyLongEmbed(ISocketMessageChannel Channel, string Title, string Text)
        {
            if (Text.Length > 2048)
            {
                var size = 2048;
                var str = Text;
                var strings = new List<string>();

                for (int i = 0; i < str.Length; i += size)
                {
                    if (i + size > str.Length)
                        size = str.Length - i;
                    strings.Add(str.Substring(i, size));
                }

                foreach (var s in strings)
                    await Channel.PrintEmbed(Title, s, Utilities.Blue);
            }
            else
                await Channel.PrintEmbed(Title, Text, Utilities.Blue);
        }

        // Display various stats for the server
        public static async Task DisplayUserStats(SocketCommandContext Context)
        {
            var Users = Context.Guild.Users;
            var Text = new StringBuilder();

            var TotalUsers = 0;
            var UsersWithTimeSet = 0;
            var UsersWithCountrySet = 0;

            foreach (var User in Users)
            {
                if (!User.IsBot)
                {
                    TotalUsers++;
                    var account = UserAccounts.GetAccount(User);
                    if (account.localTime != 999)
                        UsersWithTimeSet++;
                    if (account.country != "Not set.")
                        UsersWithCountrySet++;
                }
            }

            Text.AppendLine($"Users in Server: {TotalUsers}").AppendLine()
                .AppendLine($"Users with time set up: {UsersWithTimeSet} ({Math.Round((double)UsersWithTimeSet / TotalUsers * 100, 0)}%)").AppendLine()
                .AppendLine($"Users with country set up: {UsersWithCountrySet} ({Math.Round((double)UsersWithCountrySet / TotalUsers * 100, 0)}%)");

            await Context.Channel.PrintEmbed("Time Stats", Text.ToString(), Utilities.Blue);
        }
    }
}
