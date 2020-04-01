using CurrieTechnologies.Razor.SweetAlert2;
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
        [Inject]
        public SweetAlertService Swal { get; set; }

        private List<Product> Products { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Products = await HttpClient.GetJsonAsync<List<Product>>("api/products");
        }
    }
}
