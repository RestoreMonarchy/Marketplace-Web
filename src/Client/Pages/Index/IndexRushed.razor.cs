using Marketplace.Client.Models;
using Marketplace.Client.Models.Filters.Orders;
using Marketplace.Client.Services;
using Marketplace.Client.Shared.Components.Modals;
using Marketplace.Shared;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Marketplace.Client.Pages.Index
{
    public partial class IndexRushed
    {
        [Inject]
        private HttpClient HttpClient { get; set; }
        [Inject]
        private MarketItemsService MarketItemsService { get; set; }

        private MarketItemModal Modal { get; set; }

        private List<MarketItem> MarketItems { get; set; }
        private FiltersData<MarketItem> FiltersData { get; set; }

        protected override async Task OnInitializedAsync()
        {
            MarketItems = (await HttpClient.GetJsonAsync<List<MarketItem>>("api/marketitems")).ToList();
            FiltersData = new FiltersData<MarketItem>(MarketItems, 10, true, new LatestOrderFilter(), new PriceOrderFilter());
        }
    }
}