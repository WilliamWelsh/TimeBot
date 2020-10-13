using System.IO;

namespace TimeBot
{
    internal static class Config
    {
        public static bool IS_TESTING = false;

        static Config()
        {
            if (!Directory.Exists("Resources"))
                Directory.CreateDirectory("Resources");
        }
    }
}
