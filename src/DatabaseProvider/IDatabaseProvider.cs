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
        Task<IEnumerable<MarketItem>> GetMarketItems();
        Task<MarketItem> GetMarketItem(int id);
        Task<int> AddMarketItem(MarketItem marketItem);
        Task BuyMarketItem(int id, string buyerId);
        Task ChangePriceMarketItem(int id, decimal price);
        Task<IEnumerable<MarketItem>> GetPlayerMarketItems(string playerId);
        Task ClaimMarketItem(int id);

        // Unturned Items

        Task AddUnturnedItem(UnturnedItem item);
        Task AddItemIcon(ushort itemId, byte[] iconData);
        Task<List<UnturnedItem>> GetUnturnedItems();
        Task<byte[]> GetItemIcon(ushort itemId);
        Task<List<UnturnedItem>> GetUnturnedItemsIds();
        Task<List<UnturnedItem>> GetUnturnedItemsIdsNoIcon();
        Task<UnturnedItem> GetUnturnedItem(int itemId);
    }
}
