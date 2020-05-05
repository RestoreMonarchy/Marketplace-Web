using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Marketplace.Client.Services
{
    public class BalanceService
    {
        private readonly HttpClient httpClient;
        private readonly SweetAlertService swal;

        public BalanceService(HttpClient httpClient, SweetAlertService swal)
        {
            this.httpClient = httpClient;
            this.swal = swal;
        }

        public string BalanceMessage { get; private set; }
        public decimal? Balance { get; private set; }

        public async Task UpdateBalanceAsync()
        {
            var response = await httpClient.GetAsync("api/economy");

            if (response.StatusCode == HttpStatusCode.GatewayTimeout)
            {
                BalanceMessage = "Servers disconnected";
            } else
            {
                Balance = decimal.Parse(await response.Content.ReadAsStringAsync());
            }
        }
    }
}
