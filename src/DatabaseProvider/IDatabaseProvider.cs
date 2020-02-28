using Marketplace.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Marketplace.DatabaseProvider
{
    public interface IDatabaseProvider
    {
        Task Initialize();

        // Market Items
        Task<IEnumerable<MarketItem>> GetMarketItemsAsync();
        Task<MarketItem> GetMarketItemAsync(int id);
        Task<int> AddMarketItemAsync(MarketItem marketItem);
        Task BuyMarketItemAsync(int id, string buyerId);
        Task ChangePriceMarketItemAsync(int id, decimal price);
        Task<IEnumerable<MarketItem>> GetPlayerMarketItemsAsync(string playerId);
        Task ClaimMarketItemAsync(int id);

        // Unturned Items

        Task AddUnturnedItemAsync(UnturnedItem item);
        Task AddItemIconAsync(ushort itemId, byte[] iconData);
        Task<List<UnturnedItem>> GetUnturnedItemsAsync();
        Task<byte[]> GetItemIconAsync(ushort itemId);
        Task<List<UnturnedItem>> GetUnturnedItemsIdsAsync();
        Task<List<UnturnedItem>> GetUnturnedItemsIdsNoIconAsync();
        Task<UnturnedItem> GetUnturnedItemAsync(int itemId);
    }
}
