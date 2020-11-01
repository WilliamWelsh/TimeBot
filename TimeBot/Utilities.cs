using System;
using System.Drawing;
using System.IO;
using System.Net;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;
using ColorThiefDotNet;
using Color = Discord.Color;

namespace TimeBot
{
    public static class Utilities
    {
        // Common Colors used

        public static readonly Color Blue = new Color(127, 166, 208);
        public static readonly Color Red = new Color(231, 76, 60);
        public static readonly Color Green = new Color(31, 139, 76);

        /// <summary>
        /// ColorThief (used to extract dominant color from images)
        /// </summary>
        public static readonly ColorThief ColorThief = new ColorThief();

        /// <summary>
        /// Print a red error message.
        /// </summary>
        public static async Task PrintError(this ISocketMessageChannel channel, string message) => await channel.PrintEmbed("Error", message, Red).ConfigureAwait(false);

        /// <summary>
        /// Print a green success message.
        /// </summary>
        public static async Task PrintSuccess(this ISocketMessageChannel channel, string message) => await channel.PrintEmbed("Success", message, Green).ConfigureAwait(false);

        /// <summary>
        /// Print a basic embed.
        /// </summary>
        public static async Task PrintEmbed(this ISocketMessageChannel channel, string title, string message, Color color) => await channel.SendMessageAsync("", false, new EmbedBuilder()
                .WithTitle(title)
                .WithColor(color)
                .WithDescription(message)
                .Build());

        public static Color GetUserColor(string avatarURL)
        {
            // Download the avatar
            using (var client = new WebClient())
            using (var ms = new MemoryStream(client.DownloadData(avatarURL)))
            using (var avatar = new Bitmap(System.Drawing.Image.FromStream(ms)))
            {
                // Get the color and convert it to a Discord color
                var color = ColorThief.GetColor(avatar, 10, false).Color; // TODO check for white
                return new Color(color.R, color.G, color.B);
            }
        }

        /// <summary>
        /// Format a time into h:mm tt
        /// </summary>
        /// <param name="offset">The amount of hours to add to the current time.</param>
        /// <returns></returns>
        public static string GetTime(double offset) => DateTime.Now.AddHours(offset).ToString("h:mm tt");
    }
}
