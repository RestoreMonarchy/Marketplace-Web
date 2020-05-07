using Marketplace.Server.Constants;
using Marketplace.Shared;
using Microsoft.Extensions.Caching.Memory;
using Steam.Models.SteamCommunity;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Marketplace.Server.Services
{
    public class UserService : IUserService
    {
        private readonly ISettingService settingService;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IMemoryCache memoryCache;

        public UserService(ISettingService settingService, IHttpClientFactory httpClientFactory, IMemoryCache memoryCache)
        {
            this.settingService = settingService;
            this.httpClientFactory = httpClientFactory;
            this.memoryCache = memoryCache;
        }

        public async ValueTask<string> GetPlayerNameAsync(string steamId)
        {
            return (await GetPlayerSummariesAsync(steamId)).Nickname;
        }

        public async ValueTask<UserInfo> GetUserInfoAsync(string steamId, string role, bool isAuthenticated)
        {
            var summaries = await GetPlayerSummariesAsync(steamId);
            return new UserInfo()
            {
                SteamId = steamId,
                SteamName = summaries.Nickname,
                SteamAvatarUrl = summaries.AvatarFullUrl,                
                Role = role,
                IsAuthenticated = isAuthenticated,
                IsGlobalAdmin = Environment.GetEnvironmentVariable("ADMIN_STEAMID") == steamId
            };
        }

        public async ValueTask<PlayerSummaryModel> GetPlayerSummariesAsync(string steamId)
        {
            if (!ulong.TryParse(steamId, out var parsedId))
                throw new ArgumentException(nameof(steamId));

            return await memoryCache.GetOrCreateAsync(CacheKeys.SteamSummariesId(steamId), async (entry) =>
            {
                var setting = await settingService.GetSettingAsync("SteamDevKey", true);
                var factory = new SteamWebInterfaceFactory(setting.SettingValue);
                var steamUser = factory.CreateSteamWebInterface<SteamUser>(httpClientFactory.CreateClient());

                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                var summaries = await steamUser.GetPlayerSummaryAsync(parsedId);
                return summaries.Data;
            });
        }
    }
}
