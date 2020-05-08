﻿using CurrieTechnologies.Razor.SweetAlert2;
using Marketplace.Client.Models;
using Marketplace.Client.Models.Filters.Toggles;
using Marketplace.Client.Services;
using Marketplace.Client.Shared.Components.Modals;
using Marketplace.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Marketplace.Client.Pages.Account
{
    public partial class MarketItemsPage
    {
        [Inject]
        private HttpClient HttpClient { get; set; }
        [Inject]
        private PlayersService PlayersService { get; set; }
        [Inject]
        private SweetAlertService Swal { get; set; }

        public FiltersData<MarketItem> BuyerData { get; set; }
        public FiltersData<MarketItem> SellerData { get; set; }
        public MarketItemModal Modal { get; set; }

        private IEnumerable<MarketItem> BuyerItems { get; set; }
        private IEnumerable<MarketItem> SellerItems { get; set; }

        protected override async Task OnInitializedAsync()
        {
            BuyerItems = await HttpClient.GetJsonAsync<IEnumerable<MarketItem>>("api/marketitems/buyer");
            SellerItems = await HttpClient.GetJsonAsync<IEnumerable<MarketItem>>("api/marketitems/seller");
            BuyerData = new FiltersData<MarketItem>(BuyerItems.ToList(), 10, true, new ShowOnlyClaimedFilter());
            SellerData = new FiltersData<MarketItem>(SellerItems.ToList(), 10, true, new ShowOnlyNotSoldFilter());
        }

        private async Task ShowClaimAsync(MarketItem marketItem)
        {
            await Swal.FireAsync("Claim Information", $"To claim {marketItem.Id} ({marketItem.ItemName}) " +
                $"use in-game: <code>/claim {marketItem.Id}</code>", SweetAlertIcon.Info);
        }

        private async Task ConfirmRemoveItemAsync(MarketItem marketItem)
        {
            await Swal.FireAsync(new SweetAlertOptions()
            {
                Title = "Are you sure?",
                Text = $"{marketItem.Id} ({marketItem.ItemName}) will be moved to your buyings",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Yes, remove it!"
            }).ContinueWith(async (swalTask) => 
            {
                if (Convert.ToBoolean(swalTask.Result.Value))
                {
                    await RemoveItemAsync(marketItem);
                }
            });
        }

        private async Task RemoveItemAsync(MarketItem marketItem)
        {
            var response = await HttpClient.PutAsync("api/marketitems/" + marketItem.Id, null);

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    marketItem.IsSold = true;
                    marketItem.BuyerId = PlayersService.CurrentUserInfo.SteamId;
                    marketItem.SoldDate = DateTime.UtcNow;
                    BuyerData.AddToOrigin(marketItem);
                    await Swal.FireAsync("OK", $"Successfully moved {marketItem.Id} ({marketItem.ItemName}) to your buyings!",
                        SweetAlertIcon.Success);
                    break;
                case HttpStatusCode.MethodNotAllowed:
                    await Swal.FireAsync("Method Not Allowed", "Taking down items is disabled", SweetAlertIcon.Error);
                    break;
                case HttpStatusCode.NotFound:
                    await Swal.FireAsync("Not Found", $"Failed to find item {marketItem.Id} ({marketItem.ItemName})",
                        SweetAlertIcon.Error);
                    break;
                case HttpStatusCode.Gone:
                    await Swal.FireAsync("Not Found", $"Failed to find item {marketItem.Id} ({marketItem.ItemName})",
                       SweetAlertIcon.Error);
                    break;
                case HttpStatusCode.Forbidden:
                    await Swal.FireAsync("Forbidden", $"You are not a seller of this item", SweetAlertIcon.Error);
                    break;
                case HttpStatusCode.InternalServerError:
                    await Swal.FireAsync("Internal Server Error", $"There was an error, try again later", SweetAlertIcon.Error);
                    break;
            }
        }
    }
}