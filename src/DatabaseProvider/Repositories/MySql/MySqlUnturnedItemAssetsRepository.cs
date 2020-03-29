using Dapper;
using Marketplace.Shared;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.DatabaseProvider.Repositories.MySql
{
    public sealed class MySqlUnturnedItemAssetsRepository : IUnturnedItemsRepository
    {
        private readonly MySqlConnection connection;

        public MySqlUnturnedItemAssetsRepository(MySqlConnection connection)
        {
            this.connection = connection;
        }

        public async Task SetIconAsync(ushort itemId, Stream iconData)
        {
            const string sql = "UPDATE UnturnedItems SET Icon = @iconData WHERE ItemId = @itemId;";

            byte[] buffer = new byte[iconData.Length];

            await iconData.ReadAsync(buffer, 0, (int)iconData.Length);

            await connection.ExecuteAsync(sql, new { buffer, itemId = (int)itemId });
        }

        public async Task AddUnturnedItemAsync(UnturnedItem item)
        {
            string sql = "INSERT INTO UnturnedItems (ItemId, ItemName, ItemType, ItemDescription, Amount) " +
                "VALUES (@ItemId, @ItemName, @ItemType, @ItemDescription, @Amount);";

            await connection.ExecuteScalarAsync(sql, item);
        }

        public async Task<Stream> GetItemIconAsync(ushort itemId)
        {
            const string sql = "SELECT Icon FROM UnturnedItems WHERE ItemId = @itemId;";

            return new MemoryStream(await connection.QuerySingleAsync<byte[]>(sql, new { itemId = (int)itemId })); //Check if dapper supports streamable retreieval, 
            //It could work because the lifetime of the stream is transient just like this and the service using it. Meaning the stream will be discarded by the time the connection is closed
        }

        public async Task<UnturnedItem> GetUnturnedItemAsync(int itemId)
        {
            const string sql = "SELECT u.*, m.Id, m.ItemId, m.Metadata, m.Quality, m.Price, m.SellerId, m.CreateDate FROM UnturnedItems u " +
                "LEFT JOIN MarketItems m ON m.ItemId = u.ItemId AND m.IsSold = 0 WHERE u.ItemId = @itemId;";

            UnturnedItem item = null;

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

            return item;
        }

        public async Task<IEnumerable<UnturnedItem>> GetUnturnedItemsAsync()
        {
            const string sql = "SELECT ItemId, ItemName, ItemType, ItemDescription, Amount, " +
                "(SELECT COUNT(*) FROM MarketItems m WHERE m.ItemId = u.ItemId AND m.IsSold = 0) MarketItemsCount FROM UnturnedItems u;";

            return (await connection.QueryAsync<UnturnedItem>(sql));
        }

        public async Task<IEnumerable<UnturnedItem>> GetUnturnedItemsIdsAsync()
        {
            const string sql = "SELECT ItemId FROM UnturnedItems;";

            return (await connection.QueryAsync<UnturnedItem>(sql));
        }

        public async Task<IEnumerable<UnturnedItem>> GetUnturnedItemsIdsNoIconAsync()
        {
            string sql = "SELECT ItemId FROM UnturnedItems WHERE Icon IS NULL;";

            return (await connection.QueryAsync<UnturnedItem>(sql));
        }

        public async Task Initialize()
        {
            const string sql = "CREATE TABLE IF NOT EXISTS UnturnedItems (ItemId INT NOT NULL PRIMARY KEY, ItemName TEXT NOT NULL, ItemType VARCHAR(255) NOT NULL, " +
                "ItemDescription TEXT NOT NULL, Amount TINYINT NOT NULL DEFAULT 1, Icon MEDIUMBLOB NULL);";
            await connection.ExecuteAsync(sql).ConfigureAwait(false);
        }
    }
}
