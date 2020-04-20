using Microsoft.Extensions.Diagnostics.HealthChecks;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Marketplace.Server.Health
{
    public class UconomyDatabaseHealthCheck : IHealthCheck
    {
        private readonly MySqlConnection connection;
        public UconomyDatabaseHealthCheck(MySqlConnection connection)
        {
            this.connection = connection;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            if (!connection.Ping())
                return Task.FromResult(HealthCheckResult.Degraded());
            return Task.FromResult(HealthCheckResult.Healthy());
        }
    }
}
