using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marketplace.Server.Constants
{
    public static class CacheKeys
    {
        public static string SettingsId => "Settings";
        public static string SteamSummariesId(string steamId) => $"SteamSummaries_{steamId}";
        public static string SettingId(string settingId) => $"Setting_{settingId}";

        public static string ItemIconId(int itemId) => $"ItemIcon_{itemId}";
    }
}
