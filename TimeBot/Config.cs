using System.IO;
using Newtonsoft.Json;

namespace TimeBot
{
    class Config
    {
        public struct BotConfig { public string Token { get; set; } }

        public static BotConfig Bot;

        public static StatsHandler StatsHandler = new StatsHandler();

        static Config()
        {
            // Set up the countries.txt file to the list of valid countries
            StatsHandler.SetupCountryList();

            if (!Directory.Exists("Resources"))
                Directory.CreateDirectory("Resources");

            // If the file doesn't exist, WriteAllText with the json
            // If it exists, deserialize the json into the corresponding object
            if (!File.Exists("Resources/config.json"))
                File.WriteAllText("Resources/config.json", JsonConvert.SerializeObject(Bot, Formatting.Indented));
            else
                Bot = JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText("Resources/config.json"));
        }
    }
}