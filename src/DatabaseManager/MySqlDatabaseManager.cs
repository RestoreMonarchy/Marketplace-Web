using Dapper;
using Marketplace.Shared;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseManager
{
    public class MySqlDatabaseManager : IDatabaseManager
    {
        private readonly string _connectionString;
        private MySqlConnection connection => new MySqlConnection(_connectionString);

        public MySqlDatabaseManager(string connectionString)
        {
            _connectionString = connectionString;
            InitializeTables();
        }

        void InitializeTables()
        {
            string sql = "CREATE TABLE IF NOT EXISTS UnturnedItems (ItemId INT NOT NULL PRIMARY KEY, ItemName TEXT NOT NULL, ItemType VARCHAR(255) NOT NULL, " +
                "ItemDescription TEXT NOT NULL, Amount TINYINT NOT NULL DEFAULT 1, Icon MEDIUMBLOB NULL);";
            string sql1 = "CREATE TABLE IF NOT EXISTS MarketItems (Id INT NOT NULL AUTO_INCREMENT PRIMARY KEY, ItemId INT NOT NULL, Metadata BLOB NOT NULL, " +
                "Quality TINYINT(255) UNSIGNED NOT NULL, Amount TINYINT(255) UNSIGNED NOT NULL, Price DECIMAL(9, 2) NOT NULL, SellerId VARCHAR(255) NOT NULL, " +
                "CreateDate DATETIME NOT NULL DEFAULT NOW(), IsSold BIT NOT NULL DEFAULT 0, BuyerId VARCHAR(255) NULL, SoldDate DATETIME NULL, IsClaimed BIT NOT NULL DEFAULT 0, " +
                "ClaimDate DATETIME NULL, CONSTRAINT FK_MarketItems_ItemID FOREIGN KEY(ItemId) REFERENCES UnturnedItems(ItemId));";

            using (connection)
            {
                connection.Execute(sql);
                connection.Execute(sql1);
            }
        }

        public void AddItemIcon(ushort itemId, byte[] iconData)
        {
            string sql = "UPDATE UnturnedItems SET Icon = @iconData WHERE ItemId = @itemId;";
            using (connection)
            {
                connection.Execute(sql, new { iconData, itemId = (int)itemId });
            }
        }

        public int AddMarketItem(MarketItem marketItem)
        {
            string sql = "INSERT INTO MarketItems (ItemId, Quality, Amount, Metadata, Price, SellerId) " +
                "VALUES (@ItemId, @Quality, @Amount, @Metadata, @Price, @SellerId);";
            using (connection)
            {
                return connection.ExecuteScalar<int>(sql, marketItem);
            }
        }

        public void AddUnturnedItem(UnturnedItem item)
        {
            string sql = "INSERT INTO UnturnedItems (ItemId, ItemName, ItemType, ItemDescription, Amount) " +
                "VALUES (@ItemId, @ItemName, @ItemType, @ItemDescription, @Amount);";
            using (connection)
            {
                connection.Execute(sql, item);
            }
        }

        public void BuyMarketItem(int id, string buyerId)
        {
            string sql = "UPDATE MarketItems SET IsSold = 1, BuyerId = @buyerId, SoldDate = NOW() WHERE Id = @id;";
            using (connection)
            {
                connection.Execute(sql, new { id, buyerId });
            }
        }

        public void ChangePriceMarketItem(int id, decimal price)
        {
            string sql = "UPDATE MarketItems SET Price = @price WHERE Id = @id;";
            using (connection)
            {
                connection.Execute(sql, new { id, price });
            }
        }

        public void ClaimMarketItem(int id)
        {
            string sql = "UPDATE MarketItems SET IsClaimed = 1, ClaimDate = NOW() WHERE Id = @id;";

            using (connection)
            {
                connection.Execute(sql, new { id });
            }
        }

        public byte[] GetItemIcon(ushort itemId)
        {
            string sql = "SELECT Icon FROM UnturnedItems WHERE ItemId = @itemId;";
            using (connection)
            {
                return connection.QuerySingle<byte[]>(sql, new { itemId = (int)itemId });
            }
        }

        public MarketItem GetMarketItem(int id)
        {
            string sql = "SELECT * FROM MarketItems WHERE Id = @id;";
            using (connection)
            {
                return connection.Query<MarketItem>(sql, new { id }).FirstOrDefault();
            }
        }

        public List<MarketItem> GetMarketItems()
        {
            string sql = "SELECT * FROM MarketItems;";
            using (connection)
            {
                return connection.Query<MarketItem>(sql).ToList();
            }
        }

        public List<MarketItem> GetPlayerMarketItems(string playerId)
        {
            string sql = "SELECT m.*, u.ItemName, u.ItemType, u.ItemDescription, u.Amount, u.Icon FROM MarketItems m " +
                "LEFT JOIN UnturnedItems u ON m.ItemId = u.ItemId WHERE BuyerId = @playerId OR SellerId = @playerId;";

            using (connection)
            {
                return connection.Query<MarketItem, UnturnedItem, MarketItem>(sql, (m, u) =>
                {
                    m.Item = u;
                    m.Item.ItemId = m.ItemId;
                    return m;
                }, new { playerId }, splitOn: "ItemName").ToList();
            }
        }

        public UnturnedItem GetUnturnedItem(int itemId)
        {
            string sql = "SELECT u.*, m.Id, m.ItemId, m.Metadata, m.Quality, m.Price, m.SellerId, m.CreateDate FROM UnturnedItems u " +
                "LEFT JOIN MarketItems m ON m.ItemId = u.ItemId AND m.IsSold = 0 WHERE u.ItemId = @itemId;";

            UnturnedItem item = null;
            using (connection)
            {
                connection.Query<UnturnedItem, MarketItem, UnturnedItem>(sql, (u, m) =>
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

        public List<UnturnedItem> GetUnturnedItems()
        {
            string sql = "SELECT ItemId, ItemName, ItemType, ItemDescription, Amount, " +
                "(SELECT COUNT(*) FROM MarketItems m WHERE m.ItemId = u.ItemId AND m.IsSold = 0) MarketItemsCount FROM UnturnedItems u;";
            using (connection)
            {
                return connection.Query<UnturnedItem>(sql).ToList();
            }
        }

        public List<UnturnedItem> GetUnturnedItemsIds()
        {
            string sql = "SELECT ItemId FROM UnturnedItems;";
            using (connection)
            {
                return connection.Query<UnturnedItem>(sql).ToList();
            }
        }

        public List<UnturnedItem> GetUnturnedItemsIdsNoIcon()
        {
            string sql = "SELECT ItemId FROM UnturnedItems WHERE Icon IS NULL;";
            using (connection)
            {
                return connection.Query<UnturnedItem>(sql).ToList();
            }
        }
    }
}
