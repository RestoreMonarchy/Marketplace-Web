using CurrieTechnologies.Razor.SweetAlert2;
using Marketplace.Client.Extensions;
using Marketplace.Client.Models;
using Marketplace.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Marketplace.Client.Services
{
    public class OrderState
    {
        private HttpClient HttpClient { get; set; }
        private IJSRuntime JsRuntime { get; set; }
        private AuthenticationStateProvider AuthenticationStateProvider  { get; set; }
        private SweetAlertService Swal { get; set; }

        public OrderState(HttpClient httpClient, IJSRuntime jsRuntime, AuthenticationStateProvider authenticationStateProvider, SweetAlertService swal)
        {
            HttpClient = httpClient;
            JsRuntime = jsRuntime;
            AuthenticationStateProvider = authenticationStateProvider;
            Swal = swal;
        }

        public bool ShowingPreviewModal { get; private set; } = false;
        public Order Order { get; set; }

        public void ShowPreviewModal(MarketItem marketItem, UnturnedItem unturnedItem = null)
        {
            if (unturnedItem != null)
                Order = new Order(marketItem, unturnedItem);
            else
                Order = new Order(marketItem, marketItem.Item);

            ShowingPreviewModal = true;
            JsRuntime.ToggleModal("infoModal");
        }

        public void CancelPreviewModal()
        {
            ShowingPreviewModal = false;
            Order = null;
            JsRuntime.ToggleModal("infoModal");
        }

        public async Task BuyMarketItem()
        {            
            if (!(await AuthenticationStateProvider.GetAuthenticationStateAsync()).User.Identity.IsAuthenticated)
            {
                await Swal.FireAsync("Purchase Error", $"You have to sign in to be able to buy!", SweetAlertIcon.Error);
                return;
            }

            var response = await HttpClient.PostAsync($"api/marketitems/{Order.MarketItem.Id}/buy", null);
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                await Swal.FireAsync("Purchase Error", "The item has already been sold or you can't afford it!", SweetAlertIcon.Error);
                return;
            }

            Order.MarketItem = null;
            await Swal.FireAsync("Purchase Success", $"You successfully bought {Order.UnturnedItem.ItemName}({Order.MarketItem.ItemId}) for ${Order.MarketItem.Price}!", SweetAlertIcon.Success);
        }
    }
}
