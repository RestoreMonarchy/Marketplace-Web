using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Reflection;
using Marketplace.DatabaseProvider.Extensions;
using Marketplace.DatabaseProvider.Repositories;
using Marketplace.Server.Services;
using SteamWebAPI2.Utilities;
using Marketplace.Server.Utilities;
using MySql.Data.MySqlClient;

namespace Marketplace.Server
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

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

            services.AddMarketplaceSql(configuration.GetConnectionString("MsSql"));

            services.AddUconomyMySql(configuration.GetConnectionString("ServersDatabase"));
            services.AddMemoryCache();

            services.AddTransient(c => new SteamWebInterfaceFactory(c.GetService<ISettingService>().SteamDevKey));
            services.AddTransient(c => 
            {
                var service = c.GetService<ISettingService>();
                Console.WriteLine($"service null? {service == null}");
                return new MySqlConnection(c.GetService<ISettingService>().MySqlConnectionString);
            });

            services.AddTransient<ISteamService, SteamService>();
            services.AddSingleton<ISettingService, SettingService>();
            services.AddHttpClient();

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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapFallbackToClientSideBlazor<Client.Startup>("index.html");
            });
            using (var scope = app.ApplicationServices.CreateScope())
            {                
                scope.ServiceProvider.GetService<IUnturnedItemsRepository>().Initialize()?.GetAwaiter().GetResult();
                scope.ServiceProvider.GetService<IMarketItemsRepository>().Initialize()?.GetAwaiter().GetResult();
                scope.ServiceProvider.GetService<ISettingsRepository>().Initialize()?.GetAwaiter().GetResult();
                scope.ServiceProvider.GetService<IServersRepository>().Initialize()?.GetAwaiter().GetResult();
                scope.ServiceProvider.GetService<ICommandsRepository>().Initialize()?.GetAwaiter().GetResult();
                scope.ServiceProvider.GetService<IProductsRepository>().Initialize()?.GetAwaiter().GetResult();
                scope.ServiceProvider.GetService<ISettingService>().InitializeAsync()?.GetAwaiter().GetResult();
            }
        }
    }
}

