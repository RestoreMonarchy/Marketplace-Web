using Marketplace.Client.Services;
using Marketplace.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Marketplace.Client.Providers
{
    public class SteamAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly PlayersService playersService;

        public SteamAuthenticationStateProvider(PlayersService playersService)
        {
            this.playersService = playersService;
        }

        public UserInfo UserInfo { get; set; }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            UserInfo = await playersService.GetCurrentUserAsync();
            ClaimsIdentity steamIdentity;

            if (UserInfo.IsAuthenticated)
            {                
                steamIdentity = new ClaimsIdentity(new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, UserInfo.SteamId),
                    new Claim(ClaimTypes.Role, UserInfo.Role),
                    new Claim("IsGlobalAdmin", UserInfo.IsGlobalAdmin.ToString())
                }, "SteamAuth");
            }
            else
            {
                steamIdentity = new ClaimsIdentity();
            }
            
            return new AuthenticationState(new ClaimsPrincipal(steamIdentity));
        }

        public void NotifyAuthenticationStateChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
