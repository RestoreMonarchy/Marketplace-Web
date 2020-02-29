using Dapper;
using Marketplace.Shared;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.DatabaseProvider.Repositories.Sql
{
    public sealed class SqlMarketPlaceRepository : IMarketPlaceRepository
    {
        private readonly SqlConnection connection;

        public SqlMarketPlaceRepository(SqlConnection connection)
        {
            this.connection = connection;
        }

        public async Task<int> AddMarketItemAsync(MarketItem marketItem)
        {
            const string sql = "INSERT INTO dbo.MarketItems (ItemId, Quality, Amount, Metadata, Price, SellerId) " +
                "VALUES (@ItemId, @Quality, @Amount, @Metadata, @Price, @SellerId);";

            return await connection.ExecuteScalarAsync<int>(sql, marketItem);
        }

        public async Task BuyMarketItemAsync(int id, string buyerId)
        {
            const string sql = "UPDATE dbo.MarketItems SET IsSold = 1, BuyerId = @buyerId, SoldDate = SYSDATETIME() WHERE Id = @id;";

            await connection.ExecuteAsync(sql, new { id, buyerId });
        }

        public async Task ChangePriceMarketItemAsync(int id, decimal price)
        {
            const string sql = "UPDATE dbo.MarketItems SET Price = @price WHERE Id = @id;";

            await connection.ExecuteAsync(sql, new { id, price });

        }

        public async Task ClaimMarketItemAsync(int id)
        {
            const string sql = "UPDATE dbo.MarketItems SET IsClaimed = 1, ClaimDate = SYSDATETIME() WHERE Id = @id;";


            await connection.ExecuteAsync(sql, new { id });
        }

        public async Task<MarketItem> GetMarketItemAsync(int id)
        {
            const string sql = "SELECT * FROM dbo.MarketItems WHERE Id = @id LIMIT 1;"; //Should work

            return await connection.QuerySingleOrDefaultAsync<MarketItem>(sql, new { id });
        }

        public async Task<IEnumerable<MarketItem>> GetMarketItemsAsync()
        {
            const string sql = "SELECT * FROM dbo.MarketItems;";

            return await connection.QueryAsync<MarketItem>(sql);
        }

        public async Task<IEnumerable<MarketItem>> GetPlayerMarketItemsAsync(string playerId)
        {
            const string sql = "SELECT m.*, u.ItemName, u.ItemType, u.ItemDescription, u.Amount, u.Icon FROM dbo.MarketItems m " +
                            "LEFT JOIN dbo.UnturnedItems u ON m.ItemId = u.ItemId WHERE BuyerId = @playerId OR SellerId = @playerId;";

            return await connection.QueryAsync<MarketItem, UnturnedItem, MarketItem>(sql, (m, u) =>
            {
                m.Item = u;
                m.Item.ItemId = m.ItemId;
                return m;
            }, new { playerId }, splitOn: "ItemName"); //Once again, this feels ugly

        }

        public async Task Initialize()
        {
            const string sql = "CREATE TABLE [dbo].[MarketItems](Id INT IDENTITY(1, 1) NOT NULL CONSTRAINT PK_MarketItems PRIMARY KEY, ItemId INT NOT NULL CONSTRAINT FK_MarketItems_ItemID REFERENCES UnturnedItems(ItemId), Metadata VARBINARY(128) NOT NULL, Quality TINYINT NOT NULL, Amount TINYINT NOT NULL, Price DECIMAL(9, 2) NOT NULL SellerId VARCHAR(255) NOT NULL, CreateDate DATETIME2(0) NOT NULL CONSTRAINT DF_MarketItems_CreateDate DEFAULT SYSDATETIME(), IsSold BIT NOT NULL CONSTRAINT DF_MarketItems_IsSold DEFAULT 0, BuyerId VARCHAR(255) NULL, SoldDate DATETIME2(0) NULL, IsClaimed BIT NOT NULL CONSTRAINT DF_MarketItems_IsClaimed DEFAULT 0 ClaimDate DATETIME2(0) NULL);";

            await connection.ExecuteAsync(sql);
        }
    }
}
