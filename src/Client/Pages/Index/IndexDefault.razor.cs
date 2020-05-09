using Marketplace.Client.Models;
using Marketplace.Client.Models.Filters;
using Marketplace.Client.Models.Filters.Orders;
using Marketplace.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Marketplace.Client.Pages.Index
{
    public partial class IndexDefault
    {
        [Inject]
        private HttpClient HttpClient { get; set; }

        private List<UnturnedItem> Items { get; set; }

        public FiltersData<UnturnedItem> Data { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Items = await HttpClient.GetFromJsonAsync<List<UnturnedItem>>("api/unturneditems");
            Data = new FiltersData<UnturnedItem>(Items, 20, true, new ShowOnlyOffersFilter(), new QuantityOrderFilter(), new ItemIdOrderFilter());
        }
    }
}