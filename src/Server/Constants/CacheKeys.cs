using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marketplace.Server.Constants
{
    public static class CacheKeys
    {
        public static string SteamNicknameId(string steamId) => $"SteamNickname_{steamId}";
    }
}
