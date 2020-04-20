using Dapper;
using Marketplace.Shared;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.DatabaseProvider.Repositories.Sql
{
    public class SqlServersRepository : IServersRepository
    {
        private readonly SqlConnection connection;

        public SqlServersRepository(SqlConnection connection)
        {
            this.connection = connection;
        }

        public async Task<IEnumerable<Server>> GetServersAsync()
        {
            const string sql = "SELECT * FROM dbo.Servers;";
            return await connection.QueryAsync<Server>(sql);
        }

        public async Task<Server> GetServerAsync(int serverId)
        {
            const string sql = "SELECT * FROM dbo.Servers WHERE Id = @serverId;";
            return (await connection.QueryAsync<Server>(sql, new { serverId })).FirstOrDefault();
        }

        public async Task<int> CreateServerAsync(Server server)
        {
            const string sql = "INSERT INTO dbo.Servers (ServerName, ServerIP, ServerPort, Enabled) VALUES (@ServerName, @ServerIP, @ServerPort, @Enabled); SELECT SCOPE_IDENTITY();";
            return await connection.ExecuteAsync(sql, server);
        }

        public async Task UpdateServerAsync(Server server)
        {
            const string sql = "UPDATE dbo.Servers SET ServerName = @ServerName, ServerIP = @ServerIP, ServerPort = @ServerPort WHERE Id = @Id;";
            await connection.ExecuteAsync(sql, server);
        }

        public Task Initialize()
        {
            return Task.CompletedTask;
        }
    }
}
