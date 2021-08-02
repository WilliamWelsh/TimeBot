using System;
using System.Globalization;
using System.Linq;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using Discord;

namespace TimeBot
{
    [RequireContext(ContextType.Guild)]
    public class Commands : ModuleBase<SocketCommandContext>
    {
        // Tutorial on how to set time and country
        [Command("timesetup")]
        public async Task DisplayTimeSetup() => await StatsHandler.DisplayTimeSetup(Context.Channel);

        // Tutorial on how to set time and country
        [Command("timeinvite")]
        public async Task DMInvite() => await StatsHandler.DMInviteLink(Context.Channel, Context.User);

        // Display time (and possible country) for a user
        [Command("time")]
        public async Task DisplayStatsForUser([Remainder] string input = null)
        {
            var name = "";
            var avatarURL = "";
            ulong id;

            // No input = the user
            if (input == null)
            {
                name = ((SocketGuildUser)Context.User).Nickname ?? Context.User.Username;
                avatarURL = Context.User.GetAvatarUrl() ?? Context.User.GetDefaultAvatarUrl();
                id = Context.User.Id;
            }

            // By mention
            else if (input.StartsWith("<@!") || input.StartsWith("<@"))
            {
                id = Convert.ToUInt64(input.Replace("<@!", "").Replace(">", "").Replace("<@", ""));

                var restUser = await EventHandler._restClient.GetGuildUserAsync(Context.Guild.Id, id);
                name = restUser.Nickname ?? restUser.Username;
                avatarURL = restUser.GetAvatarUrl() ?? restUser.GetDefaultAvatarUrl();
            }

            // By ID
            else if (ulong.TryParse(input, NumberStyles.None, CultureInfo.InvariantCulture, out id))
            {
                var restUser = await EventHandler._restClient.GetGuildUserAsync(Context.Guild.Id, id);
                name = restUser.Nickname ?? restUser.Username;
                avatarURL = restUser.GetAvatarUrl() ?? restUser.GetDefaultAvatarUrl();
            }

            // By username
            else
            {
                var Users = (await EventHandler._restClient.GetGuildAsync(Context.Guild.Id)).GetUsersAsync();
                await foreach (var List in Users)
                {
                    var targetUser = List.Where(x => string.Equals(input, x.Username, StringComparison.OrdinalIgnoreCase));
                    if (targetUser.Count() == 1)
                    {
                        var restUser = await EventHandler._restClient.GetGuildUserAsync(Context.Guild.Id, targetUser.ElementAt(0).Id);
                        name = restUser.Nickname ?? restUser.Username;
                        avatarURL = restUser.GetAvatarUrl() ?? restUser.GetDefaultAvatarUrl();
                        id = restUser.Id;
                    }
                }
            }

            await StatsHandler.DisplayStats(Context.Channel, name, avatarURL, id);

            await Context.Channel.SendMessageAsync(null, false, new EmbedBuilder()
                .WithColor(Utilities.Blue)
                .WithImageUrl("https://cdn.discordapp.com/attachments/735282082963652749/871855263962005524/unknown.png")
                .WithDescription("!! ATTENTION !!\n\nThe bot is moving to SLASH commands. Please re-invite the bot using the link below, and then use SLASH commands. For example:\n/time @user\n\nIf you need any help join the support server: https://discord.gg/ga9V5pa\n\nRE-INVITE THE BOT USING THIS LINK: https://discord.com/oauth2/authorize?client_id=529569000028373002&permissions=0&scope=bot%20applications.commands\n\nYou must start using SLASH commands, the `!` commands will be removed soon.")
                .Build());
        }

        [Command("time")]
        public async Task DisplayStatsForRole(SocketRole role = null) => await StatsHandler.DisplayStats(Context, role);

        // Display everyone's current time
        [Command("timeall")]
        public async Task DisplayEveryonesTime() => await StatsHandler.DisplayEveryonesTime(Context);

        // Display everyone's country
        [Command("countryall")]
        public async Task DisplayEveryonesCountry() => await StatsHandler.DisplayEveryonesTimeByCountry(Context);

        // Set your time
        [Command("time set")]
        public async Task SetTime(double hourDifference) => await StatsHandler.SetTime(Context, hourDifference);

        // Set someone else's time
        [Command("time set")]
        public async Task SetSomeoneElsesTime(SocketUser user, double hourDifference) => await StatsHandler.SetSomeonesTime(Context, user, hourDifference);

        [Command("tstats")]
        public async Task Stats()
        {
            var totalMembers = Context.Client.Guilds.Sum(Guild => Guild.MemberCount);

            await Context.Channel.SendMessageAsync(null, false, new EmbedBuilder()
                .WithTitle("Bot Info")
                .WithColor(Utilities.Blue)
                .WithThumbnailUrl("https://cdn.discordapp.com/avatars/529569000028373002/b5100de6821ee1c4714ac022c3cd39d9.png?size=128")
                .AddField("Library", "Discord.Net")
                .AddField("Servers", Context.Client.Guilds.Count)
                .AddField("Members", totalMembers.ToString("#,##0"))
                .AddField("Developer", "Reverse#0069")
                .AddField("Color", "Suggested Role Color for Me: `#7fa6d0`")
                .AddField("Links", "[Invite](https://discord.com/oauth2/authorize?client_id=529569000028373002&permissions=68608&scope=bot) | [Vote](\n\nhttps://top.gg/bot/529569000028373002/vote) | [GitHub](https://github.com/WilliamWelsh/TimeBot) | [Support Server](https://discord.gg/ga9V5pa)")
                .Build()).ConfigureAwait(false);
        }

        // Set your country
        [Command("country set")]
        public async Task SetCountry([Remainder] string country) => await StatsHandler.SetCountry(Context.Channel, Context.User, country);

        // Help menu
        [Command("timehelp")]
        public async Task DisplayHelp() => await StatsHandler.DisplayHelp(Context.Channel);

        // TODO: Finish this
        [Command("timestats")]
        public async Task TimeStats() => await StatsHandler.DisplayUserStats(Context);
    }
}