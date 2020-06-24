using Marketplace.Client.Services;
using Marketplace.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace Marketplace.Client.Pages.Account
{
    [Authorize]
    public partial class AccountPage
    {
        [Inject]
        private PlayersService PlayersService { get; set; }
        private UserInfo UserInfo { get; set; }

        protected override void OnInitialized()
        {
            UserInfo = PlayersService.CurrentUserInfo;
        }
    }
}
