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
            Products = await HttpClient.GetJsonAsync<List<Product>>("api/products");
            Servers = await HttpClient.GetJsonAsync<List<Server>>("api/servers");
            Commands = await HttpClient.GetJsonAsync<List<Command>>("api/commands");
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
            product.Id = await HttpClient.PostJsonAsync<int>("api/products", product);
            Products.Add(product);
        }

        public async Task OnProductUpdatedAsync(Product product)
        {
            await HttpClient.PutJsonAsync("api/products", product);
        }
    }
}
