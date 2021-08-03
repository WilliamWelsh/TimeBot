using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Discord.Rest;
using Discord.WebSocket;
using Newtonsoft.Json;
using TimeBot.UserData;

namespace TimeBot.Interactions
{
    public static class SlashFunctions
    {
        // /time
        public static async Task ShowTime(this SocketInteraction interaction, SocketGuildUser user)
        {
            var name = user.Nickname ?? user.Username;
            var avatarURL = user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl();

            dynamic interactionResponse = new ExpandoObject();
            interactionResponse.type = 4;
            interactionResponse.data = new
            {
                embeds = new[]
                {
                    new
                    {
                        author = new
                        {
                            name,
                            icon_url = avatarURL
                        },
                        description = StatsHandler.GetTime(UserAccounts.GetAccount(user.Id), name),
                        color = await Utilities.GetUserColorForSlash(avatarURL)
                    }
                }
            };

            await interaction.RespondImmediately(new StringContent(JsonConvert.SerializeObject(interactionResponse), Encoding.UTF8, "application/json"));
        }

        // /time
        public static async Task ShowTime(this SocketInteraction interaction, RestGuildUser user)
        {
            Console.WriteLine(user.Id);
            var name = user.Nickname ?? user.Username;
            var avatarURL = user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl();

            dynamic interactionResponse = new ExpandoObject();
            interactionResponse.type = 4;
            interactionResponse.data = new
            {
                embeds = new[]
                {
                    new
                    {
                        author = new
                        {
                            name,
                            icon_url = avatarURL
                        },
                        description = StatsHandler.GetTime(UserAccounts.GetAccount(user.Id), name),
                        color = await Utilities.GetUserColorForSlash(avatarURL)
                    }
                }
            };

            await interaction.RespondImmediately(new StringContent(JsonConvert.SerializeObject(interactionResponse), Encoding.UTF8, "application/json"));
        }

        // /country
        public static async Task ShowCountry(this SocketInteraction interaction, SocketGuildUser user)
        {
            var name = user.Nickname ?? user.Username;
            var avatarURL = user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl();

            dynamic interactionResponse = new ExpandoObject();
            interactionResponse.type = 4;
            interactionResponse.data = new
            {
                embeds = new[]
                {
                    new
                    {
                        author = new
                        {
                            name,
                            icon_url = avatarURL
                        },
                        description = StatsHandler.GetCountry(UserAccounts.GetAccount(user.Id)),
                        color = await Utilities.GetUserColorForSlash(avatarURL)
                    }
                }
            };

            await interaction.RespondImmediately(new StringContent(JsonConvert.SerializeObject(interactionResponse), Encoding.UTF8, "application/json"));
        }

        // /country
        public static async Task ShowCountry(this SocketInteraction interaction, RestGuildUser user)
        {
            Console.WriteLine(user.Id);
            var name = user.Nickname ?? user.Username;
            var avatarURL = user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl();

            dynamic interactionResponse = new ExpandoObject();
            interactionResponse.type = 4;
            interactionResponse.data = new
            {
                embeds = new[]
                {
                    new
                    {
                        author = new
                        {
                            name,
                            icon_url = avatarURL
                        },
                        description = StatsHandler.GetCountry(UserAccounts.GetAccount(user.Id)),
                        color = await Utilities.GetUserColorForSlash(avatarURL)
                    }
                }
            };

            await interaction.RespondImmediately(new StringContent(JsonConvert.SerializeObject(interactionResponse), Encoding.UTF8, "application/json"));
        }

        // /timeall
        public static async Task ShowTimeForAll(this SocketInteraction interaction)
        {
            var Users = (await EventHandler._restClient.GetGuildAsync(interaction.Guild.Id)).GetUsersAsync();
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

            dynamic interactionResponse = new ExpandoObject();
            interactionResponse.type = 4;
            interactionResponse.data = new
            {
                embeds = new[]
                {
                    new
                    {
                        title = "Time for All",
                        description = text.ToString(),
                        color = 8365776
                    }
                }
            };

            await interaction.RespondImmediately(new StringContent(JsonConvert.SerializeObject(interactionResponse), Encoding.UTF8, "application/json"));
        }

