using Dapper;
using Marketplace.Shared;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Marketplace.DatabaseProvider.Repositories.Sql
{
    public sealed class SqlMarketItemsRepository : IMarketItemsRepository
    {
        private readonly SqlConnection connection;

        public SqlMarketItemsRepository(SqlConnection connection)
        {
            this.connection = connection;
        }

        public async Task<int> SellMarketItemAsync(MarketItem marketItem)
        {
            var p = new DynamicParameters();
            p.Add("@ItemId", marketItem.ItemId);
            p.Add("@Metadata", marketItem.Metadata);
            p.Add("@Quality", marketItem.Quality);
            p.Add("@Amount", marketItem.Amount);
            p.Add("@Price", marketItem.Price);
            p.Add("@SellerId", marketItem.SellerId);
            p.Add("@returnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
            await connection.ExecuteAsync("SellMarketItem", p, commandType: CommandType.StoredProcedure);
            return p.Get<int>("@returnValue");
        }

        public async Task<int> BuyMarketItemAsync(int id, string buyerId, decimal balance)
        {
            var p = new DynamicParameters();
            p.Add("@Id", id);
            p.Add("@BuyerId", buyerId);
            p.Add("@Balance", balance);
            p.Add("@returnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
            await connection.ExecuteAsync("BuyMarketItem", p, commandType: CommandType.StoredProcedure);
            return p.Get<int>("@returnValue");
        }

        public async Task<int> ChangePriceMarketItemAsync(int id, string playerId, decimal price)
        {
            var p = new DynamicParameters();
            p.Add("@Id", id);
            p.Add("@ChangerId", playerId);
            p.Add("@Price", price);
            p.Add("@returnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
            await connection.ExecuteAsync("ChangePriceMarketItem", p, commandType: CommandType.StoredProcedure);
            return p.Get<int>("@returnValue");

        }

        public async Task ClaimMarketItemAsync(int id)
        {
            const string sql = "UPDATE dbo.MarketItems SET IsClaimed = 1, ClaimDate = SYSDATETIME() WHERE Id = @id;";

            await connection.ExecuteAsync(sql, new { id });
        }

        public async Task<MarketItem> GetMarketItemAsync(int id)
        {
            const string sql = "SELECT * FROM dbo.MarketItems WHERE Id = @id;";

            return await connection.QuerySingleOrDefaultAsync<MarketItem>(sql, new { id });
        }

        public async Task<IEnumerable<MarketItem>> GetMarketItemsAsync()
        {
            const string sql = "SELECT m.Id, m.Metadata, m.Quality, m.Amount, m.Price, m.SellerId, m.CreateDate, m.IsSold, u.ItemId, u.ItemName, " +
                "u.ItemDescription, u.ItemType, u.Amount FROM dbo.MarketItems m JOIN dbo.UnturnedItems u ON m.ItemId = u.ItemId WHERE m.IsSold = 0;";

            return await connection.QueryAsync<MarketItem, UnturnedItem, MarketItem>(sql, (m, u) => 
            {
                m.ItemId = u.ItemId;
                m.Item = u;
                return m;
            }, splitOn: "ItemId");
        }

        public async Task<IEnumerable<MarketItem>> GetPlayerMarketItemsAsync(string playerId)
        {
            const string sql = "SELECT m.Id, m.Metadata, m.Quality, m.Amount, m.Price, m.SellerId, m.CreateDate, m.IsSold, m.BuyerId, m.SoldDate, m.IsClaimed, " +
                "m.ClaimDate, u.ItemId, u.ItemName, u.ItemType, u.ItemDescription, u.Amount FROM dbo.MarketItems m " +
                "LEFT JOIN dbo.UnturnedItems u ON m.ItemId = u.ItemId WHERE BuyerId = @playerId OR SellerId = @playerId;";

            return await connection.QueryAsync<MarketItem, UnturnedItem, MarketItem>(sql, (m, u) =>
            {
                m.ItemId = u.ItemId;
                m.Item = u;
                return m;
            }, new { playerId }, splitOn: "ItemId");
        }

        public Task Initialize()
        {
            return Task.CompletedTask;
        }
    }
}
