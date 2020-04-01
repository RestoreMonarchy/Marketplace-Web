using CurrieTechnologies.Razor.SweetAlert2;
using Marketplace.Client.Shared.Components.Modals;
using Marketplace.Shared;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Marketplace.Client.Pages.Dashboard
{
    public partial class ServersPage
    {
        [Inject]
        private HttpClient HttpClient { get; set; }
        [Inject]
        public SweetAlertService Swal { get; set; }

        private List<Server> Servers { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Servers = await HttpClient.GetJsonAsync<List<Server>>("api/servers");
        }

        public async Task ToggleServerAsync(Server server)
        {
            server.Enabled = !server.Enabled;
            await HttpClient.PutAsync($"api/servers/{server.Id}", null);
        }

        public async Task DeleteServerAsync(Server server)
        {
            var result = await Swal.FireAsync(new SweetAlertOptions 
            { 
                Title = "Are you sure?",
                Text = "You won't be able to revert this!",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = $"Yes, delete {server.ServerName}"
            });
            if (bool.Parse(result.Value))
            {
                await HttpClient.DeleteAsync($"api/servers/{server.Id}");
                Servers.Remove(server);
            }
        }
    }
}
