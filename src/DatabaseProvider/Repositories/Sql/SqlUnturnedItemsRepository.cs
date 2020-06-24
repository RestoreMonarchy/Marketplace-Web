﻿using Dapper;
using Marketplace.Shared;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace.DatabaseProvider.Repositories.Sql
{
    public sealed class SqlUnturnedItemsRepository : IUnturnedItemsRepository
    {
        private readonly SqlConnection connection;

        public SqlUnturnedItemsRepository(SqlConnection connection)
        {
            this.connection = connection;
        }  

        public async Task AddUnturnedItemAsync(UnturnedItem item)
        {
            const string sql = "INSERT INTO dbo.UnturnedItems (ItemId, ItemName, ItemType, ItemDescription, Amount) " +
                "VALUES (@ItemId, @ItemName, @ItemType, @ItemDescription, @Amount);";

            await connection.ExecuteAsync(sql, item);
        }

        public async Task SetIconAsync(int itemId, byte[] iconData)
        {
            const string sql = "UPDATE dbo.UnturnedItems SET Icon = @iconData WHERE ItemId = @itemId;";

            await connection.ExecuteAsync(sql, new { iconData, itemId });
        }

        public async Task<byte[]> GetItemIconAsync(int itemId)
        {
            const string sql = "SELECT Icon FROM dbo.UnturnedItems WHERE ItemId = @itemId;";

            return (await connection.QueryAsync<byte[]>(sql, new { itemId }))?.FirstOrDefault() ?? null;
        }

        public async Task<UnturnedItem> GetUnturnedItemAsync(int itemId)
        {
            const string sql = "SELECT u.*, m.Id, m.ItemId, m.Metadata, m.Quality, m.Price, m.SellerId, m.CreateDate FROM dbo.UnturnedItems u " +
                "LEFT JOIN dbo.MarketItems m ON m.ItemId = u.ItemId AND m.IsSold = 0 WHERE u.ItemId = @itemId;";

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
                "(SELECT COUNT(*) FROM dbo.MarketItems m WHERE m.ItemId = u.ItemId AND m.IsSold = 0) MarketItemsCount FROM dbo.UnturnedItems u;";

            return await connection.QueryAsync<UnturnedItem>(sql);
        }

        public async Task<IEnumerable<UnturnedItem>> GetUnturnedItemsIdsNoIconAsync()
        {
            const string sql = "SELECT ItemId FROM dbo.UnturnedItems WHERE Icon IS NULL;";

            return await connection.QueryAsync<UnturnedItem>(sql);
        }

        public Task Initialize()
        {
            return Task.CompletedTask;
        }
    }
}
