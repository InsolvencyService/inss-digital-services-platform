using Azure.Identity;
using Inss.Platform.Broker.Application.Providers;
using Inss.Platform.Broker.Infrastructure.Providers;
using Inss.Platform.Broker.Infrastructure.Serialization;
using Inss.Platform.Broker.Options;
using Inss.Platform.Component.Extensions;
using Inss.Platform.Infrastructure.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Azure.Cosmos;

namespace Inss.Platform.Broker.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddBrokerAuthentication(IConfiguration configuration, IWebHostEnvironment environment)
        {
            services.AddOptions<BrokerOptions>()
                .Bind(configuration.GetSection("Broker"))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            
            services.AddOptions<RpsIdentityProviderOptions>()
                .Bind(configuration.GetSection("IdentityProviders:Rps"))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            
            services.AddOptions<OneLoginIdentityProviderOptions>()
                .Bind(configuration.GetSection("IdentityProviders:OneLogin"))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            
            services.AddOptions<EntraIdentityProviderOptions>()
                .Bind(configuration.GetSection("IdentityProviders:Entra"))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.Cookie.SameSite = SameSiteMode.None;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                })
                .AddOneLogin()
                .AddRps()
                .AddEntra();
            
            if (environment.IsProduction())
            {
                services.AddAppDataProtection(configuration);
            }
            
            return services;
        }

        public IServiceCollection AddAuthCodeStore(IConfiguration configuration)
        {
            CosmosDbOptions cosmosDbOptions = new();
            configuration.GetSection("CosmosDb").Bind(cosmosDbOptions);
            
            services.AddSingleton<ITokenSecurityProvider, TokenSecurityProvider>();
            services.AddSingleton<IAuthCodeStoreProvider>(_ =>
            {
                if (!string.IsNullOrWhiteSpace(cosmosDbOptions.ConnectionString))
                {
                    CosmosClientOptions options = new() { Serializer = new CosmosModelSerializer() };
                    CosmosClient client = new(cosmosDbOptions.ConnectionString, options);
                    return new CosmosAuthCodeStoreProvider(client, cosmosDbOptions.DatabaseName, cosmosDbOptions.ContainerName);
                }

                if (!string.IsNullOrWhiteSpace(cosmosDbOptions.AccountEndpoint))
                {
                    CosmosClientOptions options = new() { Serializer = new CosmosModelSerializer() };
                    CosmosClient client = new(cosmosDbOptions.AccountEndpoint, new DefaultAzureCredential(), options);
                    return new CosmosAuthCodeStoreProvider(client, cosmosDbOptions.DatabaseName, cosmosDbOptions.ContainerName);
                }

                return new TestAuthCodeStoreProvider();
            });

            return services;
        }
    }
}