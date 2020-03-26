using Marketplace.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marketplace.Client.Models
{
    public class Order
    {
        public Order(MarketItem marketItem, UnturnedItem unturnedItem)
        {
            MarketItem = marketItem;
            UnturnedItem = unturnedItem;
        }

        public MarketItem MarketItem { get; set; }
        public UnturnedItem UnturnedItem { get; set; }
    }
}
