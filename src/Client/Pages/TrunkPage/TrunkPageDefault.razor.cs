using Marketplace.Client.Services;
using Marketplace.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Marketplace.Client.Pages.TrunkPage
{
    public partial class TrunkPageDefault
    {
        [Inject]
        private HttpClient HttpClient { get; set; }
        [Inject]
        private OrderState State { get; set; }
        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        private AuthenticationState state { get; set; }

        public IEnumerable<MarketItem> Items { get; set; }

        private IEnumerable<MarketItem> sellingItems => Items.Where(x => x.SellerId == state.User.Identity.Name && !x.IsSold);
        private IEnumerable<MarketItem> boughtItems => Items.Where(x => x.BuyerId == state.User.Identity.Name && !x.IsClaimed);
        private IEnumerable<MarketItem> historyItems => Items.Where(x => (x.IsSold && x.SellerId == state.User.Identity.Name) || (x.IsClaimed && x.BuyerId == state.User.Identity.Name));

        protected override async Task OnInitializedAsync()
        {
            state = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            Items = await HttpClient.GetJsonAsync<IEnumerable<MarketItem>>("api/marketitems/trunk");
        }
    }
}
