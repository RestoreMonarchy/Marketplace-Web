using CurrieTechnologies.Razor.SweetAlert2;
using Marketplace.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Marketplace.Client.Pages.Dashboard
{
    [Authorize(Roles = "Admin")]
    public partial class DashboardPage
    {
        [Inject]
        public HttpClient HttpClient { get; set; }
        [Inject]
        public SweetAlertService Swal { get; set; }
        private List<UnturnedItem> UnturnedItems { get; set; }
        private decimal TotalBalance { get; set; }
        private Dictionary<string, Setting> Settings { get; set; }

        private int unturnedItemsCount;
        private int marketItemsCount;

        private Setting indexLayout;

        protected override async Task OnInitializedAsync() 
        {
            UnturnedItems = await HttpClient.GetJsonAsync<List<UnturnedItem>>("api/unturneditems");
            TotalBalance = await HttpClient.GetJsonAsync<decimal>("api/marketitems/balance/total");
            Settings = (await HttpClient.GetJsonAsync<List<Setting>>("api/settings")).ToDictionary(x => x.SettingId);
            
            indexLayout = Settings["IndexLayout"];


            unturnedItemsCount = UnturnedItems.Count;
            marketItemsCount = UnturnedItems.Sum(x => x.MarketItemsCount);            
        }

        public async Task UpdateSettingAsync(string settingId)
        {
            await HttpClient.PutJsonAsync($"api/settings/{settingId}", Settings[settingId]);
        }        
    }
}
