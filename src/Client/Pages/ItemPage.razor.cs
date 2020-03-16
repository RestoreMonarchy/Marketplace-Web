using Marketplace.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http;
using System.Threading.Tasks;
using Marketplace.Client.Extensions;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components.Authorization;
using System.Collections.Generic;
using System.Net;

namespace Marketplace.Client.Pages
{
    public partial class ItemPage
    {
        [Parameter]
        public string ItemId { get; set; }

        [Inject]
        public HttpClient HttpClient { get; set; }
        [Inject]
        public IJSRuntime JsRuntime { get; set; }
        [Inject]
        public SweetAlertService Swal { get; set; }
        [Inject]
        private AuthenticationStateProvider authenticationStateProvider { get; set; }
        public AuthenticationState authenticationState { get; set; }

        public UnturnedItem Item { get; set; }
        public List<MarketItem> PagedData { get; set; }
        private MarketItem listing;

        protected override async Task OnInitializedAsync()
        {
            authenticationState = await authenticationStateProvider.GetAuthenticationStateAsync();
            Item = await HttpClient.GetJsonAsync<UnturnedItem>($"api/unturneditems/{ItemId}");
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

            var item = await HttpClient.PostAsync($"api/marketitems/{listing.Id}/buy", null);
            if (item.StatusCode == HttpStatusCode.BadRequest)
            {
                await Swal.FireAsync("Purchase Error", "The item has already been sold or you can't afford it!", SweetAlertIcon.Error);
                return;
            }

            Item.MarketItems.Remove(listing);
            PagedData.Remove(listing);
            await Swal.FireAsync("Purchase Success", $"You successfully bought {Item.ItemName}({listing.ItemId}) for ${listing.Price}!", SweetAlertIcon.Success);
        }
    }
}