using System;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace TimeBot
{
    class Program
    {
        public static DiscordSocketClient _client;
        EventHandler _handler;

        static void Main(string[] args) => new Program().StartAsync().GetAwaiter().GetResult();

        public async Task StartAsync()
        {
            if (string.IsNullOrEmpty(Config.Bot.Token))
            {
                Console.WriteLine("No bot token found, please view the README and verify there is a bot token in bin/Debug/Resources/config.json");
                Console.ReadLine();
                return;
            }

            _client = new DiscordSocketClient(new DiscordSocketConfig { LogLevel = LogSeverity.Verbose });
            _client.Log += Log;
            await _client.LoginAsync(TokenType.Bot, Config.Bot.Token);
            await _client.StartAsync();
            _handler = new EventHandler();
            await _handler.InitializeAsync(_client);
            await _client.SetGameAsync("!timehelp");
            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.Message);
            return Task.CompletedTask;
        }
    }
}