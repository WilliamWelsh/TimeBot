using System;
using Discord;
using System.Linq;
using Discord.Commands;
using System.Reflection;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace TimeBot
{
    class EventHandler
    {
        DiscordSocketClient _client;
        CommandService _service;

        public async Task InitializeAsync(DiscordSocketClient client)
        {
            _client = client;
            _service = new CommandService();
            await _service.AddModulesAsync(Assembly.GetEntryAssembly(), null);

            _client.MessageReceived += HandleCommandAsync;

            _service.Log += Log;
        }

        private Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        private async Task HandleCommandAsync(SocketMessage s)
        {
            SocketUserMessage msg = s as SocketUserMessage;
            if (msg == null || msg.Author.IsBot) return;

            var context = new SocketCommandContext(_client, msg);

            int argPos = 0;
            if (msg.HasStringPrefix("!", ref argPos))
                await _service.ExecuteAsync(context, argPos, null, MultiMatchHandling.Exception);

            string m = msg.Content.ToLower();

            if (s.Channel.Name.StartsWith("@"))
                Console.WriteLine($" ----------\n DIRECT MESSAGE\n From: {s.Channel}\n {s}\n ----------");
        }
    }
}