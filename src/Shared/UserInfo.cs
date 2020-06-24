namespace Marketplace.Shared
{
    public class UserInfo
    {        
        public string SteamId { get; set; }
        public string SteamAvatarUrl { get; set; }
        public string SteamName { get; set; }
        public string Role { get; set; }
        public bool IsAuthenticated { get; set; }
        public bool IsGlobalAdmin { get; set; }
    }
}
