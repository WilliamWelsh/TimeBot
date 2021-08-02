using System;
using Discord;
using System.IO;
using Discord.Rest;
using Discord.Commands;
using System.Reflection;
using Discord.WebSocket;
using DiscordBotsList.Api;
using System.Threading.Tasks;
using TimeBot.Interactions;

namespace TimeBot
{
    public static class EventHandler
    {
        public static DiscordSocketClient _socketClient;
        public static DiscordRestClient _restClient;
        public static CommandService _service;

        public static async Task InitializeAsync(DiscordSocketClient socketClient, DiscordRestClient restClient)
        {
            _socketClient = socketClient;
            _restClient = restClient;

            _service = new CommandService();
            await _service.AddModulesAsync(Assembly.GetEntryAssembly(), null);

            _socketClient.MessageReceived += HandleCommandAsync;

            _service.Log += Log;

            _socketClient.Ready += OnReady;

            _socketClient.InteractionCreated += OnInteractionCreated;
        }

        private static async Task OnInteractionCreated(SocketInteraction interaction)
        {
            try
            {
                await SlashCommands.SearchCommands(interaction);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static async Task OnReady()
        {
            var DblAPI = new AuthDiscordBotListApi(529569000028373002, File.ReadAllText("Resources/dblToken.txt"));
            var me = await DblAPI.GetMeAsync();
            await me.UpdateStatsAsync(_socketClient.Guilds.Count);
        }

        private static Task Log(LogMessage arg)
        {
            Console.WriteLine(arg);
            return Task.CompletedTask;
        }

        private static async Task HandleCommandAsync(SocketMessage s)
        {
            if (!(s is SocketUserMessage msg) || msg.Author.IsBot) return;

            var context = new SocketCommandContext(_socketClient, msg);

            // Uncommented while testing (my private test server)
            //if (context.Guild.Id != 735263201612005472)
            //    return;

            int argPos = 0;
            if (msg.HasStringPrefix("!", ref argPos))
                await _service.ExecuteAsync(context, argPos, null);
        }
    }
}