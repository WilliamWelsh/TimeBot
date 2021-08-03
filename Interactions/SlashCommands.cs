using System;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace TimeBot.Interactions
{
    // I know this is bad, but at the time of writing this,
    // Discord.NET doesn't formally support slash commands or interactions in general
    // So this is my workaround until they officially support it
    public static class SlashCommands
    {
        // Search for a slash command
        public static async Task SearchCommands(SocketInteraction interaction)
        {
            switch (interaction.Data.Name)
            {
                case "time":
                    if (interaction.Data.Options == null)
                        await interaction.ShowTime(interaction.User);
                    else
                    {
                        ulong id;

                        if (ulong.TryParse(interaction.Data.Options.ElementAt(0).Value.ToString(), NumberStyles.None, CultureInfo.InvariantCulture, out id))
                            await interaction.ShowTime(await EventHandler._restClient.GetGuildUserAsync(interaction.Guild.Id, id));
                    }
                    break;

                case "country":
                    if (interaction.Data.Options == null)
                        await interaction.ShowCountry(interaction.User);
                    else
                    {
                        ulong id;

                        if (ulong.TryParse(interaction.Data.Options.ElementAt(0).Value.ToString(), NumberStyles.None, CultureInfo.InvariantCulture, out id))
                            await interaction.ShowCountry(await EventHandler._restClient.GetGuildUserAsync(interaction.Guild.Id, id));
                    }
                    break;

                case "timeall":
                    await interaction.ShowTimeForAll();
                    break;

                case "countryall":
                    await interaction.ShowCountryForAll();
                    break;

                case "timehelp":
                    await interaction.ShowTimeHelp();
                    break;

                case "timeset":
                    await interaction.SetTime();
                    break;

                case "countryset":
                    await interaction.SetCountry();
                    break;

                case "timestats":
                    await interaction.ShowStats();
                    break;

                default:
                    break;
            }
        }

        // Follow up with some content
        public static async Task RespondImmediately(this SocketInteraction interaction, StringContent content)
        {
            var response = await Utilities.HttpClient.PostAsync($"https://discord.com/api/v8/interactions/{interaction.Id}/{interaction.Token}/callback", content);

            var responseString = await response.Content.ReadAsStringAsync();

            Console.WriteLine(responseString);
        }

        // Respond with an error immediately
        public static async Task ImmediateError(this SocketInteraction interaction, string description)
        {
            dynamic interactionResponse = new ExpandoObject();
            interactionResponse.type = 4;
            interactionResponse.data = new
            {
                embeds = new[]
                {
                    new
                    {
                        title = "Error",
                        description,
                        color = "15158332"
                    }
                }
            };

            await interaction.RespondImmediately(new StringContent(JsonConvert.SerializeObject(interactionResponse), Encoding.UTF8, "application/json"));
        }
    }
}