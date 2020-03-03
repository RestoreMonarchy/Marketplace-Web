using System;
using System.Collections.Generic;
using System.Text;

namespace Marketplace.Shared
{
    public class MarketItem
    {
        public MarketItem() { }
        public MarketItem(ushort itemId, decimal price, byte quality, byte amount, byte[] state, string sellerId)
        {
            ItemId = itemId;
            Price = price;
            Quality = quality;
            Amount = amount; 
            Metadata = state;            
            SellerId = sellerId;
        }

        public int Id { get; set; }
        public ushort ItemId { get; set; }
        public byte Quality { get; set; }
        public byte Amount { get; set; }
        public byte[] Metadata { get; set; }

        public ItemInfo ItemInfo => ItemInfo.Create(ItemId, Quality, Amount, Metadata); //Due to this being the db model we can't use this as the storage afaik

        public decimal Price { get; set; }
        public string SellerId { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsSold { get; set; }
        public string BuyerId { get; set; }
        public DateTime? SoldDate { get; set; }
        public bool IsClaimed { get; set; }
        public DateTime? ClaimDate { get; set; }

        public UnturnedItem Item { get; set; }
    }
}
