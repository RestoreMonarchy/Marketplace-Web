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
    public class MarketItemsService
    {
        private readonly HttpClient httpClient;
        private readonly IJSRuntime jsRuntime;
        private readonly PlayersService playersService;
        private readonly SweetAlertService swal;
        private readonly BalanceService balanceService;

        public MarketItemsService(HttpClient httpClient, IJSRuntime jsRuntime, PlayersService playersService, SweetAlertService swal, 
            BalanceService balanceService)
        {
            this.httpClient = httpClient;
            this.jsRuntime = jsRuntime;
            this.playersService = playersService;
            this.swal = swal;
            this.balanceService = balanceService;
        }


        public async Task BuyMarketItemAsync(MarketItem marketItem, Action<MarketItem> onSuccessfullBuy = null)
        {
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
                await swal.FireAsync("OK", $"You successfully bought {marketItem.Id} ({marketItem.ItemName}) for ${marketItem.Price} from {marketItem.SellerName}!", SweetAlertIcon.Success);
                await balanceService.UpdateBalanceAsync();
            }
        }
    }
}
