using System;
using Discord;
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
        public static CommandService _service;

        public static async Task InitializeAsync(DiscordSocketClient socketClient)
        {
            _socketClient = socketClient;

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

                // User Command (context menu)
                case SocketUserCommand userCommand:
                    var user = (SocketGuildUser)userCommand.Data.Member;
                    await userCommand.RespondAsync(embed: await StatsHandler.UserStatsEmbed(
                        UserAccounts.GetAccount(user.Id), user.Nickname ?? user.Username,
                        user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl()),
                        component: new ComponentBuilder().WithButton("Refresh", $"refresh_user-{userCommand.Data.Member.Id}", style: ButtonStyle.Secondary).Build());
                    break;

                // Button
                case SocketMessageComponent buttonCommand:
                    // /countryall "Refresh" button
                    if (buttonCommand.Data.CustomId.StartsWith("refresh_country"))
                        await buttonCommand.ShowCountryForAll();

                    // /time "Refresh" button
                    else if (buttonCommand.Data.CustomId.StartsWith("refresh_user"))
                        await buttonCommand.RefreshUserTime();

                    // /servertime "Refresh" button
                    else if (buttonCommand.Data.CustomId == "refresh_server")
                        await buttonCommand.RefreshServerTime();

                    // /servertime "Edit Time" button
                    else if (buttonCommand.Data.CustomId == "editservertime" ||
                        buttonCommand.Data.CustomId.StartsWith("server"))
                        await buttonCommand.EditServerTime();

                    // /set-user-time timezone selection
                    else if (buttonCommand.Data.CustomId.StartsWith("other"))
                        await buttonCommand.SetTimeForSomeoneElse();

                    // /timezone timezone selection
                    else if (buttonCommand.Data.CustomId.StartsWith("addtimezone"))
                        await buttonCommand.AddTimeZone();

                    // /timezone "Refresh" button
                    else if (buttonCommand.Data.CustomId.StartsWith("refresh_timezone"))
                        await buttonCommand.UpdateTimezones();

                    // /timezone "Add Timezone" button
                    else if (buttonCommand.Data.CustomId.StartsWith("addanothertimezone"))
                        await buttonCommand.AddTimeZone();

                    // /timezone "Edit Timezones" button
                    else if (buttonCommand.Data.CustomId.StartsWith("edittimezones"))
                        await buttonCommand.ShowEditMenuForTimeZonesCommand();

                    // roletime and the refresh button
                    else if (buttonCommand.Data.CustomId.StartsWith("refresh_tworole") ||
                        buttonCommand.Data.CustomId.StartsWith("refresh_role"))
                        await buttonCommand.ShowRoleTime();

                    else if (buttonCommand.Data.CustomId.StartsWith("refresh_twomembersrole") ||
                    buttonCommand.Data.CustomId.StartsWith("refresh_membersrole"))
                        await buttonCommand.ShowRoleMembers();

                    // /timesetup timezone selection
                    else
                        await buttonCommand.TimeSetupForSelf();
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