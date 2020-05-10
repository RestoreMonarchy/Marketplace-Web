using CurrieTechnologies.Razor.SweetAlert2;
using Marketplace.Client.Services;
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
        private PlayersService PlayersService { get; set; }

        private IEnumerable<UnturnedItem> UnturnedItems { get; set; }
        private IEnumerable<Server> Servers { get; set; }
        private Dictionary<string, Setting> Settings { get; set; }
        
        private int unturnedItemsCount;
        private int marketItemsCount;
        private int connectedServersCount;

        private Setting indexLayout;
        private Setting itemPageLayout;
        private Setting productsLayout;

        private Setting steamDevKey;      
        private Setting apiKey;
        private Setting admins;

        protected override async Task OnInitializedAsync() 
        {            
            Settings = (await HttpClient.GetFromJsonAsync<List<Setting>>("api/settings")).ToDictionary(x => x.SettingId);
            UnturnedItems = await HttpClient.GetFromJsonAsync<IEnumerable<UnturnedItem>>("api/unturneditems");
            Servers = await HttpClient.GetFromJsonAsync<IEnumerable<Server>>("api/servers");

            if (PlayersService.CurrentUserInfo?.IsGlobalAdmin ?? false)
            {
                steamDevKey = Settings["SteamDevKey"];
                apiKey = Settings["APIKey"];
                admins = Settings["Admins"];
            }

            indexLayout = Settings["IndexLayout"];
            itemPageLayout = Settings["ItemPageLayout"];
            productsLayout = Settings["ProductsLayout"];

            unturnedItemsCount = UnturnedItems.Count();
            marketItemsCount = UnturnedItems.Sum(x => x.MarketItemsCount);
            connectedServersCount = Servers.Count(x => x.IsConnected);
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
