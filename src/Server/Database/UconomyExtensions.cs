using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;

namespace Marketplace.Server.Database
{
    public static class UconomyExtensions
    {
        public static async Task UconomyPay(this MySqlConnection conn, string playerId, decimal amount)
        {
            string sql = "UPDATE uconomy SET balance = balance + @amount, lastUpdated = @date WHERE steamId = @playerId;";
            using (conn)
            {
                await conn.ExecuteAsync(sql, new { amount, date = DateTime.Now, playerId });
            }
        }

        public static async Task<decimal> UconomyGetBalanceAsync(this MySqlConnection conn, string playerId)
        {
            string sql = "SELECT balance FROM uconomy WHERE steamId = @playerId;";
            using (conn)
            {
                return await conn.ExecuteScalarAsync<decimal>(sql, new { playerId });
            }
        }
    }
}
