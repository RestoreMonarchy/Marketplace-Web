using Marketplace.Server.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Marketplace.Server.Health
{
    public class SteamWebApiHealthCheck : IHealthCheck
    {
        private readonly IUserService userService;
        public SteamWebApiHealthCheck(IUserService userService)
        {
            this.userService = userService;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                await userService.ForceGetPlayerSummaryModelAsync("76561198285897058");
                return HealthCheckResult.Healthy();
            } catch (Exception e)
            {
                return HealthCheckResult.Degraded("Failed to communicate with Steam Web API, Steam Dev Key may be null or invalid", e);
            }
        }
    }
}
