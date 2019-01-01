﻿using System;
using Discord;
using System.Text;
using System.Linq;
using TimeBot.UserData;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TimeBot
{
    [RequireContext(ContextType.Guild)]
    public class Commands : ModuleBase<SocketCommandContext>
    {
        // Tutorial on how to set time and country
        [Command("timesetup")]
        public async Task DisplayTimeSetup() => await Config.StatsHandler.DisplayTimeSetup(Context.Channel);

        // Display time (and possible country) for a user
        [Command("time")]
        public async Task DisplayStatsForUser(SocketGuildUser user = null) => await Config.StatsHandler.DisplayStats(Context.Channel, user ?? (SocketGuildUser)Context.User);

        // Set your time
        [Command("time set")]
        public async Task SetTime(int hourDifference) => await Config.StatsHandler.SetTime(Context.Channel, Context.User, hourDifference);

        // Set your country
        [Command("country set")]
        public async Task SetCountry([Remainder]string country) => await Config.StatsHandler.SetCountry(Context.Channel, Context.User, country);

        // Help menu
        [Command("timehelp")]
        public async Task DisplayHelp() => await Config.StatsHandler.DisplayHelp(Context.Channel);
    }
}
