using Marketplace.Client.Models;
using Marketplace.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Marketplace.Client.Pages.Index
{
    public partial class IndexDefault
    {
        [Inject]
        private HttpClient HttpClient { get; set; }

        private IEnumerable<UnturnedItem> Items { get; set; }

        private IEnumerable<UnturnedItem> FilteredItems => Items.Where(x => !showOnlyWithOffers || x.MarketItemsCount > 0).OrderByDescending(x => x.MarketItemsCount).ToList();

        bool showOnlyWithOffers = true;
        public FiltersData<UnturnedItem> Data { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Items = await HttpClient.GetJsonAsync<IEnumerable<UnturnedItem>>("api/unturneditems");
            Data = new FiltersData<UnturnedItem>(Items, 0);
        }

        void ChangeShowAll()
        {
            showOnlyWithOffers = !showOnlyWithOffers;
        }
    }
}
