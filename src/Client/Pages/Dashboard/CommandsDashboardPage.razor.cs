using Marketplace.Client.Shared.Components.Modals;
using Marketplace.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Marketplace.Client.Pages.Dashboard
{
    public partial class CommandsDashboardPage
    {
        [Inject]
        private HttpClient HttpClient { get; set; }

        private List<Command> Commands { get; set; }

        private CommandModal FormModal { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Commands = await HttpClient.GetJsonAsync<List<Command>>("api/commands");
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
            command.Id = await HttpClient.PostJsonAsync<int>("api/commands", command);
            Commands.Add(command);
        }

        public async Task OnCommandUpdated(Command command)
        {
            await HttpClient.PutJsonAsync("api/commands", command);
        }
    }
}
