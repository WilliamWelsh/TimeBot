using System;
using Discord;
using System.IO;
using System.Linq;
using System.Net.Http;
using Discord.WebSocket;
using Color = Discord.Color;
using System.Threading.Tasks;
using SixLabors.ImageSharp.PixelFormats;

namespace TimeBot
{
    public static class Utilities
    {
        // Common Colors used

        public static readonly Color Blue = new Color(127, 166, 208);
        public static readonly Color Red = new Color(231, 76, 60);
        public static readonly Color Green = new Color(31, 139, 76);

        // Http Client
        private static HttpClient HttpClient = new HttpClient();

        /// <summary>
        /// Return an embed field
        /// </summary>
        public static EmbedFieldBuilder MakeEmbedField(string name, string value) => new EmbedFieldBuilder().WithName(name).WithValue(value).WithIsInline(false);

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
        /// Format a time into h:mm tt
        /// </summary>
        /// <param name="offset">The amount of hours to add to the current time.</param>
        /// <returns></returns>
        public static string GetTime(double offset) => DateTime.Now.AddHours(offset).ToString("h:mm tt");

        /// <summary>
        /// Get the emoji for a country
        /// </summary>
        public static string GetCountryFlag(string country)
        {
            switch (country)
            {
                case "Australia":
                    return "🇦🇺";

                case "Canada":
                    return "🇨🇦";

                case "China":
                    return "🇨🇳";

                case "Djibouti":
                    return "🇩🇯";

                case "Latvia":
                    return "🇱🇻";

                case "Germany":
                    return "🇩🇪";

                case "France":
                    return "🇫🇷";

                case "Poland":
                    return "🇵🇱";

                case "Mexico":
                    return "🇲🇽";

                case "Turkey":
                    return "🇹🇷";

                case "Uruguay":
                    return "🇺🇾";

                case "Philippines":
                    return "🇵🇭";

                case "Denmark":
                    return "🇩🇰";

                case "Netherlands":
                    return "🇳🇱";

                case "Scotland":
                    return "🏴󠁧󠁢󠁳󠁣󠁴󠁿";

                case "Sweden":
                    return "🇸🇪";

                case "United Kingdom":
                    return "🇬🇧";

                case "United States":
                    return "🇺🇸";

                case "Bangladesh":
                    return "🇧🇩";

                case "Ethiopia":
                    return "🇪🇹";

                case "India":
                    return "🇮🇳";

                case "Indonesia":
                    return "🇮🇩";

                case "Lebanon":
                    return "🇱🇧";

                case "Morocco":
                    return "🇲🇦";

                case "Norway":
                    return "🇳🇴";

                case "Pakistan":
                    return "🇵🇰";

                case "Ukraine":
                    return "🇺🇦";

                case "Singapore":
                    return "🇸🇬";

                case "Ireland":
                    return "🇮🇪";

                case "Dominica":
                    return "🇩🇲";

                default:
                    return "";
            }
        }
    }
}