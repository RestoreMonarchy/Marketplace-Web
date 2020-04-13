using Marketplace.DatabaseProvider;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Marketplace.DatabaseProvider.Extensions;
using Marketplace.DatabaseProvider.Repositories;
using MySql.Data.MySqlClient;
using Marketplace.Server.Services;
using SteamWebAPI2.Utilities;
using Marketplace.Shared.Constants;

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
                    options.Events.OnValidatePrincipal = (arg) =>
                    {
                        string steamId = arg.Principal.FindFirst(ClaimTypes.NameIdentifier).Value.Substring(37);
                        List<Claim> claims = new List<Claim>();
                        claims.Add(new Claim(ClaimTypes.Name, steamId));
                        if (configuration.GetSection("Admins").Get<string[]>().Any(x=> x == steamId))
                            claims.Add(new Claim(ClaimTypes.Role, RoleConstants.AdminRoleId));
                        else
                            claims.Add(new Claim(ClaimTypes.Role, RoleConstants.GuestRoleId));

                        arg.ReplacePrincipal(new ClaimsPrincipal(new ClaimsIdentity(claims, "DefaultAuth")));
                        return Task.CompletedTask;
                    };
                }).AddSteam();

            services.AddLogging();
            services.AddAuthorizationCore();
            services.AddControllers();
            //services.AddMvc();
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });

            var provider = configuration.GetValue<string>("DatabaseProvider");
            switch (provider)
            {
                case "MySql":
                    services.AddMarketplaceMySql(configuration.GetConnectionString("MySql"));
                    break;
                case "MsSql":
                    services.AddMarketplaceSql(configuration.GetConnectionString("MsSql"));
                    break;
            }
            services.AddUconomyMySql(configuration.GetConnectionString("ServersDatabase"));
            services.AddMemoryCache();

            services.AddTransient(s => new SteamWebInterfaceFactory(configuration["SteamDevKey"]));
            services.AddTransient<ISteamService, SteamService>();
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
                Task.Run(scope.ServiceProvider.GetService<IUnturnedItemsRepository>().Initialize).Wait();
                Task.Run(scope.ServiceProvider.GetService<IMarketItemsRepository>().Initialize).Wait();
                Task.Run(scope.ServiceProvider.GetService<ISettingsRepository>().Initialize).Wait();
            }
        }
    }
}

