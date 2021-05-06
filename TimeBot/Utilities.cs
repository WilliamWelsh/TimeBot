﻿using System;
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