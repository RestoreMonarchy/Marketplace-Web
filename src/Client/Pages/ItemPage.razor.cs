using Marketplace.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http;
using System.Threading.Tasks;
using Marketplace.Client.Extensions;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components.Authorization;
using System.Collections.Generic;
using Marketplace.Client.Api;
using System.Net;

namespace Marketplace.Client.Pages
{
    public partial class ItemPage
    {
        [Parameter]
        public ushort ItemId { get; set; }

        [Inject]
        public UnturnedItemsClient UnturnedItemsClient { get; set; }
        [Inject]
        public MarketItemsClient MarketItemsClient { get; set; }
        [Inject]
        public IJSRuntime JsRuntime { get; set; }
        [Inject]
        public SweetAlertService Swal { get; set; }
        [Inject]
        private AuthenticationStateProvider authenticationStateProvider { get; set; }
        public AuthenticationState authenticationState { get; set; }

        public UnturnedItem Item { get; set; }
        public IEnumerable<MarketItem> PagedData { get; set; }
        private MarketItem listing;

        protected override async Task OnInitializedAsync()
        {
            authenticationState = await authenticationStateProvider.GetAuthenticationStateAsync();
            Item = await UnturnedItemsClient.GetUnturnedItemAsync(ItemId);
        }

        public void PreviewListing(MarketItem listing)
        {
            this.listing = listing;
            listing.Item = Item;
            JsRuntime.ToggleModal("infoModal");
        }

        public async Task BuyListing(MarketItem listing)
        {
            JsRuntime.ToggleModal("infoModal");
            if (!authenticationState.User.Identity.IsAuthenticated)
            {
                await Swal.FireAsync("Purchase Error", $"You have to sign in to be able to buy!", SweetAlertIcon.Error);
                return;
            }
            try
            {
                var item = await MarketItemsClient.TryBuyMarketItemAsync(ItemId);
                Item.MarketItems.Remove(listing);
                await Swal.FireAsync("Purchase Success", $"You successfully bought {Item.ItemName}({listing.ItemId}) for ${listing.Price}!", SweetAlertIcon.Success);

            }
            catch (ApiException ex) when (ex.StatusCode == (int)HttpStatusCode.BadRequest)
            {
                await Swal.FireAsync("Purchase Error", "The item has already been sold or you can't afford it!", SweetAlertIcon.Error);
            }


        }
    }
}