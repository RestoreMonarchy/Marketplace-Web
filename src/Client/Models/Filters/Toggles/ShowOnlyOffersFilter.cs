using Marketplace.Shared;
using System.Collections.Generic;

namespace Marketplace.Client.Models.Filters
{
    public class ShowOnlyOffersFilter : IToggleFilter<UnturnedItem>
    {
        public bool Enabled { get; private set; } = true;

        public string Text => "Show Only With Offers";

        public void Execute(List<UnturnedItem> data)
        {
            data.RemoveAll(x => x.MarketItemsCount == 0);
        }

        public void Toggle()
        {
            Enabled = !Enabled;
        }
    }
}
