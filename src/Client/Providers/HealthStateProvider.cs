using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Marketplace.Client.Providers
{
    public interface IHealthStateProvider
    {
        Task<HealthState?> GetHealthState();
    }

    public class HealthStateProvider : IHealthStateProvider
    {
        private readonly HttpClient httpClient;

        public HealthStateProvider(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<HealthState?> GetHealthState()
        {
            var state = await httpClient.GetAsync("/health");
            var content = await state.Content.ReadAsStringAsync(); //Optimize later to use stream and models.
            var @object = new JObject(content);

            switch (@object["status"].Value<string>())
            {
                case "Unhealthy":
                    return new HealthState();
                case "Degraded":
                    return new HealthState(new string[]
                    {
                        "/dashboard",
                        "/login"
                    });
                default:
                    return null;

            }
        }
    }

    public struct HealthState
    {
        public HealthState(IEnumerable<string> disabledExceptions)
        {
            this.DisabledExceptions = disabledExceptions ?? throw new ArgumentNullException(nameof(disabledExceptions));
        }


        public IEnumerable<string> DisabledExceptions { get; }
    }
}
