using Marketplace.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Marketplace.Client.Pages.Dashboard
{
    [Authorize(Roles = "Admin")]
    public partial class DashboardPage
    {
        [Inject]
        public HttpClient HttpClient { get; set; }
        private List<UnturnedItem> UnturnedItems { get; set; }
        private decimal TotalBalance { get; set; }

        private int unturnedItemsCount;
        private int marketItemsCount;

        protected override async Task OnInitializedAsync() 
        {
            UnturnedItems = await HttpClient.GetJsonAsync<List<UnturnedItem>>("api/unturneditems");
            
            unturnedItemsCount = UnturnedItems.Count;
            marketItemsCount = UnturnedItems.Sum(x => x.MarketItemsCount);
            TotalBalance = await HttpClient.GetJsonAsync<decimal>("api/marketitems/balance/total");
        }
    }
}
