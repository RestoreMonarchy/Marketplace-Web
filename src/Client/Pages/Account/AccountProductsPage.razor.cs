using Marketplace.Client.Models;
using Marketplace.Client.Models.Filters.Toggles;
using Marketplace.Client.Shared.Components.Modals;
using Marketplace.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Marketplace.Client.Pages.Account
{
    public partial class AccountProductsPage
    {
        [Inject]
        private HttpClient HttpClient { get; set; }

        private ProductTransactionModal Modal { get; set; }
        private FiltersData<ProductTransaction> FiltersData { get; set; }
        private IEnumerable<ProductTransaction> Transactions { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Transactions = await HttpClient.GetJsonAsync<IEnumerable<ProductTransaction>>("api/products/transactions");
            FiltersData = new FiltersData<ProductTransaction>(Transactions.ToList(), 15, true, new ShowOnlyCompleteTransactionFilter());
        }

        private string GetDotClass(ProductTransaction transaction) => transaction.IsComplete ? "dot-complete" : "dot-incomplete";
    }
}
