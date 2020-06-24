using Marketplace.Shared.Attributes;
using System;

namespace Marketplace.Shared
{
    public class MarketItem
    {
        public MarketItem() { }
        public MarketItem(int itemId, decimal price, byte quality, byte amount, byte[] state, string sellerId)
        {
            ItemId = itemId;
            Price = price;
            Quality = quality;
            Amount = amount; 
            Metadata = state;            
            SellerId = sellerId;
        }

        [Searchable]
        public int Id { get; set; }
        [Searchable]
        public int ItemId { get; set; }
        public byte Quality { get; set; }
        public byte Amount { get; set; }
        public byte[] Metadata { get; set; }

        [Searchable]
        public string ItemName => Item?.ItemName ?? string.Empty;

        public decimal Price { get; set; }
        public string SellerId { get; set; }
        public string SellerName { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsSold { get; set; }
        public string BuyerId { get; set; }
        public string BuyerName { get; set; }
        public DateTime? SoldDate { get; set; }
        public bool IsClaimed { get; set; }
        public DateTime? ClaimDate { get; set; }

        public UnturnedItem Item { get; set; }
        
    }
}
