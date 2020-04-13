using Marketplace.Client.Shared.Components.Modals;
using Marketplace.Shared;
using Marketplace.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Net.Http;
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
            Servers = await HttpClient.GetJsonAsync<List<Server>>("api/servers");
        }

        public async Task CreateServerAsync()
        {
            await FormModal.CreateServerAsync();
        }

        public async Task UpdateServerAsync(Server command)
        {
            await FormModal.UpdateServerAsync(command);
        }

        public async Task OnServerCreated(Server command)
        {
            command.Id = await HttpClient.PostJsonAsync<int>("api/servers", command);
            Servers.Add(command);
        }

        public async Task OnServerUpdated(Server command)
        {
            await HttpClient.PutJsonAsync("api/servers", command);
        }
    }
}
