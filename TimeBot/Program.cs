using System;
using Discord;
using System.IO;
using Discord.Rest;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace TimeBot
{
    internal class Program
    {
        private static void Main(string[] args) => new Program().StartAsync().GetAwaiter().GetResult();

        public async Task StartAsync()
        {
            if (string.IsNullOrEmpty(File.ReadAllText("Resources/botToken.txt")))
            {
                Console.WriteLine("No bot token found, please view the README and verify there is a bot token in bin/Debug/Resources/botToken.txt");
                Console.ReadLine();
                return;
            }

            var _restClient = new DiscordRestClient(new DiscordRestConfig());
            await _restClient.LoginAsync(TokenType.Bot, File.ReadAllText("Resources/botToken.txt"));

            var _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                GatewayIntents = GatewayIntents.GuildMessages | GatewayIntents.Guilds | GatewayIntents.GuildMembers
            });
            _client.Log += Log;

            await _client.LoginAsync(TokenType.Bot, File.ReadAllText("Resources/botToken.txt"));
            await _client.StartAsync();

            // Set up the event handler
            await EventHandler.InitializeAsync(_client, _restClient);
            await _client.SetGameAsync("!timehelp");

            // Set up the list of valid countries
            StatsHandler.SetupCountryList();

            await Task.Delay(-1).ConfigureAwait(false);
        }

        // Log messages to the console
        private static Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.Message);
            return Task.CompletedTask;
        }
    }
}