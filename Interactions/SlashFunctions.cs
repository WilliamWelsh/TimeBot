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
        public static async Task ShowTime(this SocketSlashCommand command, SocketGuildUser user) => await command.RespondAsync(embed: await StatsHandler.StatsEmbed(UserAccounts.GetAccount(user.Id), user.Nickname ?? user.Username, user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl()));

        // /time (RestUser)
        public static async Task ShowTime(this SocketSlashCommand command, RestGuildUser user) => await command.RespondAsync(embed: await StatsHandler.StatsEmbed(UserAccounts.GetAccount(user.Id), user.Nickname ?? user.Username, user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl()));

        // /country (SocketGuildUser)
        public static async Task ShowCountry(this SocketSlashCommand command, SocketGuildUser user)
        {
            var avatarURL = user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl();

            var country = StatsHandler.GetCountry(UserAccounts.GetAccount(user.Id));

            await command.RespondAsync(embed: new EmbedBuilder()
                .WithAuthor(new EmbedAuthorBuilder()
                    .WithName(user.Nickname ?? user.Username)
                    .WithIconUrl(avatarURL))
                .WithColor(await Utilities.GetUserColor(avatarURL))
                .WithDescription(country == "" ? "No country has been set up. Do `/countryset [country name]` to set your country.\n\nExample: `/countryset canada`" : country)
                .Build());
        }

        // /country (RestUser)
        public static async Task ShowCountry(this SocketSlashCommand command, RestGuildUser user)
        {
            var avatarURL = user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl();

            var country = StatsHandler.GetCountry(UserAccounts.GetAccount(user.Id));

            await command.RespondAsync(embed: new EmbedBuilder()
                .WithAuthor(new EmbedAuthorBuilder()
                    .WithName(user.Nickname ?? user.Username)
                    .WithIconUrl(avatarURL))
                .WithColor(await Utilities.GetUserColor(avatarURL))
                .WithDescription(country == "" ? "No country has been set up. Do `/countryset [country name]` to set your country.\n\nExample: `/countryset canada`" : country)
                .Build());
        }

        // /timeall
        public static async Task ShowTimeForAll(this SocketSlashCommand command)
        {
            var Users = (await EventHandler._restClient.GetGuildAsync(((SocketGuildUser)command.User).Guild.Id)).GetUsersAsync();
            var validAccounts = new List<ListItem>();

            await foreach (var List in Users)
            {
                foreach (var User in List)
                {
                    var account = UserAccounts.GetAccount(User.Id);
                    if (!User.IsBot && account.timeZoneId != "Not set.")
                        validAccounts.Add(new ListItem
                        {
                            User = User,
                            UserAccount = account,
                            Time = TimeZones.GetRawTimeByTimeZone(account.timeZoneId)
                        });
                }
            }

            // Sort by earliest time to latest
            var sortedList = validAccounts.OrderBy(u => u.Time);

            var firstLine = new StringBuilder();
            var secondLIne = new StringBuilder();

            var lastTime = sortedList.ElementAt(0).Time;
            foreach (var item in sortedList)
            {
                var text = $"{item.User.Nickname ?? item.User.Username} - {TimeZones.GetTimeByTimeZone(item.UserAccount.timeZoneId)}";

                if (lastTime != item.Time)
                {
                    lastTime = item.Time;
                    text = $"\n{text}";
                }

                if (firstLine.ToString().Length < 1800)
                    firstLine.AppendLine(text);
                else
                    secondLIne.AppendLine(text);
            }

            await command.RespondAsync(embed: new EmbedBuilder()
                .WithColor(Utilities.Blue)
                .WithTitle("Time for All")
                .WithDescription(firstLine.ToString())
                .Build());

            if (secondLIne.ToString().Length > 0)
                await command.Channel.SendMessageAsync(embed: new EmbedBuilder()
                    .WithColor(Utilities.Blue)
                    .WithTitle("Time for All")
                    .WithDescription(secondLIne.ToString())
                    .Build());
        }

        // /countryall
        public static async Task ShowCountryForAll(this SocketInteraction interaction)
        {
            try
            {
                var Users = (await EventHandler._restClient.GetGuildAsync(((SocketGuildUser)interaction.User).Guild.Id)).GetUsersAsync();
                var validAccounts = new List<ListItem>();
                await foreach (var List in Users)
                {
                    foreach (var User in List)
                    {
                        var account = UserAccounts.GetAccount(User.Id);
                        if (!User.IsBot && account.country != "Not set.")
                            validAccounts.Add(new ListItem
                            {
                                User = User,
                                UserAccount = account,
                                Time = TimeZones.GetRawTimeByTimeZone(account.timeZoneId)
                            });
                    }
                }

                // Sort them by country name (alphabetical)
                // Then by time (earliest to latest)
                validAccounts = validAccounts.OrderBy(x => x.UserAccount.country)
                    .ThenBy(x => x.Time)
                    .ThenBy(x => x.User.Nickname ?? x.User.Username)
                    .ToList();

                // Get a list of all the unique country name
                var countryNames = validAccounts.Aggregate("", (current, x) => current + $" {x.UserAccount.country.Replace(" ", "_")}").Split(' ').ToList();
                countryNames = countryNames.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();

                var firstFieldList = new List<EmbedFieldBuilder>();
                var secondFieldList = new List<EmbedFieldBuilder>();
                var totalCountries = 0;

                for (int i = 0; i < countryNames.Count; i++)
                {
                    var countryName = countryNames.ElementAt(i);

                    // We had to put underscores in for the hashtable
                    var actualCountryName = countryName.Replace("_", " ");

                    // Flag Emoji
                    var flagEmoji = Countries.List.FirstOrDefault(c => String.Equals(c.Key, countryName, StringComparison.CurrentCultureIgnoreCase)).Value;

                    // Get all users that have this country name
                    var users = from a in validAccounts
                                where a.UserAccount.country == actualCountryName
                                select a;

                    var description = $"{users.Aggregate("", (current, user) => current + $"{user.User.Nickname ?? user.User.Username} - {TimeZones.GetTimeByTimeZone(user.UserAccount.timeZoneId)}\n")}\u200B";

                    // Split the description into two field if it's too long
                    var descriptionTwo = "";
                    if (description.Length > 950) // Max is 1024 charcters per field
                    {
                        var items = description.Split('\n');
                        description = string.Join("\n", items.Take(items.Count() - 5));
                        descriptionTwo = string.Join("\n", items.Skip(items.Count() - 5));
                    }

                    if (firstFieldList.Count < 24)
                    {
                        firstFieldList.Add(new EmbedFieldBuilder()
                            .WithName($"{actualCountryName} {flagEmoji}")
                            .WithValue(description)
                            .WithIsInline(false));

                        totalCountries++;

                        if (descriptionTwo != "")
                        {
                            firstFieldList.Add(new EmbedFieldBuilder()
                                .WithName($"{actualCountryName} {flagEmoji} (Continued)")
                                .WithValue(descriptionTwo)
                                .WithIsInline(false));
                        }
                    }
                    else
                    {
                        secondFieldList.Add(new EmbedFieldBuilder()
                            .WithName($"{actualCountryName} {flagEmoji}")
                            .WithValue(description)
                            .WithIsInline(false));

                        totalCountries++;

                        if (descriptionTwo != "")
                        {
                            secondFieldList.Add(new EmbedFieldBuilder()
                                .WithName($"{actualCountryName} {flagEmoji} (Continued)")
                                .WithValue(descriptionTwo)
                                .WithIsInline(false));
                        }
                    }
                }

                // Remove the last blank line in the field lists
                if (firstFieldList.Count > 1)
                    firstFieldList[firstFieldList.Count - 1] = new EmbedFieldBuilder()
                                .WithName(firstFieldList[firstFieldList.Count - 1].Name)
                                .WithValue(firstFieldList[firstFieldList.Count - 1].Value.ToString().Replace("\u200B", ""))
                                .WithIsInline(false);

                if (secondFieldList.Count > 1)
                    secondFieldList[secondFieldList.Count - 1] = new EmbedFieldBuilder()
                                .WithName(secondFieldList[secondFieldList.Count - 1].Name)
                                .WithValue(secondFieldList[secondFieldList.Count - 1].Value.ToString().Replace("\u200B", ""))
                                .WithIsInline(false);

                var firstEmbed = new EmbedBuilder()
                    .WithColor(Utilities.Blue)
                    .WithTitle($"Everyone's Time by Country ({totalCountries})")
                    .WithFields(firstFieldList)
                    .Build();

                var secondEmbed = new EmbedBuilder()
                    .WithColor(Utilities.Blue)
                    .WithTitle($"Everyone's Time by Country ({totalCountries})")
                    .WithFields(secondFieldList)
                    .Build();

                var regularRefreshButton = new ComponentBuilder()
                    .WithButton("Refresh", "refresh_country", ButtonStyle.Secondary)
                    .Build();

                switch (interaction)
                {
                    case SocketSlashCommand command:
                        if (secondFieldList.Count == 0)
                        {
                            await command.RespondAsync(embed: firstEmbed,
                            component: regularRefreshButton);
                        }
                        else
                        {

                            await command.RespondAsync("You have so many members that it couldn't fit in one message, so I sent two 😀", ephemeral: true);
                            var firstMessage = await command.Channel.SendMessageAsync(embed: firstEmbed);
                            await command.Channel.SendMessageAsync(embed: secondEmbed,
                            component: new ComponentBuilder()
                                .WithButton("Refresh", $"refresh_country_{firstMessage.Id}", ButtonStyle.Secondary)
                                .Build());
                        }
                        break;

                    case SocketMessageComponent button:
                        var data = button.Data.CustomId.Split("_");

                        // 3 data meaans we have a first message id
                        if (data.Count() == 3)
                        {
                            var message = (RestUserMessage)await button.Channel.GetMessageAsync(Convert.ToUInt64(data[2]));
                            await message.ModifyAsync(x => x.Embed = firstEmbed);
                            await button.UpdateAsync(x =>
                            {
                                x.Embed = secondEmbed;
                                x.Components = new ComponentBuilder()
                                .WithButton("Refresh", $"refresh_country_{data[2]}", ButtonStyle.Secondary)
                                .Build();
                            });
                        }
                        else
                        {
                            await button.UpdateAsync(x =>
                            {
                                x.Embed = firstEmbed;
                                x.Components = regularRefreshButton;
                            });
                        }
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.Source);
            }
        }

        // /timehelp
        public static async Task ShowTimeHelp(this SocketSlashCommand command) =>
            await command.RespondAsync(embed: new EmbedBuilder()
                .WithTitle("Time Bot Help")
                .WithColor(Utilities.Blue)
                .WithDescription("Hello, I am TimeBot. I can provide the local time and country for other users. Data is saved across all servers.")
                .AddField("Commands", "`/timesetup` Set up your time\n`/time` View your time.\n`/time @mentionedUser` View a user's local time.\n`/countryset [country name]` Set your country.\n`/timestats` View stats for the bot.\n`/countryall` View everyone's country & time\n`/timeall` View the time for everyone")
                .AddField("Additional Help", $"You can ask on GitHub or the support server ({Utilities.SupportServer}) for additional help.\n\nOr add the Developer: Reverse#0069")
                .AddField("GitHub", "https://github.com/WilliamWelsh/TimeBot")
                .Build());

        // /countryset
        public static async Task SetCountry(this SocketSlashCommand command)
        {
            var country = command.Data.Options.ElementAt(0).Value.ToString();

            // Check if it's a valid country name
            if (!Countries.List.Any(c => String.Equals(c.Key, country, StringComparison.OrdinalIgnoreCase)))
            {
                await command.RespondAsync(embed: new EmbedBuilder()
                    .WithTitle("Error")
                    .WithDescription("That is not a valid country name, please try again.\n\nExamples:\n`/countryset united states`\n`/countryset united kingdom`\n`/countryset canada`\n\nYou can find a list of valid countries here: https://github.com/WilliamWelsh/TimeBot/blob/master/countries.txt")
                    .WithColor(Utilities.Red)
                    .Build(), ephemeral: true);
                return;
            }

            // Save the target's country
            var account = UserAccounts.GetAccount(command.User.Id);
            account.country = Countries.List.FirstOrDefault(c => String.Equals(c.Key, country, StringComparison.OrdinalIgnoreCase)).Key;
            UserAccounts.SaveAccounts();

            // Send them the result
            await command.RespondAsync(embed: new EmbedBuilder()
                .WithTitle("Success")
                .WithDescription($"You have successfully set your country to {account.country}.\n\nIf this is an error, you can run `/countryset [country name]` again.")
                .WithColor(Utilities.Green)
                .Build(), ephemeral: true);
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
                .AddField("Links", $"[Invite](https://discord.com/api/oauth2/authorize?client_id=529569000028373002&permissions=2048&scope=bot%20applications.commands) | [Vote](\n\nhttps://top.gg/bot/529569000028373002/vote) | [GitHub](https://github.com/WilliamWelsh/TimeBot) | [Support Server]({Utilities.SupportServer})")
                .Build()).ConfigureAwait(false);
        }

        // /timesetup
        public static async Task TimeSetupForSelf(this SocketInteraction interaction)
        {
            var embed = new EmbedBuilder()
                .WithColor(Utilities.Blue)
                .WithDescription($"Please select your timezone 😀\n\nIf you need help please join the Support Server: {Utilities.SupportServer}")
                .Build();

            if (interaction is SocketSlashCommand slashCommand)
            {
                await slashCommand.RespondAsync(embed: embed, component: TimeZones.GetPaginatedTimeZones(0, interaction.User.Id), ephemeral: true);
            }
            else if (interaction is SocketMessageComponent buttonCommand)
            {
                var args = buttonCommand.Data.CustomId.Split("_");

                // Previous and Next Page buttons
                if (buttonCommand.Data.CustomId.StartsWith("lastpage"))
                {
                    await buttonCommand.UpdateAsync(x =>
                    {
                        x.Embed = embed;
                        x.Components = TimeZones.GetPaginatedTimeZones(Convert.ToInt32(args[1]) - 1, interaction.User.Id);
                    });
                }
                else if (buttonCommand.Data.CustomId.StartsWith("nextpage"))
                {
                    try
                    {
                        await buttonCommand.UpdateAsync(x =>
                        {
                            x.Embed = embed;
                            x.Components =
                                TimeZones.GetPaginatedTimeZones(Convert.ToInt32(args[1]) + 1, interaction.User.Id);
                        });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                // Clicked a timezone button
                var account = UserAccounts.GetAccount(ulong.Parse(args[1]));
                account.timeZoneId = args[2];
                UserAccounts.SaveAccounts();

                await buttonCommand.UpdateAsync(x =>
                {
                    x.Embed = new EmbedBuilder()
                        .WithTitle("Success")
                        .WithDescription(
                            $"You have succesfully set your time.\n\n{StatsHandler.GetTime(account, ((SocketGuildUser)buttonCommand.User).Nickname ?? buttonCommand.User.Username)}\n\nIf the time is wrong, try again. Do `/timesetup` again.")
                        .WithColor(Utilities.Green)
                        .Build();
                    x.Components = null;
                });
            }
        }

        // Set a user's country
        public static async Task SetUserCountry(this SocketSlashCommand command)
        {
            // Check if they're an admin
            if (!((SocketGuildUser)command.User).GuildPermissions.Administrator)
            {
                await command.RespondAsync(embed: new EmbedBuilder()
                    .WithTitle("Error")
                    .WithDescription("You do not have permission to use this command.")
                    .WithColor(Utilities.Red)
                    .Build(), ephemeral: true);
                return;
            }

            // Get the account of the user
            var target = await EventHandler._restClient.GetGuildUserAsync(((SocketGuildUser)command.User).Guild.Id, ((SocketGuildUser)command.Data.Options.ElementAt(0).Value).Id);

            var country = command.Data.Options.ElementAt(1).Value.ToString();

            // Check if it's a valid country name
            if (!Countries.List.Any(c => String.Equals(c.Key, country, StringComparison.OrdinalIgnoreCase)))
            {
                await command.RespondAsync(embed: new EmbedBuilder()
                    .WithTitle("Error")
                    .WithDescription("That is not a valid country name, please try again.\n\nExamples:\n`/countryset united states`\n`/countryset united kingdom`\n`/countryset canada`\n\nYou can find a list of valid countries here: https://github.com/WilliamWelsh/TimeBot/blob/master/countries.txt")
                    .WithColor(Utilities.Red)
                    .Build(), ephemeral: true);
                return;
            }

            // Save the target's country
            var account = UserAccounts.GetAccount(target.Id);
            account.country = Countries.List.FirstOrDefault(c => String.Equals(c.Key, country, StringComparison.OrdinalIgnoreCase)).Key;
            UserAccounts.SaveAccounts();

            // Send them the result
            await command.RespondAsync(embed: new EmbedBuilder()
                .WithTitle("Success")
                .WithDescription($"You have successfully {target.Mention}'s country to {account.country}.\n\nIf this is an error, you can run `/user-country-set [country name]` again.")
                .WithColor(Utilities.Green)
                .Build(), ephemeral: true);
        }

        // Set a user's time
        public static async Task SetTimeForSomeoneElse(this SocketInteraction interaction)
        {
            /// Check if they're an admin
            if (!((SocketGuildUser)interaction.User).GuildPermissions.Administrator)
            {
                await interaction.RespondAsync(embed: new EmbedBuilder()
                    .WithTitle("Error")
                    .WithDescription("You do not have permission to use this command.")
                    .WithColor(Utilities.Red)
                    .Build(), ephemeral: true);
                return;
            }

            var embed = new EmbedBuilder()
                .WithColor(Utilities.Blue)
                .WithDescription($"Please select the timezone 😀\n\nIf you need help please join the Support Server: {Utilities.SupportServer}")
                .Build();

            if (interaction is SocketSlashCommand slashCommand)
            {
                await slashCommand.RespondAsync(embed: embed, component: TimeZones.GetPaginatedTimeZones(0, ((SocketGuildUser)slashCommand.Data.Options.ElementAt(0).Value).Id, true), ephemeral: true);
            }
            else if (interaction is SocketMessageComponent buttonCommand)
            {
                var args = buttonCommand.Data.CustomId.Split("_");

                var target = args[2];

                // Previous and Next Page buttons
                if (buttonCommand.Data.CustomId.StartsWith("otherlastpage"))
                {
                    await buttonCommand.UpdateAsync(x =>
                    {
                        x.Embed = embed;
                        x.Components = TimeZones.GetPaginatedTimeZones(Convert.ToInt32(args[1]) - 1, Convert.ToUInt64(target), true);
                    });
                }
                else if (buttonCommand.Data.CustomId.StartsWith("othernextpage"))
                {
                    try
                    {
                        await buttonCommand.UpdateAsync(x =>
                        {
                            x.Embed = embed;
                            x.Components =
                                TimeZones.GetPaginatedTimeZones(Convert.ToInt32(args[1]) + 1, Convert.ToUInt64(target), true);
                        });
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                // Clicked a timezone button
                var account = UserAccounts.GetAccount(ulong.Parse(args[1]));
                account.timeZoneId = args[2];
                UserAccounts.SaveAccounts();

                await buttonCommand.UpdateAsync(x =>
                {
                    x.Embed = new EmbedBuilder()
                        .WithTitle("Success")
                        .WithDescription(
                            $"You have succesfully set your time.\n\n{StatsHandler.GetTime(account, "for them")}\n\nIf the time is wrong, try again. Do `/set-user-time` again.")
                        .WithColor(Utilities.Green)
                        .Build();
                    x.Components = null;
                });
            }
        }
    }
}