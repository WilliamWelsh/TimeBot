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
using System.Xml.Schema;

namespace TimeBot
{
    public static class StatsHandler
    {
        // List of acceptable countries
        private static List<string> Countries;

        // The main embed that displays time for a target
        // If they don't have a country set, then the footer will be blank
        // This is to avoid a constant "no country set" message for users that don't want to set their country
        private static Embed StatsEmbed(UserAccount account, string name, string avatarURL) => new EmbedBuilder()
                .WithAuthor(new EmbedAuthorBuilder()
                .WithName(name)
                .WithIconUrl(avatarURL))
                //.WithDescription(GetTime(account, user))
                .WithDescription($"{GetTime(account, name)}")
                .WithColor(Utilities.GetUserColor(avatarURL))
                .WithFooter(GetCountry(account))
                .Build();

        // Display a User's local time
        public static string GetTime(UserAccount account, string name)
        {
            if (account.localTime == 999)
                return $"No time set for {name}.\nType `!timesetup` to set up your time and/or country.";
            var localTime = DateTime.Now.AddHours(account.localTime);
            return $"It's {localTime:h:mm tt} for {name}.\n{localTime:dddd, MMMM d.}";
        }

        // Display a User's country
        public static string GetCountry(UserAccount account) => account.country == "Not set." ? "" : account.country;

        // Display the time (and possibly country) for a target
        public static async Task DisplayStats(ISocketMessageChannel channel, string name, string avatarURL, ulong id)
        {
            await channel.SendMessageAsync("", false, StatsEmbed(UserAccounts.GetAccount(id), name, avatarURL));
        }

        // Display the time for users in a certain role
        public static async Task DisplayStats(SocketCommandContext Context, SocketRole Role)
        {
            var Users = (await EventHandler._restClient.GetGuildAsync(Context.Guild.Id)).GetUsersAsync();
            var text = new StringBuilder();
            await foreach (var List in Users)
            {
                foreach (var User in List)
                {
                    if (User.RoleIds.Contains(Role.Id))
                    {
                        var account = UserAccounts.GetAccount(User.Id);
                        text.AppendLine(account.localTime == 999
                            ? $"{User.Nickname ?? User.Username} - No Time Set"
                            : $"{User.Nickname ?? User.Username} - {Utilities.GetTime(account.localTime)}");
                    }
                }
            }

            await Context.Channel.PrintEmbed($"Time for {Role}", $"{text}", Role.Color);
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

            var account = UserAccounts.GetAccount(context.User.Id);
            account.localTime = hourDifference;
            UserAccounts.SaveAccounts();

            await context.Channel.PrintSuccess($"You have succesfully set your time.\n\n{GetTime(account, context.User.Username)}\n\nIf the time is wrong, try again. Type `!timesetup` for more help.");
        }

        // Set the time for yourself
        public static async Task SetSomeonesTime(SocketCommandContext context, SocketUser target, double hourDifference)
        {
            // If the target is talking about their self.. just set your own time..?
            if (context.User.Id == target.Id)
            {
                await SetTime(context, hourDifference).ConfigureAwait(false);
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

            var account = UserAccounts.GetAccount(target.Id);
            account.localTime = hourDifference;
            UserAccounts.SaveAccounts();

            await context.Channel.PrintSuccess($"{context.User.Mention} has succesfully set {target.Mention}'s time.\n\n{GetTime(account, target.Username)}\n\nIf the time is wrong, try again. Type `!timesetup` for more help.");
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
            var account = UserAccounts.GetAccount(user.Id);
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
                .AddField("Additional Help", "You can ask on GitHub or the support server (https://discord.gg/ga9V5pa) for additional help.\n\nOr add the Developer: Reverse#0069")
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
            var Users = (await EventHandler._restClient.GetGuildAsync(Context.Guild.Id)).GetUsersAsync();
            var text = new StringBuilder();
            await foreach (var List in Users)
            {
                foreach (var User in List)
                {
                    if (User.IsBot) continue;

                    var account = UserAccounts.GetAccount(User.Id);

                    text.AppendLine(account.localTime == 999
                        ? $"{User.Nickname ?? User.Username} - No Time Set"
                        : $"{User.Nickname ?? User.Username} - {Utilities.GetTime(account.localTime)}");
                }
            }

            await SendPossiblyLongEmbed(Context.Channel, "Time for All", $"{text}").ConfigureAwait(false);
        }

        // Display Country for everyone (requested feature)
        public static async Task DisplayEveryonesTimeByCountry(SocketCommandContext Context)
        {
            // Get a list of valid accounts
            // Valid account: A user that isn't a bot and has their country set up
            var Users = (await EventHandler._restClient.GetGuildAsync(Context.Guild.Id)).GetUsersAsync();
            var validAccounts = new List<CountryListItem>();
            await foreach (var List in Users)
            {
                foreach (var User in List)
                {
                    var account = UserAccounts.GetAccount(User.Id);
                    if (!User.IsBot && account.country != "Not set.")
                        validAccounts.Add(new CountryListItem
                        {
                            User = User,
                            UserAccount = account
                        });
                }
            }

            // Sort them by country name (alphabetical)
            // Then by time (earliest to latest)
            validAccounts = validAccounts.OrderBy(x => x.UserAccount.country)
                .ThenBy(x => x.UserAccount.localTime)
                .ToList();

            // Get a list of all the unique country name
            var countryNames = new HashSet<string>(validAccounts.Aggregate("", (current, x) => current + $" {x.UserAccount.country.Replace(" ", "_")}").Split(' '));

            // Create the result
            var embedFields = new List<EmbedFieldBuilder>();
            foreach (var countryName in countryNames)
            {
                // First one is whitespace for some reason?
                if (string.IsNullOrEmpty(countryName)) continue;

                // We had to put underscores in for the hashtable
                var actualCountryName = countryName.Replace("_", " ");

                // Get all users that have this country name
                var users = from a in validAccounts
                            where a.UserAccount.country == actualCountryName
                            select a;

                // Write everyone's time
                embedFields.Add(Utilities.MakeEmbedField($"{actualCountryName} {Utilities.GetCountryFlag(actualCountryName)}", $"{users.Aggregate("", (current, user) => current + $"{user.User.Nickname ?? user.User.Username} - {Utilities.GetTime(user.UserAccount.localTime)}\n")}\u200B"));
            }

            // Remove last blank line
            embedFields.Last().Value = embedFields.Last().Value.ToString().Replace("\u200B", "");

            // Send the result
            await Context.Channel.SendMessageAsync("", false, new EmbedBuilder()
                .WithTitle("Everyone's Time by Country")
                .WithColor(Utilities.Blue)
                .WithFields(embedFields)
                .Build());
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
                if (User.IsBot) continue;
                TotalUsers++;
                var account = UserAccounts.GetAccount(User.Id);
                if (account.localTime != 999)
                    UsersWithTimeSet++;
                if (account.country != "Not set.")
                    UsersWithCountrySet++;
            }

            Text.AppendLine($"Users in Server: {TotalUsers}").AppendLine()
                .AppendLine($"Users with time set up: {UsersWithTimeSet} ({Math.Round((double)UsersWithTimeSet / TotalUsers * 100, 0)}%)").AppendLine()
                .AppendLine($"Users with country set up: {UsersWithCountrySet} ({Math.Round((double)UsersWithCountrySet / TotalUsers * 100, 0)}%)");

            await Context.Channel.PrintEmbed("Time Stats", Text.ToString(), Utilities.Blue);
        }
    }
}