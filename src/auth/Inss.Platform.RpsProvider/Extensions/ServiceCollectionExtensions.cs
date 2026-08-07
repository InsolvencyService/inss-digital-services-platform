using System.Net;
using Azure.Identity;
using Inss.Common.Infrastructure;
using Inss.Common.Infrastructure.Options;
using Inss.Platform.Component.Extensions;
using Inss.Platform.Infrastructure.Options;
using Inss.Platform.RpsProvider.Application.Clients;
using Inss.Platform.RpsProvider.Application.Providers;
using Inss.Platform.RpsProvider.Application.Services;
using Inss.Platform.RpsProvider.Infrastructure.Clients;
using Inss.Platform.RpsProvider.Infrastructure.Providers;
using Inss.Platform.RpsProvider.Infrastructure.Serialization;
using Inss.Platform.RpsProvider.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Azure.Cosmos;

namespace Inss.Platform.RpsProvider.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddRpsAuthentication(IConfiguration configuration, IWebHostEnvironment environment)
        {
            services.AddScoped<ILoginService, LoginService>();
            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/login";
                    options.Cookie.SameSite = SameSiteMode.None;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                });
            
            services.AddOptions<ProviderOptions>()
                .Bind(configuration.GetSection("Provider"))
                .ValidateDataAnnotations()
                .ValidateOnStart();
            
            services.AddOptions<LoginOptions>()
                .Bind(configuration.GetSection("Login"))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddScoped<ILoginService, LoginService>();
            
            ExternalApiOptions loginOptions = configuration.GetSection("RpsLogin").Get<ExternalApiOptions>()!;

            if (environment.IsDevelopment())
            {
                services.AddSingleton<IUserAuthenticationPageClient, MockUserAuthenticationPageClient>();
                services.AddSingleton<IUserAuthenticationClient, MockUserAuthenticationClient>();
            }
            else
            {
                if (environment.IsProduction())
                {
                    services.AddAppDataProtection(configuration);
                }

                CookieContainer cookieContainer = new();
                services.AddScoped<CookieContainer>(_ => cookieContainer);
                
                services.AddHttpClient<IUserAuthenticationPageClient, UserAuthenticationPageClient>(client =>
                    {
                        client.BaseAddress = new Uri(loginOptions.Url);
                    })
                    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                    {
                        CookieContainer = cookieContainer, UseCookies = true, AllowAutoRedirect = true
                    })
                    .SetHandlerLifetime(TimeSpan.FromMinutes(loginOptions.LifetimeMinutes))
                    .AddPolicyHandler((sp, _) => Resilience.GetRetryPolicy(sp, loginOptions.RetryCount))
                    .AddPolicyHandler((sp, _) => Resilience.GetCircuitBreaker(sp, 
                        loginOptions.CountBeforeBreaking, loginOptions.BreakDurationSeconds));

                services.AddHttpClient<IUserAuthenticationClient, UserAuthenticationClient>(client =>
                    {
                        client.BaseAddress = new Uri(loginOptions.Url);
                    })
                    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                    {
                        CookieContainer = cookieContainer, UseCookies = true, AllowAutoRedirect = false
                    })
                    .SetHandlerLifetime(TimeSpan.FromMinutes(loginOptions.LifetimeMinutes))
                    .AddPolicyHandler((sp, _) => Resilience.GetRetryPolicy(sp, loginOptions.RetryCount))
                    .AddPolicyHandler((sp, _) => Resilience.GetCircuitBreaker(sp,
                        loginOptions.CountBeforeBreaking, loginOptions.BreakDurationSeconds));
            }
            
            services.AddSingleton<ITokenSecurityProvider, TokenSecurityProvider>();
            
            return services;
        }

        public IServiceCollection AddAuthCodeStore(IConfiguration configuration)
        {
            CosmosDbOptions cosmosDbOptions = new();
            configuration.GetSection("CosmosDb").Bind(cosmosDbOptions);
            
            services.AddSingleton<IUserAuthStoreProvider>(_ =>
            {
                if (!string.IsNullOrWhiteSpace(cosmosDbOptions.ConnectionString))
                {
                    CosmosClientOptions options = new() { Serializer = new CosmosModelSerializer() };
                    CosmosClient client = new(cosmosDbOptions.ConnectionString, options);
                    return new CosmosUserAuthStoreProvider(client, cosmosDbOptions.DatabaseName, cosmosDbOptions.ContainerName);
                }

                if (!string.IsNullOrWhiteSpace(cosmosDbOptions.AccountEndpoint))
                {
                    CosmosClientOptions options = new() { Serializer = new CosmosModelSerializer() };
                    CosmosClient client = new(cosmosDbOptions.AccountEndpoint, new DefaultAzureCredential(), options);
                    return new CosmosUserAuthStoreProvider(client, cosmosDbOptions.DatabaseName, cosmosDbOptions.ContainerName);
                }

                return new TestUserAuthStoreProvider();
            });
            
            return services;
        }
    }
}