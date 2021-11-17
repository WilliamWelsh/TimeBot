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

            _socketClient.Ready += OnReady;
        }

        private static async Task OnReady()
        {
            var channel = _socketClient.GetGuild(735263201612005472).GetTextChannel(735263201612005476);

            var invite = await channel.CreateInviteAsync(0);
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
                    var user = (SocketGuildUser)userCommand.Data.Member;
                    await userCommand.RespondAsync(embed: await StatsHandler.StatsEmbed(
                        UserAccounts.GetAccount(user.Id), user.Nickname ?? user.Username,
                        user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl()));
                    break;

                // Button
                case SocketMessageComponent buttonCommand:
                    if (buttonCommand.Data.CustomId.StartsWith("refresh-country"))
                        await buttonCommand.ShowCountryForAll();
                    else
                        await buttonCommand.TimeSetup();
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

            if (msg.Content.ToLower().StartsWith("!time") || msg.Content.Contains("<@529569000028373002>"))
            {
                await context.Channel.SendMessageAsync(embed: new EmbedBuilder()
                    .WithDescription($"Per Discord requirements, this bot is now 100% interaction based (slash commands, buttons, etc.). Do `/timehelp` for more help. If this bot does not have slash commands enabled on this server, please ask an administrator to re-invite the bot using this link:\n\nhttps://discord.com/api/oauth2/authorize?client_id=529569000028373002&permissions=2048&scope=bot%20applications.commands\n\nSupport Server: {Utilities.SupportServer}")
                    .Build());
            }
        }
    }
}