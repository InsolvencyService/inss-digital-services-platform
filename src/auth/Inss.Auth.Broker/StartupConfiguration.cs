using Azure.Monitor.OpenTelemetry.AspNetCore;
using Inss.Auth.Broker;
using Inss.Auth.Broker.Application.Providers;
using Inss.Auth.Broker.Extensions;
using Inss.Auth.Broker.Infrastructure.Providers;
using Inss.Auth.Broker.Options;
using Microsoft.AspNetCore.Authentication.Cookies;

[assembly: HostingStartup(typeof(StartupConfiguration))]

namespace Inss.Auth.Broker;

public class StartupConfiguration : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            services.AddOptions<BrokerOptions>()
                .Bind(context.Configuration.GetSection("Broker"))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            
            services.AddOptions<RpsIdentityProviderOptions>()
                .Bind(context.Configuration.GetSection("IdentityProviders:Rps"))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            
            services.AddOptions<OneLoginIdentityProviderOptions>()
                .Bind(context.Configuration.GetSection("IdentityProviders:OneLogin"))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            
            services.AddOptions<EntraIdentityProviderOptions>()
                .Bind(context.Configuration.GetSection("IdentityProviders:Entra"))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            
            services.AddAuthentication(options => { options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; })
                .AddCookie()
                .AddOneLogin()
                .AddRps()
                .AddEntra();
            
            services.AddSingleton<ITokenSecurityProvider, TokenSecurityProvider>();
            services.AddSingleton<IAuthCodeStoreProvider, TestAuthCodeStoreProvider>();
            services.AddOpenTelemetry().UseAzureMonitor();
            
        });
        
        builder.Configure(app =>
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        });
    }
}