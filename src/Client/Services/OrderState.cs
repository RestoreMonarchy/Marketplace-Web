using CurrieTechnologies.Razor.SweetAlert2;
using Marketplace.Shared;
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
        private readonly HttpClient httpClient;
        private readonly IJSRuntime jsRuntime;
        private readonly AuthenticationStateProvider authenticationStateProvider;
        private readonly SweetAlertService swal;
        private readonly BalanceService balanceService;

        public OrderState(HttpClient httpClient, IJSRuntime jsRuntime, AuthenticationStateProvider authenticationStateProvider, 
            SweetAlertService swal, BalanceService balanceService)
        {
            this.httpClient = httpClient;
            this.jsRuntime = jsRuntime;
            this.authenticationStateProvider = authenticationStateProvider;
            this.swal = swal;
            this.balanceService = balanceService;
        }

        public MarketItem InfoItem { get; set; }

        public async Task ShowInfoModalAsync(MarketItem marketItem)
        {
            InfoItem = marketItem;
            await jsRuntime.InvokeVoidAsync("ToggleModal", "infoModal");
        }

        public async Task CloseInfoModalAsync()
        {
            await jsRuntime.InvokeVoidAsync("ToggleModal", "infoModal");
            InfoItem = null;
        }

        public async Task BuyMarketItemAsync(MarketItem marketItem, Action<MarketItem> onSuccessfullBuy = null)
        {            
            if (!(await authenticationStateProvider.GetAuthenticationStateAsync()).User.Identity.IsAuthenticated)
            {
                await swal.FireAsync("Unauthorized", $"You have to sign in to be able to buy!", SweetAlertIcon.Error);
                return;
            }

            var response = await httpClient.PostAsync($"api/marketitems/{marketItem.Id}/buy", null);
            
            switch (response.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    await swal.FireAsync("Not Found", "The item you were trying to buy could not be found", SweetAlertIcon.Error);
                    break;
                case HttpStatusCode.Forbidden:
                    await swal.FireAsync("Forbidden", "You are the seller of this item", SweetAlertIcon.Error);
                    break;
                case HttpStatusCode.Gone:
                    await swal.FireAsync("No Content", "The item you are trying to buy was already sold", SweetAlertIcon.Error);
                    break;
                case HttpStatusCode.BadRequest:
                    await swal.FireAsync("Bad Request", "You cannot afford buying this item", SweetAlertIcon.Error);
                    break;
                case HttpStatusCode.Conflict:
                    await swal.FireAsync("Conflict", "You have already reached the maximum number of active shoppings", SweetAlertIcon.Error);
                    break;
                case HttpStatusCode.ServiceUnavailable:
                    await swal.FireAsync("Service Unavailable", "Failed to communicate with game server, try again later", SweetAlertIcon.Error);
                    break;
                case HttpStatusCode.InternalServerError:
                    await swal.FireAsync("Internal Server Error", "Try again later", SweetAlertIcon.Error);
                    break;
                case HttpStatusCode.Unauthorized:
                    await swal.FireAsync("Unauthorized", "You have to sign in to be able to buy", SweetAlertIcon.Error);
                    break;
            }

            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (onSuccessfullBuy != null)
                {
                    onSuccessfullBuy.Invoke(marketItem);
                }
                await swal.FireAsync("OK", $"You successfully bought {marketItem.ItemName}({marketItem.ItemId}) for ${marketItem.Price}!", SweetAlertIcon.Success);
                await balanceService.UpdateBalanceAsync();
            }
            
            await CloseInfoModalAsync();
        }

        public async Task ChangePrice(MarketItem item)
        {
            await swal.FireAsync(new SweetAlertOptions
            {
                Title = $"Change Price",
                Text = $"Input new price for listing {item.Id} [{item.ItemName}]",
                Icon = SweetAlertIcon.Warning,
                Input = SweetAlertInputType.Number,
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
                    var response = await httpClient.SendAsync(msg);
                    item.Price = price;
                    await swal.FireAsync("Success", $"Successfully changed the price of listing {item.Id} [{item.ItemName}] to {price}!", SweetAlertIcon.Success);
                }
                else
                {
                    await swal.FireAsync("Cancel", $"Changing price for listing {item.Id} canceled.", SweetAlertIcon.Error);
                }
            });
        }

        public async Task ShowClaim(MarketItem item)
        {
            await swal.FireAsync("Claim Information", $"To claim your {item.ItemName}, use in-game: <code>/claim {item.Id}</code>", SweetAlertIcon.Info);
        }
    }
}
