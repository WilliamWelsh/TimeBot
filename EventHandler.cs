using System;
using Discord;
using Discord.Rest;
using Discord.Commands;
using System.Reflection;
using Discord.WebSocket;
using System.Threading.Tasks;
using TimeBot.Interactions;
using TimeBot.UserData;

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

            _socketClient.InteractionCreated += OnInteractionCreated;
        }

        private static async Task OnInteractionCreated(SocketInteraction arg)
        {
            switch (arg)
            {
                // Slash Commands
                case SocketSlashCommand slashCommand:
                    await SlashCommands.SearchCommands(slashCommand);
                    break;

                // User Commands (context menu)
                case SocketUserCommand userCommand:
                    var user = (SocketGuildUser) userCommand.Data.Member;
                    await userCommand.RespondAsync(embed: await StatsHandler.StatsEmbed(UserAccounts.GetAccount(user.Id), user.Nickname ?? user.Username, user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl()));
                    break;

                // Button
                case SocketMessageComponent buttonCommand:
                    if (buttonCommand.Data.CustomId == "refresh-country")
                        await buttonCommand.ShowCountryForAll();
                    break;

                default:
                    break;
            }
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