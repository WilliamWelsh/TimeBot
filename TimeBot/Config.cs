using System.IO;

namespace TimeBot
{
    internal static class Config
    {
        static Config()
        {
            if (!Directory.Exists("Resources"))
                Directory.CreateDirectory("Resources");
        }
    }
}
