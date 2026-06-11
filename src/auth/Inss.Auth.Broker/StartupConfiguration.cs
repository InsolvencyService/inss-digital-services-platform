using Azure.Identity;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using GovUk.Forms.Application.Providers;
using GovUk.Forms.Infrastructure.Options;
using GovUk.Forms.Infrastructure.Providers;
using GovUk.Forms.Infrastructure.Serialization;
using Inss.Auth.Broker;
using Inss.Auth.Broker.Application.Providers;
using Inss.Auth.Broker.Extensions;
using Inss.Auth.Broker.Infrastructure.Providers;
using Inss.Auth.Broker.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Azure.Cosmos;

[assembly: HostingStartup(typeof(StartupConfiguration))]

namespace Inss.Auth.Broker;

public class StartupConfiguration : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configurationBuilder) =>
        {
            IConfigurationRoot config = configurationBuilder.Build();
            string? configFileOverride = config["config"];
            
            if (configFileOverride is not null && File.Exists(configFileOverride)){
                configurationBuilder.AddJsonFile(configFileOverride, optional: true);
            }
        });
        
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
            
            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie()
                .AddOneLogin()
                .AddRps()
                .AddEntra();
            
            CosmosDbOptions cosmosDbOptions = new();
            context.Configuration.GetSection("CosmosDb").Bind(cosmosDbOptions);
            
            services.AddSingleton<ITokenSecurityProvider, TokenSecurityProvider>();
            services.AddSingleton<IAuthCodeStoreProvider>(_ =>
            {
                if (!string.IsNullOrWhiteSpace(cosmosDbOptions.ConnectionString) ||
                    !string.IsNullOrWhiteSpace(cosmosDbOptions.AccountEndpoint))
                {
                    CosmosClientOptions options = new() { Serializer = new CosmosModelSerializer() };
                    CosmosClient client = cosmosDbOptions.ConnectionString is not null
                        ? new CosmosClient(cosmosDbOptions.ConnectionString, options)
                        : new CosmosClient(cosmosDbOptions.AccountEndpoint, new DefaultAzureCredential(), options);
                    return new CosmosAuthCodeStoreProvider(client, cosmosDbOptions.DatabaseName, cosmosDbOptions.ContainerName);
                }

                return new TestAuthCodeStoreProvider();
            });
            services.AddOpenTelemetry().UseAzureMonitor();
            services.AddScoped<IPagePropertiesProvider, PagePropertiesProvider>();
            
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