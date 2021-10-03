using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using TimeBot.UserData;

namespace TimeBot.Interactions
{
    public static class SlashFunctions
    {
        // /time (SocketGuildUser)
        public static async Task ShowTime(this SocketSlashCommand command, SocketGuildUser user)
        {
            await command.RespondAsync(embed: await StatsHandler.StatsEmbed(UserAccounts.GetAccount(user.Id), user.Nickname ?? user.Username, user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl()));
        }

        // /time (RestUser)
        public static async Task ShowTime(this SocketSlashCommand command, RestGuildUser user)
        {
            await command.RespondAsync(embed: await StatsHandler.StatsEmbed(UserAccounts.GetAccount(user.Id), user.Nickname ?? user.Username, user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl()));
        }

        // /country (SocketGuildUser)
        public static async Task ShowCountry(this SocketSlashCommand command, SocketGuildUser user)
        {
            var avatarURL = user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl();

            await command.RespondAsync(embed: new EmbedBuilder()
                .WithAuthor(new EmbedAuthorBuilder()
                    .WithName(user.Nickname ?? user.Username)
                    .WithIconUrl(avatarURL))
                .WithColor(await Utilities.GetUserColor(avatarURL))
                .WithDescription(StatsHandler.GetCountry(UserAccounts.GetAccount(user.Id)))
                .Build());
        }

        // /country (RestUser)
        public static async Task ShowCountry(this SocketSlashCommand command, RestGuildUser user)
        {
            var avatarURL = user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl();

            await command.RespondAsync(embed: new EmbedBuilder()
                .WithAuthor(new EmbedAuthorBuilder()
                    .WithName(user.Nickname ?? user.Username)
                    .WithIconUrl(avatarURL))
                .WithColor(await Utilities.GetUserColor(avatarURL))
                .WithDescription(StatsHandler.GetCountry(UserAccounts.GetAccount(user.Id)))
                .Build());
        }

        // /timeall
        public static async Task ShowTimeForAll(this SocketSlashCommand command)
        {
            var Users = (await EventHandler._restClient.GetGuildAsync(((SocketGuildUser)command.User).Guild.Id)).GetUsersAsync();
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

            await command.RespondAsync(embed: new EmbedBuilder()
                .WithColor(Utilities.Blue)
                .WithTitle("Time for All")
                .WithDescription(text.ToString())
                .Build());
        }

        // /countryall
        public static async Task ShowCountryForAll(this SocketSlashCommand command)
        {
            var Users = (await EventHandler._restClient.GetGuildAsync(((SocketGuildUser)command.User).Guild.Id)).GetUsersAsync();
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
            var countryNames = (validAccounts.Aggregate("", (current, x) => current + $" {x.UserAccount.country.Replace(" ", "_")}").Split(' ')).ToList();
            countryNames = countryNames.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();

            var fields = new List<EmbedFieldBuilder>();

            for (int i = 0; i < countryNames.Count; i++)
            {
                var countryName = countryNames.ElementAt(i);

                // We had to put underscores in for the hashtable
                var actualCountryName = countryName.Replace("_", " ");

                // Get all users that have this country name
                var users = from a in validAccounts
                            where a.UserAccount.country == actualCountryName
                            select a;

                fields.Add(new EmbedFieldBuilder()
                    .WithName($"{actualCountryName} {Utilities.GetCountryFlag(actualCountryName)}")
                    .WithValue($"{users.Aggregate("", (current, user) => current + $"{user.User.Nickname ?? user.User.Username} - {Utilities.GetTime(user.UserAccount.localTime)}\n")}\u200B")
                    .WithIsInline(false));
            }

            await command.RespondAsync(embed: new EmbedBuilder()
                .WithColor(Utilities.Blue)
                .WithTitle("Everyone's Time by Country")
                .WithFields(fields)
                .Build());
        }

        // /timehelp
        public static async Task ShowTimeHelp(this SocketSlashCommand command) =>
            await command.RespondAsync(embed: new EmbedBuilder()
                .WithTitle("Time Bot Help")
                .WithColor(Utilities.Blue)
                .WithDescription("Hello, I am TimeBot. I can provide the local time and country for other users. Data is saved across all servers.")
                .AddField("Commands", "`/timesetup` Help on setting up your time (and country if you want)\n`!time` View your time.\n`/time @mentionedUser` View a target's local time.\n`/timeset [number]` Set your local time.\n`/countryset [country name]` Set your country.\n`/timestats` View stats for the bot.")
                .AddField("Additional Help", "You can ask on GitHub or the support server (https://discord.gg/ga9V5pa) for additional help.\n\nOr add the Developer: Reverse#0069")
                .AddField("GitHub", "https://github.com/WilliamWelsh/TimeBot")
                .Build());

