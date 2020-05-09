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
    public partial class ProductsDashboardPage
    {
        [Inject]
        private HttpClient HttpClient { get; set; }

        private ProductModal FormModal { get; set; }
        private List<Product> Products { get; set; }

        private List<Server> Servers { get; set; }
        private List<Command> Commands { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Products = await HttpClient.GetFromJsonAsync<List<Product>>("api/products");
            Servers = await HttpClient.GetFromJsonAsync<List<Server>>("api/servers");
            Commands = await HttpClient.GetFromJsonAsync<List<Command>>("api/commands");
        }

        public async Task CreateProductAsync()
        {
            await FormModal.CreateProductAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            await FormModal.UpdateProductAsync(product);
        }

        public async Task OnProductCreatedAsync(Product product)
        {
            var response = await HttpClient.PostAsJsonAsync("api/products", product);
            product.Id = await response.Content.ReadFromJsonAsync<int>();
            Products.Add(product);
        }

        public async Task OnProductUpdatedAsync(Product product)
        {
            await HttpClient.PutAsJsonAsync("api/products", product);
        }
    }
}
