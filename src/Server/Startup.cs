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

namespace Marketplace.Server
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(options => { options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; })
                .AddCookie(options =>
                {
                    options.LoginPath = "/signin";
                    options.LogoutPath = "/signout";
                    options.Events.OnValidatePrincipal = InitializePlayerAsync;
                }).AddSteam();

            services.AddMvc();
            services.AddResponseCompression(opts =>
            {
                opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
                    new[] { "application/octet-stream" });
            });

            if (_configuration["DatabaseProvider"].Equals("MYSQL", StringComparison.OrdinalIgnoreCase))
            {
                services.AddSingleton<IDatabaseProvider>(new MySqlDatabaseProvider(_configuration.GetConnectionString("MYSQL")));
            } else
            {
                services.AddSingleton<IDatabaseProvider>(new SqlDatabaseProvider(_configuration.GetConnectionString("MSSQL")));
            }

            services.AddMemoryCache();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Marketplace Web {Assembly.GetExecutingAssembly().GetName().Version} is getting loaded..."); //TODO: Use logger instead.
            Console.ResetColor();
        }

        private async Task InitializePlayerAsync(CookieValidatePrincipalContext arg) //Shoudn't be async?
        {
            string steamId = arg.Principal.FindFirst(ClaimTypes.NameIdentifier).Value.Substring(37); //Magic number, not good, what does 37 mean? Make a constant for it.
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, steamId));
            claims.Add(new Claim(ClaimTypes.Role, "Guest"));

            arg.ReplacePrincipal(new ClaimsPrincipal(new ClaimsIdentity(claims, "DefaultAuth")));
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
        }
    }
}
