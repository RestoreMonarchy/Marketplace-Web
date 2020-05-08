using Marketplace.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Marketplace.Client.Models.Filters.Toggles
{
    public class ShowOnlyClaimedFilter : IToggleFilter<MarketItem>
    {
        public bool Enabled { get; set; } = true;

        public string Text => "Show Only Claimed";

        public void Execute(List<MarketItem> data)
        {
            if (Enabled)
                data.RemoveAll(x => x.IsClaimed);
        }
    }
}
