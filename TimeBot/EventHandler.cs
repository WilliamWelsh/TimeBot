using System;
using Discord;
using System.IO;
using Discord.Commands;
using System.Reflection;
using Discord.WebSocket;
using System.Threading.Tasks;
using DiscordBotsList.Api;
using Newtonsoft.Json;

namespace TimeBot
{
    public static class EventHandler
    {
        public static DiscordSocketClient _client;
        public static CommandService _service;

        public static async Task InitializeAsync(DiscordSocketClient client)
        {
            _client = client;
            _service = new CommandService();
            await _service.AddModulesAsync(Assembly.GetEntryAssembly(), null);

            _client.MessageReceived += HandleCommandAsync;

            _service.Log += Log;

            _client.Ready += OnReady;
        }

        private static async Task OnReady()
        {
            // Update server count on Top.GG
            var DblAPI = new AuthDiscordBotListApi(529569000028373002, File.ReadAllText("Resources/dblToken.txt"));
            var me = await DblAPI.GetMeAsync();
            await me.UpdateStatsAsync(_client.Guilds.Count);
        }

        private static Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        private static async Task HandleCommandAsync(SocketMessage s)
        {
            if (!(s is SocketUserMessage msg) || msg.Author.IsBot) return;

            var context = new SocketCommandContext(_client, msg);

            int argPos = 0;
            if (msg.HasStringPrefix("!", ref argPos))
                await _service.ExecuteAsync(context, argPos, null, MultiMatchHandling.Exception);

            //if (msg.Channel.Id == 624765261542719509)
            //    Console.WriteLine(msg.Content);
        }
    }
}
