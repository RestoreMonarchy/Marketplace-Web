using Marketplace.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Marketplace.Client.Providers
{
    public class SteamAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient httpClient;

        public SteamAuthenticationStateProvider(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public AuthenticationState State { get; set; }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var userInfo = await httpClient.GetJsonAsync<UserInfo>("api/authentication");
            ClaimsIdentity steamIdentity;

            if (userInfo.IsAuthenticated)
            {                
                steamIdentity = new ClaimsIdentity(new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, userInfo.SteamId),
                    new Claim(ClaimTypes.Role, userInfo.Role),
                    new Claim("IsGlobalAdmin", userInfo.IsGlobalAdmin.ToString())
                }, "SteamAuth");
            }
            else
            {
                steamIdentity = new ClaimsIdentity();
            }

            State = new AuthenticationState(new ClaimsPrincipal(steamIdentity));
            return State;
        }

        public void NotifyAuthenticationStateChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
