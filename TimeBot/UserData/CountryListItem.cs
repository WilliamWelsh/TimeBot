using Discord.Rest;

namespace TimeBot.UserData
{
    public class CountryListItem
    {
        public RestGuildUser User { get; set; }
        public UserAccount UserAccount { get; set; }
    }
}