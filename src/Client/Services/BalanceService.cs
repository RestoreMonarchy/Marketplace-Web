using CurrieTechnologies.Razor.SweetAlert2;
using System;
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

        public event Action OnChange;
        private void NotifyStateChanged() => OnChange?.Invoke();

        public async Task UpdateBalanceAsync()
        {
            Balance = null;
            var response = await httpClient.GetAsync("api/economy");

            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    Balance = Convert.ToDecimal(await response.Content.ReadAsStringAsync());
                    break;
                case HttpStatusCode.GatewayTimeout:
                    BalanceMessage = "Servers timeout";
                    break;
                case HttpStatusCode.ServiceUnavailable:
                    BalanceMessage = "Servers disconnected";
                    break;
            }

            NotifyStateChanged();
        }
    }
}
