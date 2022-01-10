using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Discord;

namespace TimeBot
{
    public static class TimeZones
    {
        /// <summary>
        /// List of TimeZones
        /// </summary>
        public static List<string> List = new()
        {
            "Dateline Standard Time",
            "UTC-11",
            "Aleutian Standard Time",
            "Hawaiian Standard Time",
            "Marquesas Standard Time",
            "Alaskan Standard Time",
            "UTC-09",
            "Pacific Standard Time (Mexico)",
            "UTC-08",
            "Pacific Standard Time",
            "US Mountain Standard Time",
            "Mountain Standard Time (Mexico)",
            "Mountain Standard Time",
            "Yukon Standard Time",
            "Central America Standard Time",
            "Central Standard Time",
            "Easter Island Standard Time",
            "Central Standard Time (Mexico)",
            "Canada Central Standard Time",
            "SA Pacific Standard Time",
            "Eastern Standard Time (Mexico)",
            "Eastern Standard Time",
            "Haiti Standard Time",
            "Cuba Standard Time",
            "US Eastern Standard Time",
            "Turks And Caicos Standard Time",
            "Paraguay Standard Time",
            "Atlantic Standard Time",
            "Venezuela Standard Time",
            "Central Brazilian Standard Time",
            "SA Western Standard Time",
            "Pacific SA Standard Time",
            "Newfoundland Standard Time",
            "Tocantins Standard Time",
            "E. South America Standard Time",
            "SA Eastern Standard Time",
            "Argentina Standard Time",
            "Greenland Standard Time",
            "Montevideo Standard Time",
            "Magallanes Standard Time",
            "Saint Pierre Standard Time",
            "Bahia Standard Time",
            "UTC-02",
            "Mid-Atlantic Standard Time",
            "Azores Standard Time",
            "Cape Verde Standard Time",
            "UTC",
            "GMT Standard Time",
            "Greenwich Standard Time",
            "Sao Tome Standard Time",
            "Morocco Standard Time",
            "W. Europe Standard Time",
            "Central Europe Standard Time",
            "Romance Standard Time",
            "Central European Standard Time",
            "W. Central Africa Standard Time",
            "Jordan Standard Time",
            "GTB Standard Time",
            "Middle East Standard Time",
            "Egypt Standard Time",
            "E. Europe Standard Time",
            "Syria Standard Time",
            "West Bank Standard Time",
            "South Africa Standard Time",
            "FLE Standard Time",
            "Israel Standard Time",
            "Kaliningrad Standard Time",
            "Sudan Standard Time",
            "Libya Standard Time",
            "Namibia Standard Time",
            "Arabic Standard Time",
            "Turkey Standard Time",
            "Arab Standard Time",
            "Belarus Standard Time",
            "Russian Standard Time",
            "E. Africa Standard Time",
            "Volgograd Standard Time",
            "Iran Standard Time",
            "Arabian Standard Time",
            "Astrakhan Standard Time",
            "Azerbaijan Standard Time",
            "Russia Time Zone 3",
            "Mauritius Standard Time",
            "Saratov Standard Time",
            "Georgian Standard Time",
            "Caucasus Standard Time",
            "Afghanistan Standard Time",
            "West Asia Standard Time",
            "Ekaterinburg Standard Time",
            "Pakistan Standard Time",
            "Qyzylorda Standard Time",
            "India Standard Time",
            "Sri Lanka Standard Time",
            "Nepal Standard Time",
            "Central Asia Standard Time",
            "Bangladesh Standard Time",
            "Omsk Standard Time",
            "Myanmar Standard Time",
            "SE Asia Standard Time",
            "Altai Standard Time",
            "W. Mongolia Standard Time",
            "North Asia Standard Time",
            "N. Central Asia Standard Time",
            "Tomsk Standard Time",
            "China Standard Time",
            "North Asia East Standard Time",
            "Singapore Standard Time",
            "W. Australia Standard Time",
            "Taipei Standard Time",
            "Ulaanbaatar Standard Time",
            "Aus Central W. Standard Time",
            "Transbaikal Standard Time",
            "Tokyo Standard Time",
            "North Korea Standard Time",
            "Korea Standard Time",
            "Yakutsk Standard Time",
            "Cen. Australia Standard Time",
            "AUS Central Standard Time",
            "E. Australia Standard Time",
            "AUS Eastern Standard Time",
            "West Pacific Standard Time",
            "Tasmania Standard Time",
            "Vladivostok Standard Time",
            "Lord Howe Standard Time",
            "Bougainville Standard Time",
            "Russia Time Zone 10",
            "Magadan Standard Time",
            "Norfolk Standard Time",
            "Sakhalin Standard Time",
            "Central Pacific Standard Time",
            "Russia Time Zone 11",
            "New Zealand Standard Time",
            "UTC+12",
            "Fiji Standard Time",
            "Kamchatka Standard Time",
            "Chatham Islands Standard Time",
            "UTC+13",
            "Tonga Standard Time",
            "Samoa Standard Time",
            "Line Islands Standard Time"
        };

