using Marketplace.Shared;
using System;
using System.Collections.Generic;

namespace DatabaseManager
{
    public interface IDatabaseManager
    {
        // Market Items
        List<MarketItem> GetMarketItems();
        MarketItem GetMarketItem(int id);
        int AddMarketItem(MarketItem marketItem);
        void BuyMarketItem(int id, string buyerId);
        void ChangePriceMarketItem(int id, decimal price);
        List<MarketItem> GetPlayerMarketItems(string playerId);
        void ClaimMarketItem(int id);

        // Unturned Items

        void AddUnturnedItem(UnturnedItem item);
        void AddItemIcon(ushort itemId, byte[] iconData);
        List<UnturnedItem> GetUnturnedItems();
        byte[] GetItemIcon(ushort itemId);
        List<UnturnedItem> GetUnturnedItemsIds();
        List<UnturnedItem> GetUnturnedItemsIdsNoIcon();
        UnturnedItem GetUnturnedItem(int itemId);
    }
}
