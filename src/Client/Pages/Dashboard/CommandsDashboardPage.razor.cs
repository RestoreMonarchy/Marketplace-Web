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
    public partial class CommandsDashboardPage
    {
        [Inject]
        private HttpClient HttpClient { get; set; }

        private List<Command> Commands { get; set; }

        private CommandModal FormModal { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Commands = await HttpClient.GetFromJsonAsync<List<Command>>("api/commands");
        }

        public async Task CreateCommandAsync()
        {
            await FormModal.CreateCommandAsync();
        }

        public async Task UpdateCommandAsync(Command command)
        {
            await FormModal.UpdateCommandAsync(command);
        }

        public async Task OnCommandCreated(Command command)
        {
            var response = await HttpClient.PostAsJsonAsync("api/commands", command);
            command.Id = await response.Content.ReadFromJsonAsync<int>();
            Commands.Add(command);
        }

        public async Task OnCommandUpdated(Command command)
        {
            await HttpClient.PutAsJsonAsync("api/commands", command);
        }
    }
}
