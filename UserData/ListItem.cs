using Discord.Rest;

namespace TimeBot.UserData
{
    public class ListItem
    {
        public RestGuildUser User { get; set; }
        public UserAccount UserAccount { get; set; }
    }
}