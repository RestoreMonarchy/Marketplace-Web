using Marketplace.Client.Models;
using Marketplace.Client.Services;
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
        public HttpClient HttpClient { get; set; }
        [Inject]
        public OrderState OrderState { get; set; }

        private FiltersData<MarketItem> Data { get; set; }
        private UnturnedItem UnturnedItem { get; set; }

        protected override async Task OnInitializedAsync()
        {
            UnturnedItem = await HttpClient.GetJsonAsync<UnturnedItem>($"api/unturneditems/{ItemId}");
            UnturnedItem.MarketItems.ToList().ForEach(x => x.Item = UnturnedItem);
            Data = new FiltersData<MarketItem>(UnturnedItem.MarketItems, 15, false);
        }
    }
}
