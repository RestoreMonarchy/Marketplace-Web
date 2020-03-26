using Marketplace.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marketplace.Client.Models.Filters.Orders
{
    public class LatestOrderFilter : IOrderFilter<MarketItem>
    {
        public string Text => "Latest";

        public bool Enabled { get; set; } = true;

        public void Execute(ref List<MarketItem> data)
        {
            data = data.OrderByDescending(x => x.CreateDate).ToList();
        }
    }
}
