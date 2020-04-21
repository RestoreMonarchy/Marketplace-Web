using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;

namespace Marketplace.DatabaseProvider.Repositories.MySql
{
    public sealed class UconomyEconomyRepository : IEconomyRepository
    {
        private readonly MySqlConnection connection;

        public UconomyEconomyRepository(MySqlConnection connection)
        {
            this.connection = connection;
        }

        public Task<decimal> GetBalanceAsync(string id)
        {
            const string sql = "SELECT balance FROM uconomy WHERE steamId = @id;";

            return connection.ExecuteScalarAsync<decimal>(sql, new { id });
        }

        public async Task IncrementBalanceAsync(string id, decimal amount, DateTime? date = null)
        {
            if (date == null)
                date = DateTime.UtcNow;
            const string sql = "UPDATE uconomy SET balance = @newBalance, lastUpdated = @date WHERE steamId = @id;";

            await connection.ExecuteAsync(sql, new { amount, date, id });

        }
        public Task Initialize()
        {
            return Task.CompletedTask;
        }

        public async Task<decimal> GetTotalBalanceAsync()
        {
            const string sql = "SELECT SUM(balance) AS total FROM uconomy;";
            return await connection.ExecuteScalarAsync<decimal>(sql);
        }

        public async Task PayAsync(string senderId, string receiverId, decimal amount)
        {
            DateTime date = DateTime.UtcNow;

            await IncrementBalanceAsync(senderId, -amount, date);
            await IncrementBalanceAsync(receiverId, amount, date);
        }
    }
}
