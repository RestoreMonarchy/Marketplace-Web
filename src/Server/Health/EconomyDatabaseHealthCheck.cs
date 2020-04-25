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
            var providerSetting = await settingService.GetSettingAsync("EconomyProvider", true);
            try
            {   
                string sql;
                if (providerSetting.SettingValue == "AviEconomy")
                    sql = "SELECT TOP 1 * FROM bankaccount;";
                else
                    sql = "SELECT TOP 1 * FROM uconomy;";

                await connection.ExecuteScalarAsync(sql);
                return HealthCheckResult.Healthy();

            } catch (MySqlException e)
            {                
                return HealthCheckResult.Unhealthy($"Failed to fetch data from {providerSetting.SettingValue} table." +
                        "Sign in with admin and edit EconomyConnectionString in Dashboard page", e);
            }
        }
    }
}
