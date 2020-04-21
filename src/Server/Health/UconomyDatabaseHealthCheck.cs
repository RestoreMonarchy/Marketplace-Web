using Dapper;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MySql.Data.MySqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace Marketplace.Server.Health
{
    public class EconomyDatabaseHealthCheck : IHealthCheck
    {
        private readonly MySqlConnection connection;
        public EconomyDatabaseHealthCheck(MySqlConnection connection)
        {
            this.connection = connection;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
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