        // /timeset
        public static async Task SetTime(this SocketSlashCommand command)
        {
            var hourDifference = Convert.ToDouble(command.Data.Options.ElementAt(0).Value.ToString());

            if (hourDifference < -24 || hourDifference > 24)
            {
                await command.PrintError("Invalid hour difference. The input must be between -24 and 24. Please run `/timesetup` for more help.");
                return;
            }

            var minuteDifference = (int)((decimal)hourDifference % 1 * 100);
            if (minuteDifference != 50 && minuteDifference != 0 && minuteDifference != -50)
            {
                await command.PrintError("Invalid minute difference. The minute offset can only be 0 or 0.5, such as 1.5 or 5.5. Please run `/timesetup` for more help.");
                return;
            }

            var account = UserAccounts.GetAccount(command.User.Id);
            account.localTime = hourDifference;
            UserAccounts.SaveAccounts();

            await command.RespondAsync(embed: new EmbedBuilder()
                .WithTitle("Success")
                .WithDescription($"You have succesfully set your time.\n\n{StatsHandler.GetTime(account, ((SocketGuildUser)command.User).Nickname ?? command.User.Username)}\n\nIf the time is wrong, try again. Type `/timesetup` for more help.")
                .WithColor(Utilities.Green)
                .Build());
        }

        // /countryset
        public static async Task SetCountry(this SocketSlashCommand command)
        {
            var country = command.Data.Options.ElementAt(0).Value.ToString();

            if (!StatsHandler.Countries.Contains(country, StringComparer.CurrentCultureIgnoreCase))
            {
                await command.PrintError("Country not valid. Please try again.\n\nExamples:\n`/countryset united states`\n`/country set united kingdom`\n`/country set canada`\n\nList of valid countries: https://raw.githubusercontent.com/WilliamWelsh/TimeBot/master/TimeBot/countries.txt");
                return;
            }

            // Find the country input and set it to the capitlized version
            var index = StatsHandler.Countries.FindIndex(x => x.Equals(country, StringComparison.OrdinalIgnoreCase));

            // Save the target's country
            var account = UserAccounts.GetAccount(command.User.Id);
            account.country = StatsHandler.Countries.ElementAt(index);
            UserAccounts.SaveAccounts();

            await command.RespondAsync(embed: new EmbedBuilder()
                .WithTitle("Success")
                .WithDescription($"You have successfully set your country to {account.country}.\n\nIf this is an error, you can run `/countryset [country name]` again.")
                .WithColor(Utilities.Green)
                .Build());
        }

        // /timestats
        public static async Task ShowStats(this SocketSlashCommand command)
        {
            var totalMembers = EventHandler._socketClient.Guilds.Sum(Guild => Guild.MemberCount);

            await command.RespondAsync(embed: new EmbedBuilder()
                .WithTitle("Bot Info")
                .WithColor(Utilities.Blue)
                .WithThumbnailUrl("https://cdn.discordapp.com/avatars/529569000028373002/b5100de6821ee1c4714ac022c3cd39d9.png?size=128")
                .AddField("Library", "Discord.Net.Labs")
                .AddField("Servers", EventHandler._socketClient.Guilds.Count)
                .AddField("Members", totalMembers.ToString("#,##0"))
                .AddField("Developer", "Reverse#0069")
                .AddField("Color", "Suggested Role Color for Me: `#7fa6d0`")
                .AddField("Links", "[Invite](https://discord.com/api/oauth2/authorize?client_id=529569000028373002&permissions=2048&scope=bot%20applications.commands) | [Vote](\n\nhttps://top.gg/bot/529569000028373002/vote) | [GitHub](https://github.com/WilliamWelsh/TimeBot) | [Support Server](https://discord.gg/ga9V5pa)")
                .Build()).ConfigureAwait(false);
        }

        // Send an error message
        public static async Task PrintError(this SocketSlashCommand command, string description) =>
            await command.RespondAsync(embed: new EmbedBuilder()
                .WithTitle("Error")
                .WithDescription(description)
                .WithColor(Utilities.Red)
                .Build());
    }
}