using CurrieTechnologies.Razor.SweetAlert2;
using Marketplace.Shared;
using Marketplace.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

        private List<UnturnedItem> UnturnedItems { get; set; }
        private decimal TotalBalance { get; set; }
        private Dictionary<string, Setting> Settings { get; set; }

        private int unturnedItemsCount;
        private int marketItemsCount;

        private Setting indexLayout;
        private Setting itemPageLayout;
        private Setting trunkLayout;
        private Setting productsLayout;
        private Setting steamDevKey;
        private Setting uconomyConnectionString;        
        private Setting apiKey;
        private Setting admins;
        private string adminValue = string.Empty;
        private List<string> adminList;

        protected override async Task OnInitializedAsync() 
        {
            UnturnedItems = await HttpClient.GetJsonAsync<List<UnturnedItem>>("api/unturneditems");
            TotalBalance = await HttpClient.GetJsonAsync<decimal>("api/uconomy/total");
            Settings = (await HttpClient.GetJsonAsync<List<Setting>>("api/settings")).ToDictionary(x => x.SettingId);
            
            indexLayout = Settings["IndexLayout"];
            itemPageLayout = Settings["ItemPageLayout"];
            trunkLayout = Settings["TrunkLayout"];
            productsLayout = Settings["ProductsLayout"];
            uconomyConnectionString = Settings["UconomyConnectionString"];
            steamDevKey = Settings["SteamDevKey"];
            apiKey = Settings["APIKey"];
            admins = Settings["Admins"];
            adminList = admins.SettingValue.Split(',').ToList();

            unturnedItemsCount = UnturnedItems.Count;
            marketItemsCount = UnturnedItems.Sum(x => x.MarketItemsCount);            
        }

        public async Task UpdateSettingAsync(string settingId)
        {
            await HttpClient.PutJsonAsync($"api/settings/{settingId}", Settings[settingId]);
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

        public void AddAdmin()
        {
            adminList.Add(adminValue);
            admins.SettingValue = string.Join(",", adminList);
        }

        public async Task RemoveAdmin(string admin)
        {
            adminList.Remove(admin);
            admins.SettingValue = string.Join(",", adminList);
            await UpdateSettingAsync("Admins");
        }
    }
}