        // /countryall
        public static async Task ShowCountryForAll(this SocketInteraction interaction)
        {
            var Users = (await EventHandler._restClient.GetGuildAsync(interaction.Guild.Id)).GetUsersAsync();
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

            var fields = new object[countryNames.Count];

            for (int i = 0; i < countryNames.Count; i++)
            {
                var countryName = countryNames.ElementAt(i);

                // We had to put underscores in for the hashtable
                var actualCountryName = countryName.Replace("_", " ");

                // Get all users that have this country name
                var users = from a in validAccounts
                            where a.UserAccount.country == actualCountryName
                            select a;

                fields[i] = new
                {
                    name = $"{actualCountryName} {Utilities.GetCountryFlag(actualCountryName)}",
                    value = $"{users.Aggregate("", (current, user) => current + $"{user.User.Nickname ?? user.User.Username} - {Utilities.GetTime(user.UserAccount.localTime)}\n")}\u200B",
                    inline = false
                };
            }

            dynamic interactionResponse = new ExpandoObject();
            interactionResponse.type = 4;
            interactionResponse.data = new
            {
                embeds = new[]
                {
                    new
                    {
                        title = "Everyone's Time by Country",
                        color = 8365776,
                        fields
                    }
                }
            };

            await interaction.RespondImmediately(new StringContent(JsonConvert.SerializeObject(interactionResponse), Encoding.UTF8, "application/json"));
        }

        // /timehelp
        public static async Task ShowTimeHelp(this SocketInteraction interaction)
        {
            dynamic interactionResponse = new ExpandoObject();
            interactionResponse.type = 4;
            interactionResponse.data = new
            {
                embeds = new[]
                {
                    new
                    {
                        title = "Time Bot Help",
                        description = "Hello, I am TimeBot. I can provide the local time and country for other users. Data is saved across all servers.",
                        color = "8365776",
                        fields = new[]
                        {
                            new
                            {
                                name = "Commands",
                                value = "`/timesetup` Help on setting up your time (and country if you want)\n`/time` View your time.\n`/time @mentionedUser` View a target's local time.\n`/timeset [number]` Set your local time.\n`/countryset [country name]`Set your country.",
                                //inline = false
                            },
                            new
                            {
                                name = "Additional Help",
                                value = "You can ask on GitHub or the support server (https://discord.gg/ga9V5pa) for additional help.\n\nOr add the Developer: Reverse#0069"
                            },
                            new
                            {
                                name = "Invite Link",
                                value = "https://discord.com/api/oauth2/authorize?client_id=529569000028373002&permissions=2048&scope=bot%20applications.commands"
                            },
                            new
                            {
                                name = "GitHub",
                                value = "https://github.com/WilliamWelsh/TimeBot"
                            }
                        }
                    }
                }
            };

            await interaction.RespondImmediately(new StringContent(JsonConvert.SerializeObject(interactionResponse), Encoding.UTF8, "application/json"));
        }

        // /timeset
        public static async Task SetTime(this SocketInteraction interaction)
        {
            var hourDifference = Convert.ToDouble(interaction.Data.Options.ElementAt(0).Value.ToString());

            if (hourDifference < -24 || hourDifference > 24)
            {
                await interaction.ImmediateError("Invalid hour difference. The input must be between -24 and 24. Please run `/timesetup` for more help.");
                return;
            }

            var minuteDifference = (int)((decimal)hourDifference % 1 * 100);
            if (minuteDifference != 50 && minuteDifference != 0 && minuteDifference != -50)
            {
                await interaction.ImmediateError("Invalid minute difference. The minute offset can only be 0 or 0.5, such as 1.5 or 5.5. Please run `/timesetup` for more help.");
                return;
            }

            var account = UserAccounts.GetAccount(interaction.User.Id);
            account.localTime = hourDifference;
            UserAccounts.SaveAccounts();

            dynamic interactionResponse = new ExpandoObject();
            interactionResponse.type = 4;
            interactionResponse.data = new
            {
                embeds = new[]
                {
                    new
                    {
                        title = "Success",
                        description = $"You have succesfully set your time.\n\n{StatsHandler.GetTime(account, interaction.User.Nickname ?? interaction.User.Username)}\n\nIf the time is wrong, try again. Type `/timesetup` for more help.",
                        color = 2067276
                    }
                }
            };

            await interaction.RespondImmediately(new StringContent(JsonConvert.SerializeObject(interactionResponse), Encoding.UTF8, "application/json"));
        }

