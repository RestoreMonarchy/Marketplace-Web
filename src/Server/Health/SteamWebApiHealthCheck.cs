using Marketplace.Server.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Marketplace.Server.Health
{
    public class SteamWebApiHealthCheck : IHealthCheck
    {
        private readonly ISettingService settingService;
        private readonly IHttpClientFactory httpClientFactory;
        public SteamWebApiHealthCheck(ISettingService settingService, IHttpClientFactory httpClientFactory)
        {
            this.settingService = settingService;
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var factory = new SteamWebInterfaceFactory((await settingService.GetSettingAsync("SteamDevKey", true)).SettingValue);
                var news = factory.CreateSteamWebInterface<ISteamNews>(httpClientFactory.CreateClient());
                await news.GetNewsForAppAsync(304930, count: 1);
                return HealthCheckResult.Healthy();
            } catch (Exception e)
            {
                return HealthCheckResult.Degraded("Steam Web API Key is invalid, sign in as admin and change it in dashboard page", e);
            }
        }
    }
}
