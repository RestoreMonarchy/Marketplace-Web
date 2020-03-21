using Marketplace.Shared;
using System.Collections.Generic;
using System.Linq;

namespace Marketplace.Client.Models.Filters.Orders
{
    public class ItemIdOrderFilter : IOrderFilter<UnturnedItem>
    {
        public string Text => "Item ID";

        public bool Enabled { get; set; } = false;

        public void Execute(ref List<UnturnedItem> data)
        {
            data = data.OrderByDescending(x => x.ItemId).ToList();
        }

        public void Toggle()
        {
            Enabled = !Enabled;
        }
    }
}
