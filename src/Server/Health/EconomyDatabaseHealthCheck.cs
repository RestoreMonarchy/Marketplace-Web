using Dapper;
using Marketplace.Server.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MySql.Data.MySqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace Marketplace.Server.Health
{
    public class EconomyDatabaseHealthCheck : IHealthCheck
    {
        private readonly MySqlConnection connection;
        private readonly ISettingService settingService;
        public EconomyDatabaseHealthCheck(MySqlConnection connection, ISettingService settingService)
        {
            this.connection = connection;
            this.settingService = settingService;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var providerSetting = await settingService.GetSettingAsync("EconomyProvider", true);
                string sql = "";
                if (providerSetting.SettingValue == "AviEconomy")

                else
                    

                await connection.ExecuteScalarAsync("SELECT 1");
                return HealthCheckResult.Healthy();

            } catch (MySqlException e)
            {
                return HealthCheckResult.Unhealthy("Failed to connect to Uconomy MySQL server. " +
                        "Sign in with admin and edit Uconomy Connection String in Dashboard page", e);
            }
        }
    }
}
