﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Marketplace.Server.Health
{
    internal static class HealthCheckHelpers
    {
        public static async Task WriteResponses(HttpContext context, HealthReport result)
        {
            var json = new JObject(
                            new JProperty("status", result.Status.ToString()),
                            new JProperty("results", new JObject(result.Entries.Select(pair =>
                            new JProperty(pair.Key, new JObject(
                                new JProperty("status", pair.Value.Status.ToString()),
                                new JProperty("description", pair.Value.Description),
                                new JProperty("data", new JObject(pair.Value.Data.Select(
                                    p => new JProperty(p.Key, p.Value))))))))));

            context.Response.ContentType = MediaTypeNames.Application.Json;
            await context.Response.WriteAsync(json.ToString(Formatting.Indented));
        }
    }
}
