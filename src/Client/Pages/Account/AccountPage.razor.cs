using Marketplace.Client.Providers;
using Marketplace.Client.Shared.Components.Modals;
using Marketplace.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Marketplace.Client.Pages.Account
{
    public partial class AccountPage
    {
        [Inject]
        private HttpClient HttpClient { get; set; }
        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; }
        private UserInfo UserInfo { get; set; }

        public MarketItemModal Modal { get; set; }
        public IEnumerable<MarketItem> BuyerItems { get; set; }
        public IEnumerable<MarketItem> SellerItems { get; set; }

        protected override async Task OnInitializedAsync()
        {
            UserInfo = (AuthenticationStateProvider as SteamAuthenticationStateProvider).UserInfo;
            Console.WriteLine("should send below");
            Console.WriteLine(UserInfo.SteamName);
            Console.WriteLine(UserInfo.SteamAvatarUrl);
        }
    }
}
