using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using System.Dynamic;
using System.IO;
using System.Threading.Tasks;
using TimeBot.UserData;

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
        public async Task DisplayStatsForUser(SocketGuildUser user = null) => await StatsHandler.DisplayStats(Context.Channel, user ?? (SocketGuildUser)Context.User);

        // Display time (and possible country) for a user
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
        public async Task SetTime(double hourDifference) => await StatsHandler.SetTime(Context.Channel, Context.User, hourDifference);

        [Command("t")]
        public async Task ttt()
        {
            int total = 0;
            foreach (var Guild in Context.Client.Guilds)
                total += Guild.MemberCount;
            await Context.Channel.SendMessageAsync($"Servers: {Context.Client.Guilds.Count}\nMembers: {total}");
        }

        // Set your country
        [Command("country set")]
        public async Task SetCountry([Remainder]string country) => await StatsHandler.SetCountry(Context.Channel, Context.User, country);

        // Help menu
        [Command("timehelp")]
        public async Task DisplayHelp() => await StatsHandler.DisplayHelp(Context.Channel);

        // TODO: Finish this
        [Command("timestats")]
        public async Task TimeStats() => await StatsHandler.DisplayUserStats(Context);
    }
}
