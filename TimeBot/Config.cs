using System.IO;

namespace TimeBot
{
    class Config
    {
        static Config()
        {
            if (!Directory.Exists("Resources"))
                Directory.CreateDirectory("Resources");
        }
    }
}