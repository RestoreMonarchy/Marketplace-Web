using Dapper;
using Marketplace.Shared;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.DatabaseProvider.Repositories.MySql
{
    public class MySqlMarketPlaceRepository : IMarketItemsRepository
    {
        private readonly MySqlConnection connection;

        public MySqlMarketPlaceRepository(MySqlConnection connection)
        {
            this.connection = connection;
        }

        public async Task<int> SellMarketItemAsync(MarketItem marketItem)
        {
            const string sql = "INSERT INTO MarketItems (ItemId, Quality, Amount, Metadata, Price, SellerId) " +
                "VALUES (@SellingItem.ItemId, @Quality, @Amount, @Metadata, @Price, @SellerId);";

            return await connection.ExecuteScalarAsync<int>(sql, marketItem);
        }

        //public async Task BuyMarketItemAsync(int id, string buyerId)
        //{
        //    const string sql = "UPDATE MarketItems SET IsSold = 1, BuyerId = @buyerId, SoldDate = NOW() WHERE Id = @id;";

        //    await connection.ExecuteAsync(sql, new { id, buyerId });
        //}

        public Task<int> BuyMarketItemAsync(int id, string buyerId, decimal balance)
        {
            throw new NotImplementedException();
        }

        public async Task ChangePriceMarketItemAsync(int id, decimal price)
        {
            const string sql = "UPDATE MarketItems SET Price = @price WHERE Id = @id;";

            await connection.ExecuteAsync(sql, new { id, price });
        }

        public async Task ClaimMarketItemAsync(int id)
        {
            const string sql = "UPDATE MarketItems SET IsClaimed = 1, ClaimDate = NOW() WHERE Id = @id;";


            await connection.ExecuteAsync(sql, new { id });
        }

        public async Task<MarketItem> GetMarketItemAsync(int id)
        {
            const string sql = "SELECT * FROM MarketItems WHERE Id = @id LIMIT 1;"; //Added limit 1, haven't done sql in a long time so please tell me if this is not gonna work

            return (await connection.QuerySingleOrDefaultAsync<MarketItem>(sql, new { id }));
        }

        public async Task<IEnumerable<MarketItem>> GetMarketItemsAsync()
        {
            const string sql = "SELECT * FROM MarketItems;";


            return (await connection.QueryAsync<MarketItem>(sql)); //An Async enumerable would be dope here, lets look into the possibility to implement it.
        }

        public async Task<IEnumerable<MarketItem>> GetPlayerMarketItemsAsync(string playerId)
        {
            const string sql = "SELECT m.*, u.ItemName, u.ItemType, u.ItemDescription, u.Amount, u.Icon FROM MarketItems m " +
                "LEFT JOIN UnturnedItems u ON m.ItemId = u.ItemId WHERE BuyerId = @playerId OR SellerId = @playerId;";


            return (await connection.QueryAsync<MarketItem, UnturnedItem, MarketItem>(sql, (m, u) =>
            {
                m.Item = u;
                m.Item.ItemId = m.ItemId;
                return m;
            }, new { playerId }, splitOn: "ItemName"));
        }

        public async Task Initialize()
        {
            const string sql = "CREATE TABLE IF NOT EXISTS MarketItems (Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY, ItemId INT NOT NULL, Metadata BLOB NOT NULL, " +
                "Quality TINYINT(255) UNSIGNED NOT NULL, Amount TINYINT(255) UNSIGNED NOT NULL, Price DECIMAL(9, 2) NOT NULL, SellerId VARCHAR(255) NOT NULL, " +
                "CreateDate DATETIME NOT NULL DEFAULT NOW(), IsSold BIT NOT NULL DEFAULT 0, BuyerId VARCHAR(255) NULL, SoldDate DATETIME NULL, IsClaimed BIT NOT NULL DEFAULT 0, " +
                "ClaimDate DATETIME NULL, CONSTRAINT FK_MarketItems_ItemID FOREIGN KEY(ItemId) REFERENCES UnturnedItems(ItemId));";
            await connection.ExecuteAsync(sql).ConfigureAwait(false);
        }

        public Task BuyMarketItemAsync(int id, string buyerId)
        {
            throw new NotImplementedException();
        }

        public Task<int> ChangePriceMarketItemAsync(int id, string playerId, decimal price)
        {
            throw new NotImplementedException();
        }
    }
}
