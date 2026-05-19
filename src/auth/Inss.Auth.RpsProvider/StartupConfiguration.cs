using System.Net;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using GovUk.Forms.Application.Providers;
using GovUk.Forms.Components.Extensions;
using GovUk.Forms.Components.Options;
using GovUk.Forms.Infrastructure.Providers;
using GovUk.Frontend.AspNetCore;
using Inss.Auth.RpsProvider.Application.Clients;
using Inss.Auth.RpsProvider.Application.Providers;
using Inss.Auth.RpsProvider.Application.Services;
using Inss.Auth.RpsProvider.Infrastructure.Clients;
using Inss.Auth.RpsProvider.Infrastructure.Providers;
using Inss.Auth.RpsProvider.Options;
using Microsoft.AspNetCore.Authentication.Cookies;

[assembly: HostingStartup(typeof(Inss.Auth.RpsProvider.StartupConfiguration))]

namespace Inss.Auth.RpsProvider;

public class StartupConfiguration : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => options.LoginPath = "/login");
            
            services.AddOptions<ProviderOptions>()
                .Bind(context.Configuration.GetSection("Provider"))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            
            services.AddOptions<LoginOptions>()
                .Bind(context.Configuration.GetSection("Login"))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddSingleton<ILoginService, LoginService>();

            ExternalApiOptions authenticationOptions = context.Configuration.GetSection("RpsRelay").Get<ExternalApiOptions>()!;
            HttpClientHandler clientHandler = new() { AllowAutoRedirect = true, CookieContainer = new CookieContainer() };
            services.AddTypedClient<IUserAuthenticationClient, UserAuthenticationClient>(authenticationOptions, clientHandler);
            
            services.AddSingleton<IUserAuthStoreProvider, TestUserAuthStoreProvider>();
            services.AddSingleton<ITokenSecurityProvider, TokenSecurityProvider>();
            services.AddScoped<IPagePropertiesProvider, PagePropertiesProvider>();
            services.AddGovUkFrontend();
            services.AddControllersWithViews();
            services.AddOpenTelemetry().UseAzureMonitor();
        });
        
        builder.Configure(app =>
        {
            app.UseAuthentication();
            app.UseGovUkFrontend();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        });
    }
}