using Marketplace.Shared;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Marketplace.Client.Services
{
    public class PlayersService
    {
        private readonly HttpClient httpClient;
        public PlayersService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public UserInfo CurrentUserInfo { get; private set; }

        public async Task<UserInfo> GetCurrentUserAsync()
        {
            if (CurrentUserInfo == null)
                CurrentUserInfo = await httpClient.GetFromJsonAsync<UserInfo>("api/user");
            return CurrentUserInfo;
        }

        public async Task<UserInfo> GetPlayerUserAsync(string steamId)
        {
            return await httpClient.GetFromJsonAsync<UserInfo>("api/user/" + steamId);
        }
    }
}
