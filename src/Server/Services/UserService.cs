using Marketplace.Server.Constants;
using Marketplace.Shared;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<IUserService> logger;

        public UserService(ISettingService settingService, IHttpClientFactory httpClientFactory, IMemoryCache memoryCache, 
            ILogger<IUserService> logger)
        {
            this.settingService = settingService;
            this.httpClientFactory = httpClientFactory;
            this.memoryCache = memoryCache;
            this.logger = logger;
        }

        public async ValueTask<string> GetPlayerNameAsync(string steamId)
        {
            return (await GetPlayerSummariesAsync(steamId))?.Nickname ?? string.Empty;
        }

        public async ValueTask<UserInfo> GetUserInfoAsync(string steamId, string role, bool isAuthenticated)
        {
            var userInfo = new UserInfo()
            {
                SteamId = steamId,                
                Role = role,
                IsAuthenticated = isAuthenticated,
                IsGlobalAdmin = Environment.GetEnvironmentVariable("ADMIN_STEAMID") == steamId
            };

            var summaries = await GetPlayerSummariesAsync(steamId);
            if (summaries != null)
            {
                userInfo.SteamName = summaries.Nickname;
                userInfo.SteamAvatarUrl = summaries.AvatarFullUrl;
            }
            return userInfo;
        }

        public async ValueTask<PlayerSummaryModel> GetPlayerSummariesAsync(string steamId)
        {
            if (memoryCache.TryGetValue(CacheKeys.SteamSummariesId(steamId), out PlayerSummaryModel summaries))
            {
                return summaries;
            }
            
            try
            {
                summaries = await ForceGetPlayerSummaryModelAsync(steamId);
            } catch (Exception e)
            {
                logger.LogError("Failed to communicate with Steam Web API, Steam Dev Key may be null or invalid", e);
                return null;
            }
            
            memoryCache.Set(CacheKeys.SteamSummariesId(steamId), summaries, TimeSpan.FromMinutes(30));
            return summaries;
        }

        public async ValueTask<PlayerSummaryModel> ForceGetPlayerSummaryModelAsync(string steamId)
        {            
            if (!ulong.TryParse(steamId, out var parsedId))
                throw new ArgumentException(nameof(steamId));

            var setting = await settingService.GetSettingAsync("SteamDevKey", true);

            var factory = new SteamWebInterfaceFactory(setting.SettingValue);
            var steamUser = factory.CreateSteamWebInterface<SteamUser>(httpClientFactory.CreateClient());
            var summariesResponse = await steamUser.GetPlayerSummaryAsync(parsedId);
            return summariesResponse.Data;
        }
    }
}
