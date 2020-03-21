using CurrieTechnologies.Razor.SweetAlert2;
using Marketplace.Client.Extensions;
using Marketplace.Client.Models;
using Marketplace.Client.Services;
using Marketplace.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Marketplace.Client.Pages.Index
{
    public partial class IndexRushed
    {
        [Inject]
        private HttpClient HttpClient { get; set; }
        [Inject]
        private OrderState OrderState { get; set; }

        private List<MarketItem> MarketItems { get; set; }
        private FiltersData<MarketItem> FiltersData { get; set; }


        protected override async Task OnInitializedAsync()
        {
            MarketItems = (await HttpClient.GetJsonAsync<List<MarketItem>>("api/marketitems")).OrderByDescending(x => x.CreateDate).ToList();
            FiltersData = new FiltersData<MarketItem>(MarketItems, 10);
        }
    }
}