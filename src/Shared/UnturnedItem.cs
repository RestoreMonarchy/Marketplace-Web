using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Shared
{
    public class UnturnedItem
    {
        public UnturnedItem(ushort itemId, string itemName, EItemType itemType, string itemDescription, byte amount, byte[] icon = null)
        {
            ItemId = itemId;
            ItemName = itemName;
            ItemType = itemType;
            ItemDescription = itemDescription;
            Amount = amount;
            Icon = icon;
        }

        public UnturnedItem() { }

        public ushort ItemId { get; set; }
        public string ItemName { get; set; }
        public EItemType ItemType { get; set; }
        public string ItemDescription { get; set; }
        public byte Amount { get; set; }
        public byte[] Icon { get; set; }

        public int MarketItemsCount { get; set; }
        public ICollection<MarketItem> MarketItems { get; set; }
    }
}
