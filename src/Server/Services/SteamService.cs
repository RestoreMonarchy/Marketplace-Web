using Marketplace.Server.Constants;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Marketplace.Server.Services
{

    public interface ISteamService
    {
        ValueTask<string> GetPlayerNameAsync(string steamId);
    }

    public class SteamService : ISteamService
    {
        private readonly SteamWebInterfaceFactory steamFactory;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly IMemoryCache memoryCache;
        private readonly SteamUser steamUser;


        public SteamService(SteamWebInterfaceFactory steamFactory,  IHttpClientFactory httpClientFactory, IMemoryCache memoryCache)
        {
            this.steamFactory = steamFactory;
            this.httpClientFactory = httpClientFactory;
            this.memoryCache = memoryCache;
            steamUser = steamFactory.CreateSteamWebInterface<SteamUser>(httpClientFactory.CreateClient());
        }

        public async ValueTask<string> GetPlayerNameAsync(string steamId)
        {
            if (!ulong.TryParse(steamId, out var parsedId))
                throw new ArgumentException(nameof(steamId));

            return await memoryCache.GetOrCreateAsync(CacheKeys.SteamNicknameId(steamId), async (entry) =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);
                var summaries = await steamUser.GetPlayerSummaryAsync(parsedId);
                return summaries.Data.Nickname;
            });
        }
    }
}
