using System;
using Discord;
using System.Timers;
using TimeBot.UserData;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace TimeBot
{
    /// <summary>
    /// A handler that updates channel names with the time of a user
    /// </summary>
    public class UpdatingClock
    {
        // The client to get the server and channel data from
        private DiscordSocketClient client;

        // The channel to update (the name)
        private ITextChannel channel;

        // 60 second timer
        private Timer timer;

        // Whose time will be display?
        private SocketGuildUser user;

        // Set up the timer and get a reference to the channel and user we need
        public async Task Initialize(DiscordSocketClient _client, ulong serverID, ulong channelID, ulong userID)
        {
            client = _client;

            channel = client.GetGuild(serverID).GetTextChannel(channelID) as ITextChannel;

            timer = new Timer();
            timer.Interval = 600000;
            timer.Elapsed += OnTimerElapsed;
            timer.Enabled = true;
            timer.Start();

            user = client.GetGuild(serverID).GetUser(userID);

            await UpdateChannel();
        }

        // Update the channel name
        private async Task UpdateChannel() => await channel.ModifyAsync(c => { c.Name = $"{user.Nickname ?? user.Username}-{DateTime.Now.AddHours(UserAccounts.GetAccount(user).localTime):hmmtt}"; });

        // Update the channel name on timer elapse event
        private async void OnTimerElapsed(object sender, ElapsedEventArgs e) => await UpdateChannel();
    }
}
