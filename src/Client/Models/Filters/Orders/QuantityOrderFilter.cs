using Marketplace.Shared;
using System.Collections.Generic;
using System.Linq;

namespace Marketplace.Client.Models.Filters.Orders
{
    public class QuantityOrderFilter : IOrderFilter<UnturnedItem>
    {
        public string Text => "Quantity";

        public bool Enabled { get; set; } = false;

        public void Execute(ref List<UnturnedItem> data)
        {
            data = data.OrderByDescending(x => x.MarketItemsCount).ToList();
        }

        public void Toggle()
        {
            Enabled = !Enabled;
        }
    }
}
