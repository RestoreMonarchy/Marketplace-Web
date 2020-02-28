using Dapper;
using MySql.Data.MySqlClient;
using System;

namespace Marketplace.Server.Database
{
    public static class UconomyExtensions
    {
        public static void UconomyPay(this MySqlConnection conn, string playerId, decimal amount)
        {
            string sql = "UPDATE uconomy SET balance = balance + @amount, lastUpdated = @date WHERE steamId = @playerId;";
            using (conn)
            {
                conn.Execute(sql, new { amount, date = DateTime.Now, playerId });
            }
        }

        public static decimal UconomyGetBalance(this MySqlConnection conn, string playerId)
        {
            string sql = "SELECT balance FROM uconomy WHERE steamId = @playerId;";
            using (conn)
            {
                return conn.ExecuteScalar<decimal>(sql, new { playerId });
            }
        }
    }
}
