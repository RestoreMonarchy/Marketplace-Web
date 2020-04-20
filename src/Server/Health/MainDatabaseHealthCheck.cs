using Dapper;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Marketplace.Server.Health
{
    public class MainDatabaseHealthCheck : IHealthCheck
    {
        public SqlConnection connection;
        
        public MainDatabaseHealthCheck(SqlConnection connection)
        {
            this.connection = connection;
        }
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                await connection.ExecuteScalarAsync("SELECT 1");
                return HealthCheckResult.Healthy();
            }
            catch (SqlException e)
            {
                return HealthCheckResult.Unhealthy("Connection string is invalid or not provided. " +
                    "Restart your docker container with valid MSSQL_CONNECTION_STRING env variable", e);
            }
        }
    }
}
