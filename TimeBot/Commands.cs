using System;
using System.Globalization;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;

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

            await StatsHandler.DisplayStats(Context.Channel, name, avatarURL, id);
        }

        [Command("time")]
        public async Task DisplayStatsForRole(SocketRole role = null) => await StatsHandler.DisplayStats(Context, role);

        // Display everyone's current time
        [Command("timeall")]
        public async Task DisplayEveryonesTime() => await StatsHandler.DisplayEveryonesTime(Context);

        // Display everyone's country
        [Command("countryall")]
        public async Task DisplayEveryonesCountry() => await StatsHandler.DisplayEveryonesCountry(Context);

        // Set your time
        [Command("time set")]
        public async Task SetTime(double hourDifference) => await StatsHandler.SetTime(Context, hourDifference);

        // Set someone else's time
        [Command("time set")]
        public async Task SetSomeoneElsesTime(SocketUser user, double hourDifference) => await StatsHandler.SetSomeonesTime(Context, user, hourDifference);

        [Command("tstats")]
        public async Task ttt()
        {
            int total = 0;
            foreach (var Guild in Context.Client.Guilds)
                total += Guild.MemberCount;
            await Context.Channel.SendMessageAsync($"Servers: {Context.Client.Guilds.Count}\nMembers: {total}");
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
