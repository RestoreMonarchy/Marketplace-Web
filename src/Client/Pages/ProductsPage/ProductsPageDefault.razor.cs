using CurrieTechnologies.Razor.SweetAlert2;
using Marketplace.Client.Events;
using Marketplace.Client.Services;
using Marketplace.Client.Shared.Components.Modals;
using Marketplace.Shared;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Marketplace.Client.Pages.ProductsPage
{
    public partial class ProductsPageDefault
    {
        [Inject]
        private HttpClient HttpClient { get; set; }
        [Inject]
        private SweetAlertService Swal { get; set; }
        [Inject]
        private BalanceService BalanceService { get; set; }

        public ProductPreviewModal PreviewModal { get; set; }
        private IEnumerable<Product> Products { get; set; }
        private IEnumerable<ProductTransaction> Transactions { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Products = await HttpClient.GetJsonAsync<IEnumerable<Product>>("api/products");
            Transactions = await HttpClient.GetJsonAsync<IEnumerable<ProductTransaction>>("api/products/Transactions");
        }

        public async Task BuyProductAsync(BuyProductEventArgs args)
        {
            var response = await HttpClient.PostAsync($"api/products/{args.Product.Id}/buy?serverId={args.SelectedServer.Id}", null);

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    await Swal.FireAsync("OK", $"You successfully bought {args.Product.Title} for ${args.Product.Price}!", SweetAlertIcon.Success);
                    break;
                case HttpStatusCode.NotFound:
                    await Swal.FireAsync("Not Found", "The product you are trying to buy could not be found", SweetAlertIcon.Error);
                    break;
                case HttpStatusCode.NoContent:
                    await Swal.FireAsync("No Content", "The product you are trying to buy ios disabled", SweetAlertIcon.Error);
                    break;
                case HttpStatusCode.BadRequest:
                    await Swal.FireAsync("Bad Request", "You cannot afford buying this product", SweetAlertIcon.Error);
                    break;
                case HttpStatusCode.Conflict:   
                    await Swal.FireAsync("Conflict", "You have already bought maximum amount of this product", SweetAlertIcon.Error);
                    break;
                case HttpStatusCode.Unauthorized:
                    await Swal.FireAsync("Unauthorized", "You have to sign in to be able to buy", SweetAlertIcon.Error);
                    break;
            }

            await PreviewModal.ToggleModalAsync();
            await BalanceService.UpdateBalanceAsync();
        }
    }
}
