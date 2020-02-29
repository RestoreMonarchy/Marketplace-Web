using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.DatabaseProvider.Repositories.MySql
{
    public sealed class MySqlUconomyRepository : IUconomyRepository
    {
        private readonly MySqlConnection connection;

        public MySqlUconomyRepository(MySqlConnection connection)
        {
            this.connection = connection;
        }
        public Task<decimal> GetBalanceAsync(string id)
        {
            const string sql = "SELECT balance FROM uconomy WHERE steamId = @id;";

            return connection.ExecuteScalarAsync<decimal>(sql, new { id });
        }

        public async Task SetBalanceAsync(string id, decimal newBalance)
        {
            const string sql = "UPDATE uconomy SET balance = @newBalance, lastUpdated = @date WHERE steamId = @id;";

            await connection.ExecuteAsync(sql, new { newBalance, date = DateTime.Now, id });

        }
        public Task Initialize()
        {
            //throw new NotImplementedException(); TODO: ?
            return Task.CompletedTask;
        }

    }
}
