using Marketplace.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Marketplace.Client.Services
{
    public class SettingsService
    {
        private readonly HttpClient httpClient;

        public SettingsService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<string> GetSettingValueAsync(string settingId)
        {
            var setting = await httpClient.GetFromJsonAsync<Setting>("api/settings/" + settingId);
            return setting.SettingValue;
        }
    }
}
