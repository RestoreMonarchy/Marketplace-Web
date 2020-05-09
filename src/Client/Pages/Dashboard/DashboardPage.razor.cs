﻿using CurrieTechnologies.Razor.SweetAlert2;
using Marketplace.Shared;
using Marketplace.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Marketplace.Client.Pages.Dashboard
{
    [Authorize(Roles = RoleConstants.AdminRoleId)]
    public partial class DashboardPage
    {
        [Inject]
        private HttpClient HttpClient { get; set; }
        [Inject]
        private SweetAlertService Swal { get; set; }
        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        private List<UnturnedItem> UnturnedItems { get; set; }
        //private decimal TotalBalance { get; set; }
        private Dictionary<string, Setting> Settings { get; set; }

        private int unturnedItemsCount;
        private int marketItemsCount;

        private Setting indexLayout;
        private Setting itemPageLayout;
        private Setting trunkLayout;
        private Setting productsLayout;

        private Setting steamDevKey;
        private Setting economyConnectionString;        
        private Setting apiKey;
        private Setting admins;
        private Setting economyProvider;

        private bool isGlobalAdmin;

        protected override async Task OnInitializedAsync() 
        {
            UnturnedItems = await HttpClient.GetFromJsonAsync<List<UnturnedItem>>("api/unturneditems");
            Settings = (await HttpClient.GetFromJsonAsync<List<Setting>>("api/settings")).ToDictionary(x => x.SettingId);

            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            isGlobalAdmin = bool.Parse(authState.User.FindFirst("IsGlobalAdmin").Value);

            if (isGlobalAdmin)
            {
                economyConnectionString = Settings["UconomyConnectionString"];
                steamDevKey = Settings["SteamDevKey"];
                apiKey = Settings["APIKey"];
                admins = Settings["Admins"];
                economyProvider = Settings["EconomyProvider"];
            }

            indexLayout = Settings["IndexLayout"];
            itemPageLayout = Settings["ItemPageLayout"];
            trunkLayout = Settings["TrunkLayout"];
            productsLayout = Settings["ProductsLayout"];

            unturnedItemsCount = UnturnedItems.Count;
            marketItemsCount = UnturnedItems.Sum(x => x.MarketItemsCount);            
        }

        public async Task UpdateSettingAsync(string settingId)
        {
            await HttpClient.PutAsJsonAsync($"api/settings/{settingId}", Settings[settingId]);
            await Swal.FireAsync(new SweetAlertOptions($"Successfully updated {settingId} to {Settings[settingId].SettingValue}!") 
            { 
                Position = SweetAlertPosition.TopEnd,
                Icon = SweetAlertIcon.Success,
                ShowConfirmButton = false,
                Timer = 1000
            });
        }
        
        public void GenerateApiKey()
        {
            apiKey.SettingValue = Guid.NewGuid().ToString("N");
        }
    }
}
