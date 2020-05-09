using Marketplace.Client.Models;
using Marketplace.Client.Services;
using Marketplace.Client.Shared.Components.Modals;
using Marketplace.Shared;
using Microsoft.AspNetCore.Components;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Marketplace.Client.Pages.ItemPage
{
    public partial class ItemPageDefault
    {
        [Parameter]
        public string ItemId { get; set; }

        [Inject]
        private HttpClient HttpClient { get; set; }
        [Inject]
        private MarketItemsService MarketItemsService { get; set; }

        private MarketItemModal Modal { get; set; }
        private FiltersData<MarketItem> FiltersData { get; set; }
        private UnturnedItem UnturnedItem { get; set; }

        protected override async Task OnInitializedAsync()
        {
            UnturnedItem = await HttpClient.GetJsonAsync<UnturnedItem>($"api/unturneditems/{ItemId}");
            UnturnedItem.MarketItems.ToList().ForEach(x => x.Item = UnturnedItem);
            FiltersData = new FiltersData<MarketItem>(UnturnedItem.MarketItems, 10, false);
        }
    }
}
