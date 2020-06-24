using Marketplace.Client.Shared.Components.Modals;
using Marketplace.Shared;
using Marketplace.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Marketplace.Client.Pages.Dashboard
{
    [Authorize(Roles = RoleConstants.AdminRoleId)]
    public partial class ServersDashboardPage
    {
        [Inject]
        private HttpClient HttpClient { get; set; }

        private List<Server> Servers { get; set; }

        private ServerModal FormModal { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Servers = await HttpClient.GetFromJsonAsync<List<Server>>("api/servers");
        }

        public async Task CreateServerAsync()
        {
            await FormModal.CreateServerAsync();
        }

        public async Task UpdateServerAsync(Server server)
        {
            await FormModal.UpdateServerAsync(server);
        }

        public async Task OnServerCreated(Server server)
        {
            var response = await HttpClient.PostAsJsonAsync("api/servers", server);
            server = await response.Content.ReadFromJsonAsync<Server>();
            Servers.Add(server);
        }

        public async Task OnServerUpdated(Server command)
        {
            await HttpClient.PutAsJsonAsync("api/servers", command);
        }
    }
}
