using Marketplace.Shared;
using System.Collections.Generic;

namespace Marketplace.Client.Models.Filters.Toggles
{
    public class ShowOnlyCompleteTransactionFilter : IToggleFilter<ProductTransaction>
    {
        public bool Enabled { get; set; } = false;

        public string Text => "Show Only Complete";

        public void Execute(List<ProductTransaction> data)
        {
            if (Enabled)
                data.RemoveAll(x => !x.IsComplete);
        }
    }
}
