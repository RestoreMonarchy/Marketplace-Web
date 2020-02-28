using Marketplace.Shared;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dapper;
using System.Linq;

namespace Marketplace.DatabaseProvider
{
    public class SqlDatabaseProvider : IDatabaseProvider
    {
        private readonly string _connectionString;
        private SqlConnection connection => new SqlConnection(_connectionString);
        public SqlDatabaseProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddItemIcon(ushort itemId, byte[] iconData)
        {
            string sql = "UPDATE dbo.UnturnedItems SET Icon = @iconData WHERE ItemId = @itemId;";
            using (connection)
            {
                connection.Executeas(sql, new { iconData, itemId = (int)itemId });
            }
        }

        public int AddMarketItem(MarketItem marketItem)
        {
            string sql = "INSERT INTO dbo.MarketItems (ItemId, Quality, Amount, Metadata, Price, SellerId) " +
                "VALUES (@ItemId, @Quality, @Amount, @Metadata, @Price, @SellerId);";
            using (connection)
            {
                return connection.ExecuteScalar<int>(sql, marketItem);
            }
        }

        public void AddUnturnedItem(UnturnedItem item)
        {
            string sql = "INSERT INTO dbo.UnturnedItems (ItemId, ItemName, ItemType, ItemDescription, Amount) " +
                "VALUES (@ItemId, @ItemName, @ItemType, @ItemDescription, @Amount);";
            using (connection)
            {
                connection.Execute(sql, item);
            }
        }

        public void BuyMarketItem(int id, string buyerId)
        {
            string sql = "UPDATE dbo.MarketItems SET IsSold = 1, BuyerId = @buyerId, SoldDate = SYSDATETIME() WHERE Id = @id;";
            using (connection)
            {
                connection.Execute(sql, new { id, buyerId });
            }
        }

        public void ChangePriceMarketItem(int id, decimal price)
        {
            string sql = "UPDATE dbo.MarketItems SET Price = @price WHERE Id = @id;";
            using (connection)
            {
                connection.Execute(sql, new { id, price });
            }
        }

        public void ClaimMarketItem(int id)
        {
            string sql = "UPDATE dbo.MarketItems SET IsClaimed = 1, ClaimDate = SYSDATETIME() WHERE Id = @id;";

            using (connection)
            {
                connection.Execute(sql, new { id });
            }
        }

        public byte[] GetItemIcon(ushort itemId)
        {
            string sql = "SELECT Icon FROM dbo.UnturnedItems WHERE ItemId = @itemId;";
            using (connection)
            {
                return connection.QuerySingle<byte[]>(sql, new { itemId = (int)itemId });
            }
        }

        public MarketItem GetMarketItem(int id)
        {
            string sql = "SELECT * FROM dbo.MarketItems WHERE Id = @id;";
            using (connection)
            {
                return connection.Query<MarketItem>(sql, new { id }).FirstOrDefault();
            }
        }

        public List<MarketItem> GetMarketItems()
        {
            string sql = "SELECT * FROM dbo.MarketItems;";
            using (connection)
            {
                return connection.Query<MarketItem>(sql).ToList();
            }
        }

        public List<MarketItem> GetPlayerMarketItems(string playerId)
        {
            string sql = "SELECT m.*, u.ItemName, u.ItemType, u.ItemDescription, u.Amount, u.Icon FROM dbo.MarketItems m " +
                "LEFT JOIN dbo.UnturnedItems u ON m.ItemId = u.ItemId WHERE BuyerId = @playerId OR SellerId = @playerId;";

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
            string sql = "SELECT u.*, m.Id, m.ItemId, m.Metadata, m.Quality, m.Price, m.SellerId, m.CreateDate FROM dbo.UnturnedItems u " +
                "LEFT JOIN dbo.MarketItems m ON m.ItemId = u.ItemId AND m.IsSold = 0 WHERE u.ItemId = @itemId;";

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
                "(SELECT COUNT(*) FROM dbo.MarketItems m WHERE m.ItemId = u.ItemId AND m.IsSold = 0) MarketItemsCount FROM dbo.UnturnedItems u;";
            using (connection)
            {
                return connection.Query<UnturnedItem>(sql).ToList();
            }
        }

        public List<UnturnedItem> GetUnturnedItemsIds()
        {
            string sql = "SELECT ItemId FROM dbo.UnturnedItems;";
            using (connection)
            {
                return connection.Query<UnturnedItem>(sql).ToList();
            }
        }

        public List<UnturnedItem> GetUnturnedItemsIdsNoIcon()
        {
            string sql = "SELECT ItemId FROM dbo.UnturnedItems WHERE Icon IS NULL;";
            using (connection)
            {
                return connection.Query<UnturnedItem>(sql).ToList();
            }
        }
    }
}
