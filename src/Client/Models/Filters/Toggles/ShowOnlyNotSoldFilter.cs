using Marketplace.Shared;
using System.Collections.Generic;

namespace Marketplace.Client.Models.Filters.Toggles
{
    public class ShowOnlyNotSoldFilter : IToggleFilter<MarketItem>
    {
        public bool Enabled { get; set; } = true;

        public string Text => "Show Only Not Sold";

        public void Execute(List<MarketItem> data)
        {
            if (Enabled)
                data.RemoveAll(x => x.IsSold);
        }
    }
}
