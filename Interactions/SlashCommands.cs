using System;
using System.Linq;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace TimeBot.Interactions
{
    public static class SlashCommands
    {
        // Search for a slash command
        public static async Task SearchCommands(SocketSlashCommand command)
        {
            switch (command.CommandName)
            {
                case "time":
                    if (command.Data.Options == null)
                        await command.ShowTime((SocketGuildUser)command.User);
                    else
                        await command.ShowTime(await EventHandler._restClient.GetGuildUserAsync(((SocketGuildUser)command.User).Guild.Id, ((SocketGuildUser)command.Data.Options.ElementAt(0).Value).Id));
                    break;

                case "country":
                    if (command.Data.Options == null)
                        await command.ShowCountry((SocketGuildUser)command.User);
                    else
                        await command.ShowCountry(await EventHandler._restClient.GetGuildUserAsync(((SocketGuildUser)command.User).Guild.Id, ((SocketGuildUser)command.Data.Options.ElementAt(0).Value).Id));
                    break;

                case "timeall":
                    await command.ShowTimeForAll();
                    break;

                case "countryall":
                    await command.ShowCountryForAll();
                    break;

                case "timehelp":
                    await command.ShowTimeHelp();
                    break;

                case "countryset":
                    await command.SetCountry();
                    break;

                case "timestats":
                    await command.ShowStats();
                    break;

                case "timesetup":
                    await command.TimeSetupForSelf();
                    break;

                case "set-user-country":
                    await command.SetUserCountry();
                    break;

                case "set-user-time":
                    await command.SetTimeForSomeoneElse();
                    break;

                default:
                    break;
            }
        }
    }
}