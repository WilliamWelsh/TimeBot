using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using Color = Discord.Color;
using System.Threading.Tasks;
using SixLabors.ImageSharp.PixelFormats;

namespace TimeBot
{
    public static class Utilities
    {
        // Common Colors used

        public static readonly Color Blue = new(127, 166, 208);
        public static readonly Color Red = new(231, 76, 60);
        public static readonly Color Green = new(31, 139, 76);

        /// <summary>
        /// Invite link to the support server
        /// </summary>
        public static string SupportServer = "https://discord.gg/FjgCFYRZAw";

        // Http Client
        public static HttpClient HttpClient = new();

        /// <summary>
        /// Get the average color for a user's profile picture
        /// </summary>
        public static async Task<Color> GetUserColor(string avatarURL)
        {
            using (var ms = new MemoryStream(await HttpClient.GetByteArrayAsync(avatarURL)))
            {
                var palette = DominantColorFinder.GetPalette((SixLabors.ImageSharp.Image<Rgba32>)SixLabors.ImageSharp.Image.Load(ms));
                return new Color(Convert.ToInt16(palette.Average(a => a.Color.R)), Convert.ToInt16(palette.Average(a => a.Color.G)), Convert.ToInt16(palette.Average(a => a.Color.B)));
            }
        }

        /// <summary>
        /// Return the "a minute ago" discord text
        /// </summary>
        public static string GetRefreshedTimeText() => $"Last refreshed <t:{((int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds)}:R>";
    }
}