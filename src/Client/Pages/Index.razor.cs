using Marketplace.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Marketplace.Client.Pages
{
    public partial class Index
    {
        [Inject]
        private HttpClient HttpClient { get; set; }

        private IEnumerable<UnturnedItem> items;

        private IEnumerable<UnturnedItem> filteredItems => items.Where(x => !showOnlyWithOffers || x.MarketItemsCount > 0).OrderByDescending(x => x.MarketItemsCount).ToList();
        private IEnumerable<UnturnedItem> searchItems => filteredItems.Where(x => x.ItemId.ToString()
            .Equals(searchString) || x.ItemName.ToLower().Contains(searchString)).ToList();

        string searchString = string.Empty;

        bool showOnlyWithOffers = true;

        protected override async Task OnInitializedAsync()
        {
            items = await HttpClient.GetJsonAsync<IEnumerable<UnturnedItem>>("api/unturneditems");
        }

        void ChangeShowAll()
        {
            showOnlyWithOffers = !showOnlyWithOffers;
        }
    }
}
