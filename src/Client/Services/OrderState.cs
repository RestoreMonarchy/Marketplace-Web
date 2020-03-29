using CurrieTechnologies.Razor.SweetAlert2;
using Marketplace.Client.Models;
using Marketplace.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System;
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

        public MarketItem InfoItem { get; set; }

        public async Task ShowInfoModalAsync(MarketItem marketItem)
        {
            InfoItem = marketItem;
            await JsRuntime.InvokeVoidAsync("ToggleModal", "infoModal");
        }

        public async Task CloseInfoModalAsync()
        {
            await JsRuntime.InvokeVoidAsync("ToggleModal", "infoModal");
            InfoItem = null;
        }

        public async Task BuyMarketItemAsync(MarketItem marketItem, Action<MarketItem> onSuccessfullBuy = null)
        {            
            if (!(await AuthenticationStateProvider.GetAuthenticationStateAsync()).User.Identity.IsAuthenticated)
            {
                await Swal.FireAsync("Unauthorized", $"You have to sign in to be able to buy!", SweetAlertIcon.Error);
                return;
            }

            var response = await HttpClient.PostAsync($"api/marketitems/{marketItem.Id}/buy", null);
            
            switch (response.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    await Swal.FireAsync("Not Found", "The item you were trying to buy could not be found", SweetAlertIcon.Error);
                    break;
                case HttpStatusCode.Unauthorized:
                    await Swal.FireAsync("Unauthorized", "You are the seller of this item", SweetAlertIcon.Error);
                    break;
                case HttpStatusCode.NoContent:
                    await Swal.FireAsync("No Content", "The item you are trying to buy was already sold", SweetAlertIcon.Error);
                    break;
                case HttpStatusCode.BadRequest:
                    await Swal.FireAsync("Bad Request", "You cannot afford buying this item", SweetAlertIcon.Error);
                    break;
                case HttpStatusCode.Conflict:
                    await Swal.FireAsync("Conflict", "You have already reached the maximum number of active shoppings", SweetAlertIcon.Error);
                    break;
            }

            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (onSuccessfullBuy != null)
                {
                    onSuccessfullBuy.Invoke(marketItem);
                }
                await Swal.FireAsync("OK", $"You successfully bought {marketItem.ItemName}({marketItem.ItemId}) for ${marketItem.Price}!", SweetAlertIcon.Success);
            }
            
            await CloseInfoModalAsync();
        }

        public async Task ChangePrice(MarketItem item)
        {
            await Swal.FireAsync(new SweetAlertOptions
            {
                Title = $"Change Price",
                Text = $"Input new price for listing {item.Id} [{item.ItemName}]",
                Icon = SweetAlertIcon.Warning,
                Input = SweetAlertInputType.Number,
                //InputPlaceholder = item.Price.ToString(),
                ShowCancelButton = true,
                ConfirmButtonText = "Submit",
                ShowLoaderOnConfirm = true,
            }).ContinueWith(async (swalTask) =>
            {
                var result = swalTask.Result;
                if (!string.IsNullOrEmpty(result.Value) && decimal.TryParse(result.Value, out decimal price))
                {   
                    var msg = new HttpRequestMessage(new HttpMethod("PATCH"), $"api/marketitems/{item.Id}");
                    msg.Content = new StringContent(price.ToString());
                    var response = await HttpClient.SendAsync(msg);
                    item.Price = price;
                    await Swal.FireAsync("Success", $"Successfully changed the price of listing {item.Id} [{item.ItemName}] to {price}!", SweetAlertIcon.Success);
                }
                else
                {
                    await Swal.FireAsync("Cancel", $"Changing price for listing {item.Id} canceled.", SweetAlertIcon.Error);
                }
            });
        }

        public async Task ShowClaim(MarketItem item)
        {
            await Swal.FireAsync("Claim Information", $"To claim your {item.ItemName}, use in-game: <code>/claim {item.Id}</code>", SweetAlertIcon.Info);
        }
    }
}
