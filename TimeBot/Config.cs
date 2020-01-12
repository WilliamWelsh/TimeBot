using System.IO;

namespace TimeBot
{
    static class Config
    {
        static Config()
        {
            if (!Directory.Exists("Resources"))
                Directory.CreateDirectory("Resources");
        }
    }
}
