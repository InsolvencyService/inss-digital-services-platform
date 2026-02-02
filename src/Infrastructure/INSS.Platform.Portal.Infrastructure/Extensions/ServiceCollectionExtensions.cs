using INSS.Platform.Cache.Application.Repositories;
using INSS.Platform.Cache.Infrastructure.Configuration;
using INSS.Platform.Cache.Infrastructure.Repositories;
using INSS.Platform.Portal.Application.Clients;
using INSS.Platform.Portal.Application.Resolvers;
using INSS.Platform.Portal.Application.Services;
using INSS.Platform.Portal.Infrastructure.Clients;
using INSS.Platform.Portal.Infrastructure.Configuration;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace INSS.Platform.Portal.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IFormStateService, TestFormStateService>();
        services.AddSingleton<IUserSessionResolver, TestUserSessionResolver>();
        services.AddSingleton<IBankClient, MockBankClient>();

        return services;
    }

    /// <summary>
    /// Registers the bank validation infrastructure and its configuration options.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance for accessing configuration settings.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddBankValidationInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IBankClient, PayPointBankValidationClient>();

        services.AddOptions<BankValidationOptions>()
            .Bind(configuration.GetSection("BankValidation"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }

    /// <summary>
    /// Registers the canonical data API client infrastructure and its configuration options.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance for accessing configuration settings.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddCanonicalDataInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICanonicalDataClient, CanonicalDataClient>();

        services.AddOptions<CanonicalDataOptions>()
            .Bind(configuration.GetSection("CanonicalData"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }

    /// <summary>
    /// Registers the analytics client infrastructure and its configuration options.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance for accessing configuration settings.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddAnalyticsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAnalyticsClient, RybbitAnalyticsClient>();

        services.AddOptions<AnalyticsOptions>()
            .Bind(configuration.GetSection("Analytics"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }

    /// <summary>
    /// Registers Cosmos DB-based cache infrastructure and session management for the application.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="environment">The <see cref="IHostEnvironment"/> providing application and environment names for session cookie configuration.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> instance for accessing configuration settings.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
    /// <remarks>
    /// This method configures session state with a deterministic cookie name, binds Cosmos cache options from configuration,
    /// registers a singleton <see cref="CosmosClient"/> for database access, and adds scoped cache repository and client services.
    /// </remarks>
    public static IServiceCollection AddCosmosCacheInfrastructure(this IServiceCollection services, IHostEnvironment environment, IConfiguration configuration)
    {
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromDays(1);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.Name = CreateSessionCookieName(environment);
        });

        services.AddOptions<CosmosCacheOptions>()
            .Bind(configuration.GetSection("FormCache"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton(provider =>
        {
            CosmosCacheOptions options = provider.GetRequiredService<IOptions<CosmosCacheOptions>>().Value;

            CosmosClientOptions cosmosClientOptions = new()
            {
                SerializerOptions = new CosmosSerializationOptions
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase,
                    IgnoreNullValues = true,
                }
            };

            return new CosmosClient(options.ConnectionString, cosmosClientOptions);
        });

        services.AddScoped<ICacheRepository, CacheRepository>();

        services.AddScoped<IFormCacheClient, FormCacheCosmosClient>();

        return services;
    }

    /// <summary>
    /// Creates a safe, deterministic cookie name based on the application and environment names.
    /// </summary>
    /// <param name="environment">The host environment providing <see cref="IHostEnvironment.ApplicationName"/> and <see cref="IHostEnvironment.EnvironmentName"/>.</param>
    /// <returns>
    /// A cookie name in the form <c>{ApplicationName}.{EnvironmentName}.Cache</c>,
    /// where non-alphanumeric characters (except <c>.</c>) are removed. Fallbacks to
    /// <c>UnknownApplication</c> and <c>UnknownEnvironment</c> if values are missing.
    /// </returns>
    /// <remarks>
    /// This method sanitizes the application and environment names to include only letters, digits, and periods,
    /// ensuring the resulting cookie name is valid and consistent across deployments.
    /// </remarks>
    private static string CreateSessionCookieName(IHostEnvironment environment)
    {
        string appName = string.IsNullOrWhiteSpace(environment.ApplicationName) ? "UnknownApplication" : environment.ApplicationName;
        string envName = string.IsNullOrWhiteSpace(environment.EnvironmentName) ? "UnknownEnvironment" : environment.EnvironmentName;

        string safeApp = new([.. appName.Where(ch => char.IsLetterOrDigit(ch) || ch == '.')]);
        string safeEnv = new([.. envName.Where(ch => char.IsLetterOrDigit(ch) || ch == '.')]);

        return $"{safeApp}.{safeEnv}.Cache";
    }
}