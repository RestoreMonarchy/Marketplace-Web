using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.DatabaseProvider.Repositories.MySql
{
    public class AviEconomyRepository : IEconomyRepository
    {
        private readonly MySqlConnection connection;
        public AviEconomyRepository(MySqlConnection connection)
        {
            this.connection = connection;
        }

        public async Task<decimal> GetBalanceAsync(string id)
        {
            const string sql = "SELECT Balance FROM bankaccount WHERE PlayerId = @id;";
            return await connection.ExecuteScalarAsync<decimal>(sql, new { id });
        }

        public async Task<decimal> GetTotalBalanceAsync()
        {
            const string sql = "SELECT SUM(Balance) AS total FROM bankaccount WHERE PlayerId <> 'Bank';";
            return await connection.ExecuteScalarAsync<decimal>(sql);
        }

        public async Task IncrementBalanceAsync(string id, decimal amount, DateTime? date = null)
        {
            if (date == null)
                date = DateTime.UtcNow;

            const string sql = "UPDATE bankaccount SET Balance = Balance + @amount, LastUpdate = @date WHERE PlayerId = @id;";
            await connection.ExecuteAsync(sql, new { id, amount, date });
        }

        public Task Initialize()
        {
            return Task.CompletedTask;
        }

        public async Task PayAsync(string senderId, string receiverId, decimal amount)
        {
            const string reason = "Marketplace";
            DateTime date = DateTime.UtcNow;

            const string sql = "INSERT INTO transaction (FromId, ToId, Amount, Timestamp, Reason) " +
                "VALUES (@senderId, @receiverId, @amount, @date, @reason);";
            await IncrementBalanceAsync(senderId, -amount, date);
            await IncrementBalanceAsync(receiverId, amount, date);
            await connection.ExecuteAsync(sql, new { senderId, receiverId, amount, date, reason });
        }
    }
}
