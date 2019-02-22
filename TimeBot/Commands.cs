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
        public async Task DisplayTimeSetup() => await Config.StatsHandler.DisplayTimeSetup(Context.Channel);

        // Tutorial on how to set time and country
        [Command("timeinvite")]
        public async Task DMInvite() => await Config.StatsHandler.DMInviteLink(Context.Channel, Context.User);

        // Display time (and possible country) for a user
        [Command("time")]
        public async Task DisplayStatsForUser(SocketGuildUser user = null) => await Config.StatsHandler.DisplayStats(Context.Channel, user ?? (SocketGuildUser)Context.User);

        // Display time (and possible country) for a user
        [Command("time")]
        public async Task DisplayStatsForRole(SocketRole role = null) => await Config.StatsHandler.DisplayStats(Context, role);

        // Display everyone's current time
        [Command("timeall")]
        public async Task DisplayEveryonesTime() => await Config.StatsHandler.DisplayEveryonesTime(Context);

        // Display everyone's country
        [Command("countryall")]
        public async Task DisplayEveryonesCountry() => await Config.StatsHandler.DisplayEveryonesCountry(Context);

        // Set your time
        [Command("time set")]
        public async Task SetTime(double hourDifference) => await Config.StatsHandler.SetTime(Context.Channel, Context.User, hourDifference);

        // Set your country
        [Command("country set")]
        public async Task SetCountry([Remainder]string country) => await Config.StatsHandler.SetCountry(Context.Channel, Context.User, country);

        // Help menu
        [Command("timehelp")]
        public async Task DisplayHelp() => await Config.StatsHandler.DisplayHelp(Context.Channel);

        // TODO: Finish this
        [Command("timestats")]
        public async Task TimeStats() => await Config.StatsHandler.DisplayUserStats(Context);

        // View the server time
        [Command("time server")]
        public async Task TimeServer()
        {
            var server = UserData.Servers.GetServer(Context.Guild);
        }

        // Set the server time
        [Command("time set server")]
        public async Task TryToSetServerTime() => await Config.StatsHandler.TryToSetUpServerTime(Context);
    }
}
