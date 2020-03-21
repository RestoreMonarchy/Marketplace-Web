using Marketplace.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http;
using System.Threading.Tasks;
using Marketplace.Client.Extensions;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components.Authorization;
using System.Collections.Generic;
using System.Net;
using Marketplace.Client.Services;

namespace Marketplace.Client.Pages
{
    public partial class ItemPage
    {
        [Parameter]
        public string ItemId { get; set; }

        [Inject]
        public HttpClient HttpClient { get; set; }
        [Inject]
        public OrderState OrderState { get; set; }

        private UnturnedItem UnturnedItem { get; set; }
        public List<MarketItem> PagedData { get; set; }

        protected override async Task OnInitializedAsync()
        {
            UnturnedItem = await HttpClient.GetJsonAsync<UnturnedItem>($"api/unturneditems/{ItemId}");
        }
    }
}