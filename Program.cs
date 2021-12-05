using System;
using Discord;
using System.IO;
using Discord.WebSocket;
using System.Threading.Tasks;
using DiscordBotsList.Api;
using DiscordBotsList.Api.Objects;

namespace TimeBot
{
    internal class Program
    {
        private IDblSelfBot _dblApi;

        private DiscordSocketClient _client;

        private static void Main(string[] args) => new Program().StartAsync().GetAwaiter().GetResult();

        public async Task StartAsync()
        {
            if (string.IsNullOrEmpty(File.ReadAllText("Resources/botToken.txt")))
            {
                Console.WriteLine("No bot token found, please view the README and verify there is a bot token in bin/Debug/Resources/botToken.txt");
                Console.ReadLine();
                return;
            }

            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                GatewayIntents = GatewayIntents.GuildMessages | GatewayIntents.Guilds | GatewayIntents.GuildMembers
            });
            _client.Log += Log;

            await _client.LoginAsync(TokenType.Bot, File.ReadAllText("Resources/botToken.txt"));
            await _client.StartAsync();

            // Crate Discord Bot List client (Top.gg)
            var discordBotList = new AuthDiscordBotListApi(529569000028373002, File.ReadAllText("Resources/dblToken.txt"));
            _dblApi = await discordBotList.GetMeAsync();

            // These events will update the current amount of guilds the bot is in on Top.gg (_dblApi)
            _client.Ready += OnReady;
            _client.JoinedGuild += OnGuildJoined;
            _client.LeftGuild += OnGuildLeft;

            // Set up the event handler
            await EventHandler.InitializeAsync(_client);
            await _client.SetGameAsync("/timehelp");

            await Task.Delay(-1).ConfigureAwait(false);
        }

        // Update the server count
        private async Task UpdateServerCount()
        {
            // Update on the bot's status
            await _client.SetGameAsync($"/timehelp | {_client.Guilds.Count} servers");

            // Update on top.gg
            await _dblApi.UpdateStatsAsync(_client.Guilds.Count);
        }

        private async Task OnReady() => await UpdateServerCount();

        private async Task OnGuildLeft(SocketGuild arg) => await UpdateServerCount();

        private async Task OnGuildJoined(SocketGuild arg) => await UpdateServerCount();

        // Log messages to the console
        private static Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.Message);
            return Task.CompletedTask;
        }
    }
}