﻿using CurrieTechnologies.Razor.SweetAlert2;
using Marketplace.Client.Api;
using Marketplace.Client.Extensions;
using Marketplace.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Marketplace.Client.Pages
{
    [Authorize]
    public partial class TrunkPage
    {
        [Inject]
        public MarketItemsClient MarketItemsClient { get; set; }
        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        [Inject]
        public IJSRuntime JsRuntime { get; set; }
        [Inject]
        public SweetAlertService Swal { get; set; }
        private AuthenticationState state { get; set; }

        public IEnumerable<MarketItem> Items { get; set; }

        private IEnumerable<MarketItem> sellingItems => Items.Where(x => x.SellerId == state.User.Identity.Name && !x.IsSold).ToList();
        private IEnumerable<MarketItem> boughtItems => Items.Where(x => x.BuyerId == state.User.Identity.Name && !x.IsClaimed).ToList();
        private IEnumerable<MarketItem> historyItems => Items.Where(x => (x.IsSold && x.SellerId == state.User.Identity.Name) || (x.IsClaimed && x.BuyerId == state.User.Identity.Name)).ToList();

        private MarketItem infoItem;

        protected override async Task OnInitializedAsync()
        {
            state = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            Items = await MarketItemsClient.GetMyMarketItemsAsync();
        }

        public void ShowInfo(MarketItem marketItem)
        {
            infoItem = marketItem;
            JsRuntime.ToggleModal("infoModal");
        }

        public async Task ShowClaim(MarketItem item)
        {
            await Swal.FireAsync("Claim Information", $"To claim your {item.Item.ItemName}, use in-game: <code>/claim {item.Id}</code>", SweetAlertIcon.Info);
        }

        public async Task ChangePrice(MarketItem item)
        {

            await Swal.FireAsync(new SweetAlertOptions
            {
                Title = $"Change Price",
                Text = $"Input new price for listing {item.Id} [{item.Item.ItemName}]",
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
                    item.Price = price;
                    await MarketItemsClient.ChangePriceMarketItemAsync(item.Id, price);
                    await Swal.FireAsync("Price Changed", $"Successfully changed the price of listing {item.Id} [{item.Item.ItemName}] to {item.Price}!", SweetAlertIcon.Success);
                } else
                {
                    await Swal.FireAsync("Canceled", $"Changing price for listing {item.Id} canceled.", SweetAlertIcon.Error);
                }
            });            
        }
    }
}