        /// <summary>
        /// Convert a TimeZone Id into hour:minute am/pm
        /// </summary>
        public static string GetTimeByTimeZone(string Id) => Id == "Not set." ? Id : TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, Id).ToString("h:mm tt");

        /// <summary>
        /// Convert a TimeZone Id into a DAteTime
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public static DateTime GetRawTimeByTimeZone(string Id) => Id == "Not set." ? new DateTime(1970, 1, 1) : TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, Id);

        /// <summary>
        /// Get buttons of TimeZones
        /// </summary>
        public static MessageComponent GetPaginatedTimeZones(int page, string data, string prefix = "")
        {
            var zones = List.GetRange(page * 20, 20);

            var row = 0;
            var component = new ComponentBuilder();

            var counter = 1;
            for (int i = 0; i < zones.Count; i++)
            {
                if (counter % 5 == 0)
                {
                    row++;
                    counter = 1;
                }

                var zone = TimeZoneInfo.FindSystemTimeZoneById(zones.ElementAt(i));
                component.WithButton(
                    new ButtonBuilder($"{GetTimeByTimeZone(zone.Id)} {zone.Id}", $"{prefix}set_{data}_{zone.Id}", ButtonStyle.Secondary), row);
            }

            component.WithButton(new ButtonBuilder("Previous Page", $"{prefix}lastpage_{page}_{data}", disabled: page == 0),
                row + 1);
            component.WithButton(new ButtonBuilder("Next Page", $"{prefix}nextpage_{page}_{data}", disabled: page == 6), row + 1);

            return component.Build();
        }

        /// <summary>
        /// Get a list of timezones and their current time for /timezone
        /// </summary>
        public static string GetTimeZoneTimes(string data)
        {
            // This is a list of numbers, each number
            // is an index of a timezone in the List
            var timezoneIndices = data.Split(",");

            var timezones = new List<TimeZone>();

            foreach (var zone in timezoneIndices)
                timezones.Add(new TimeZone(TimeZoneInfo.FindSystemTimeZoneById(List.ElementAt(Convert.ToInt32(zone))).Id));

            // Sort the list
            timezones = timezones.OrderBy(x => x.RawTime).Reverse().ToList();

            var result = new StringBuilder();

            foreach (var zone in timezones)
                result.AppendLine($"**{zone.TimeZoneId}** {zone.RawTime.ToString("h:mm tt dddd, MMMM d")}");

            return result.ToString();
        }
    }

    public class TimeZone
    {
        public string TimeZoneId { get; set; }
        public DateTime RawTime { get; set; }

        public TimeZone(string timezoneId)
        {
            TimeZoneId = timezoneId;
            RawTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, TimeZoneId);
        }
    }
}
