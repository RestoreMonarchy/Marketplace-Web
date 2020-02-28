using Dapper;
using Marketplace.Shared;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marketplace.DatabaseProvider
{
    public class MySqlDatabaseProvider : IDatabaseProvider
    {
        private readonly string _connectionString;
        private MySqlConnection connection => new MySqlConnection(_connectionString);

        public MySqlDatabaseProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task Initialize()
        {
            string sql = "CREATE TABLE IF NOT EXISTS UnturnedItems (ItemId INT NOT NULL PRIMARY KEY, ItemName TEXT NOT NULL, ItemType VARCHAR(255) NOT NULL, " +
                "ItemDescription TEXT NOT NULL, Amount TINYINT NOT NULL DEFAULT 1, Icon MEDIUMBLOB NULL);";
            string sql1 = "CREATE TABLE IF NOT EXISTS MarketItems (Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY, ItemId INT NOT NULL, Metadata BLOB NOT NULL, " +
                "Quality TINYINT(255) UNSIGNED NOT NULL, Amount TINYINT(255) UNSIGNED NOT NULL, Price DECIMAL(9, 2) NOT NULL, SellerId VARCHAR(255) NOT NULL, " +
                "CreateDate DATETIME NOT NULL DEFAULT NOW(), IsSold BIT NOT NULL DEFAULT 0, BuyerId VARCHAR(255) NULL, SoldDate DATETIME NULL, IsClaimed BIT NOT NULL DEFAULT 0, " +
                "ClaimDate DATETIME NULL, CONSTRAINT FK_MarketItems_ItemID FOREIGN KEY(ItemId) REFERENCES UnturnedItems(ItemId));";

            using (connection)
            {
                await connection.ExecuteAsync(sql);
                await connection.ExecuteAsync(sql1);
            }
        }

        public async Task AddItemIcon(ushort itemId, byte[] iconData)
        {
            const string sql = "UPDATE UnturnedItems SET Icon = @iconData WHERE ItemId = @itemId;";
            using (connection)
            {
                await connection.ExecuteAsync(sql, new { iconData, itemId = (int)itemId });
            }
        }

        public async Task<int> AddMarketItem(MarketItem marketItem)
        {
            string sql = "INSERT INTO MarketItems (ItemId, Quality, Amount, Metadata, Price, SellerId) " +
                "VALUES (@ItemId, @Quality, @Amount, @Metadata, @Price, @SellerId);";
            using (connection)
            {
                return await connection.ExecuteScalarAsync<int>(sql, marketItem);
            }
        }

        public async Task AddUnturnedItem(UnturnedItem item)
        {
            string sql = "INSERT INTO UnturnedItems (ItemId, ItemName, ItemType, ItemDescription, Amount) " +
                "VALUES (@ItemId, @ItemName, @ItemType, @ItemDescription, @Amount);";
            using (connection)
            {
                await connection.ExecuteScalarAsync(sql, item);
            }
        }

        public async Task BuyMarketItem(int id, string buyerId)
        {
            string sql = "UPDATE MarketItems SET IsSold = 1, BuyerId = @buyerId, SoldDate = NOW() WHERE Id = @id;";
            using (connection)
            {
                await connection.ExecuteAsync(sql, new { id, buyerId });
            }
        }

        public async Task ChangePriceMarketItem(int id, decimal price)
        {
            string sql = "UPDATE MarketItems SET Price = @price WHERE Id = @id;";
            using (connection)
            {
                await connection.ExecuteAsync(sql, new { id, price });
            }
        }

        public async Task ClaimMarketItem(int id)
        {
            string sql = "UPDATE MarketItems SET IsClaimed = 1, ClaimDate = NOW() WHERE Id = @id;";

            using (connection)
            {
                await connection.ExecuteAsync(sql, new { id });
            }
        }

        public async Task<byte[]> GetItemIcon(ushort itemId)
        {
            string sql = "SELECT Icon FROM UnturnedItems WHERE ItemId = @itemId;";
            using (connection)
            {
                return await connection.QuerySingleAsync<byte[]>(sql, new { itemId = (int)itemId });
            }
        }

        public async Task<MarketItem> GetMarketItem(int id)
        {
            string sql = "SELECT * FROM MarketItems WHERE Id = @id;";
            using (connection)
            {
                return (await connection.QueryAsync<MarketItem>(sql, new { id })).FirstOrDefault();
            }
        }

        public async Task<IEnumerable<MarketItem>> GetMarketItems()
        {
            string sql = "SELECT * FROM MarketItems;";
            using (connection)
            {

                return (await connection.QueryAsync<MarketItem>(sql));
            }
        }

        public async Task<IEnumerable<MarketItem>> GetPlayerMarketItems(string playerId)
        {
            string sql = "SELECT m.*, u.ItemName, u.ItemType, u.ItemDescription, u.Amount, u.Icon FROM MarketItems m " +
                "LEFT JOIN UnturnedItems u ON m.ItemId = u.ItemId WHERE BuyerId = @playerId OR SellerId = @playerId;";

            using (connection)
            {
                return (await connection.QueryAsync<MarketItem, UnturnedItem, MarketItem>(sql, (m, u) =>
                {
                    m.Item = u;
                    m.Item.ItemId = m.ItemId;
                    return m;
                }, new { playerId }, splitOn: "ItemName"));
            }
        }

        public async Task<UnturnedItem> GetUnturnedItem(int itemId)
        {
            string sql = "SELECT u.*, m.Id, m.ItemId, m.Metadata, m.Quality, m.Price, m.SellerId, m.CreateDate FROM UnturnedItems u " +
                "LEFT JOIN MarketItems m ON m.ItemId = u.ItemId AND m.IsSold = 0 WHERE u.ItemId = @itemId;";

            UnturnedItem item = null;
            using (connection)
            {
                await connection.QueryAsync<UnturnedItem, MarketItem, UnturnedItem>(sql, (u, m) =>
                {
                    if (item == null)
                    {
                        item = u;
                        u.MarketItems = new List<MarketItem>();
                    }
                    if (m != null && m.SellerId != null)
                    {
                        item.MarketItems.Add(m);
                    }
                    return null;
                }, new { itemId });
            }
            return item;
        }

        public async Task<List<UnturnedItem>> GetUnturnedItems()
        {
            string sql = "SELECT ItemId, ItemName, ItemType, ItemDescription, Amount, " +
                "(SELECT COUNT(*) FROM MarketItems m WHERE m.ItemId = u.ItemId AND m.IsSold = 0) MarketItemsCount FROM UnturnedItems u;";
            using (connection)
            {
                return (await connection.QueryAsync<UnturnedItem>(sql)).ToList();
            }
        }

        public async Task<List<UnturnedItem>> GetUnturnedItemsIds()
        {
            string sql = "SELECT ItemId FROM UnturnedItems;";
            using (connection)
            {
                return (await connection.QueryAsync<UnturnedItem>(sql)).ToList();
            }
        }

        public async Task<List<UnturnedItem>> GetUnturnedItemsIdsNoIcon()
        {
            string sql = "SELECT ItemId FROM UnturnedItems WHERE Icon IS NULL;";
            using (connection)
            {
                return (await connection.QueryAsync<UnturnedItem>(sql)).ToList();
            }
        }
    }
}
