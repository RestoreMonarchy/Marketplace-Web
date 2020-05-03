using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Reflection;
using Marketplace.DatabaseProvider.Extensions;
using Marketplace.Server.Services;
using Marketplace.Server.Utilities;
using Marketplace.Server.Health;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Marketplace.Server.Extensions;
using Marketplace.Server.WebSockets;

namespace Marketplace.Server
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options => { options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; })
                .AddCookie(options =>
                {
                    options.LoginPath = "/signin";
                    options.LogoutPath = "/signout";
                    options.AccessDeniedPath = "/";
                    options.Events.OnValidatePrincipal = PrincipalValidator.ValidateAsync;
                }).AddSteam();

            services.AddLogging();
            services.AddAuthorizationCore();
            services.AddControllers();
            services.AddMvc();

            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });

            services.AddMarketplaceSql(Environment.GetEnvironmentVariable("MSSQL_CONNECTION_STRING"));
            services.AddEconomyServices();

            services.AddMemoryCache();
            services.AddHttpClient();

            services.AddSingleton<ServerService>();
            services.AddSingleton<ISettingService, SettingService>();
            services.AddSingleton<IUnturnedItemsIconService, UnturnedItemsIconService>();
            services.AddTransient<ISteamService, SteamService>();
            services.AddTransient<WebSocketsUtility>();

            services.AddHealthChecks()                
                .AddCheck<MainDatabaseHealthCheck>("MainDatabase")
                .AddCheck<EconomyDatabaseHealthCheck>("Economy")
                .AddCheck<SteamWebApiHealthCheck>("SteamWebAPI");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Marketplace Web {Assembly.GetExecutingAssembly().GetName().Version} is getting loaded..."); //TODO: Use logger instead.
            Console.ResetColor();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBlazorDebugging();
            }

            app.UseStaticFiles();
            app.UseClientSideBlazorFiles<Client.Startup>();
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();

            app.UseWebSockets();
            app.UseMiddleware<WebSocketsMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapFallbackToClientSideBlazor<Client.Startup>("index.html");

                endpoints.MapHealthChecks("/health", new HealthCheckOptions
                {
                    Predicate = (check) => true,
                    ResponseWriter = HealthCheckHelpers.WriteResponses
                });
            });

            using (var scope = app.ApplicationServices.CreateScope())
            {
                scope.InitializeRepositories();
            }
        }
    }
}