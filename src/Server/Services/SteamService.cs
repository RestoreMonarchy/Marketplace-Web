using Marketplace.Server.Constants;
using Microsoft.Extensions.Caching.Memory;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Marketplace.Server.Services
{
    public class SteamService : ISteamService
    {
        private readonly ISettingService settingService;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IMemoryCache memoryCache;

        public SteamService(ISettingService settingService, IHttpClientFactory httpClientFactory, IMemoryCache memoryCache)
        {
            this.settingService = settingService;
            this.httpClientFactory = httpClientFactory;
            this.memoryCache = memoryCache;
        }

        public async ValueTask<string> GetPlayerNameAsync(string steamId)
        {
            if (!ulong.TryParse(steamId, out var parsedId))
                throw new ArgumentException(nameof(steamId));

            return await memoryCache.GetOrCreateAsync(CacheKeys.SteamNicknameId(steamId), async (entry) =>
            {
                var setting = await settingService.GetSettingAsync("SteamDevKey", true);
                var factory = new SteamWebInterfaceFactory(setting.SettingValue);
                var steamUser = factory.CreateSteamWebInterface<SteamUser>(httpClientFactory.CreateClient());

                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);
                var summaries = await steamUser.GetPlayerSummaryAsync(parsedId);
                return summaries.Data.Nickname;
            });
        }
    }
}
