using System;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace TimeBot
{
    public static class Utilities
    {
        // Colors
        public readonly static Color Blue = new Color(127, 166, 208);
        public readonly static Color Red = new Color(231, 76, 60);
        public readonly static Color Green = new Color(31, 139, 76);

        // Print an Error
        public static async Task PrintError(ISocketMessageChannel channel, string message) => await PrintEmbed(channel, "Error", message, Red).ConfigureAwait(false);

        // Print a Success message
        public static async Task PrintSuccess(ISocketMessageChannel channel, string message) => await PrintEmbed(channel, "Success", message, Green).ConfigureAwait(false);

        // Print a basic embed, like an error message or success message
        public static async Task PrintEmbed(ISocketMessageChannel channel, string title, string message, Color color) => await channel.SendMessageAsync("", false, new EmbedBuilder()
                .WithTitle(title)
                .WithColor(color)
                .WithDescription(message)
                .Build());

        // Get the current time... somewhere
        public static string GetTime(double offset) => DateTime.Now.AddHours(offset).ToString("h:mm tt");
    }
}
