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
using Marketplace.WebSockets;
using Marketplace.WebSockets.Logger;

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

            services.AddMemoryCache();
            services.AddHttpClient();

            services.AddTransient<IWebSocketsLogger, WebSocketsConsoleLogger>(c => new WebSocketsConsoleLogger(true));
            services.AddSingleton<IWebSocketsManager, WebSocketsManager>();
            services.AddWebSocketCallers();
            services.AddWebSocketsData();

            services.AddSingleton<ISettingService, SettingService>();
            services.AddSingleton<IUnturnedItemsIconService, UnturnedItemsIconService>();
            services.AddTransient<IUserService, UserService>();

            services.AddHealthChecks()                
                .AddCheck<MainDatabaseHealthCheck>("MainDatabase")
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
                scope.InitializeWebSocketCallers();
            }
        }
    }
}