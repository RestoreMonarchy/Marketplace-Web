using Marketplace.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marketplace.Client.Models.Filters.Orders
{
    public class PriceOrderFilter : IOrderFilter<MarketItem>
    {
        public string Text => "Lowest Price";

        public bool Enabled { get; set; } = false;

        public void Execute(ref List<MarketItem> data)
        {
            data = data.OrderBy(x => x.Price).ToList();
        }
    }
}
