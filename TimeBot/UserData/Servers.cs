using System.Linq;
using Discord.WebSocket;
using System.Collections.Generic;

namespace TimeBot.UserData
{
    public static class Servers
    {
        private static List<ServerData> servers;

        private static string serversFile = "Resources/server_data.json";

        static Servers()
        {
            if (DateStorage.SaveExists(serversFile))
                servers = DateStorage.LoadServerData(serversFile).ToList();
            else
            {
                servers = new List<ServerData>();
                SaveServerData();
            }
        }

        public static void SaveServerData() => DateStorage.SaveServerData(servers, serversFile);

        public static ServerData GetServer(SocketGuild guild) => GetOrCreateServerData(guild.Id);

        private static ServerData GetOrCreateServerData(ulong id)
        {
            var result = from a in servers
                         where a.guildID == id
                         select a;
            var server = result.FirstOrDefault();
            if (server == null) server = CreateServerData(id);
            return server;
        }

        private static ServerData CreateServerData(ulong id)
        {
            var newServer = new ServerData()
            {
                guildID = id,
                serverTime = 999,
            };
            servers.Add(newServer);
            SaveServerData();
            return newServer;
        }
    }
}