        // /countryset
        public static async Task SetCountry(this SocketInteraction interaction)
        {
            var country = interaction.Data.Options.ElementAt(0).Value.ToString();

            if (!StatsHandler.Countries.Contains(country, StringComparer.CurrentCultureIgnoreCase))
            {
                await interaction.ImmediateError("Country not valid. Please try again.\n\nExamples:\n`/countryset united states`\n`/country set united kingdom`\n`/country set canada`\n\nList of valid countries: https://raw.githubusercontent.com/WilliamWelsh/TimeBot/master/TimeBot/countries.txt");
                return;
            }

            // Find the country input and set it to the capitlized version
            var index = StatsHandler.Countries.FindIndex(x => x.Equals(country, StringComparison.OrdinalIgnoreCase));

            // Save the target's country
            var account = UserAccounts.GetAccount(interaction.User.Id);
            account.country = StatsHandler.Countries.ElementAt(index);
            UserAccounts.SaveAccounts();

            dynamic interactionResponse = new ExpandoObject();
            interactionResponse.type = 4;
            interactionResponse.data = new
            {
                embeds = new[]
                {
                    new
                    {
                        title = "Success",
                        description = $"You have successfully set your country to {account.country}.\n\nIf this is an error, you can run `/countryset [country name]` again.",
                        color = 2067276
                    }
                }
            };

            await interaction.RespondImmediately(new StringContent(JsonConvert.SerializeObject(interactionResponse), Encoding.UTF8, "application/json"));
        }

        // /timestats
        public static async Task ShowStats(this SocketInteraction interaction)
        {
            var totalMembers = EventHandler._socketClient.Guilds.Sum(Guild => Guild.MemberCount);

            dynamic interactionResponse = new ExpandoObject();
            interactionResponse.type = 4;
            interactionResponse.data = new
            {
                embeds = new[]
                {
                    new
                    {
                        title = "Bot Info",
                        thumbnail_url = "https://cdn.discordapp.com/avatars/529569000028373002/b5100de6821ee1c4714ac022c3cd39d9.png?size=128",
                        color = 8365776,
                        fields = new[]
                        {
                            new
                            {
                                name = "Library",
                                value = "Discord.Net"
                            },
                            new
                            {
                                name = "Servers",
                                value = EventHandler._socketClient.Guilds.Count.ToString()
                            },
                            new
                            {
                                name = "Members",
                                value = totalMembers.ToString("#,##0")
                            },
                            new
                            {
                                name = "Developer",
                                value = "Reverse#006"
                            },
                            new
                            {
                                name = "Color",
                                value = "Suggested Role Color for Me: `#7fa6d0`"
                            },
                            new
                            {
                                name = "Links",
                                value = "[Invite](https://discord.com/api/oauth2/authorize?client_id=529569000028373002&permissions=2048&scope=bot%20applications.commands) | [Vote](\n\nhttps://top.gg/bot/529569000028373002/vote) | [GitHub](https://github.com/WilliamWelsh/TimeBot) | [Support Server](https://discord.gg/ga9V5pa)"
                            }
                        }
                    }
                }
            };

            await interaction.RespondImmediately(new StringContent(JsonConvert.SerializeObject(interactionResponse), Encoding.UTF8, "application/json"));
        }
    }
}