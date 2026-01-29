using INSS.Platform.Cache.Application.Repositories;
using INSS.Platform.Cache.Infrastructure.Repositories;
using INSS.Platform.Portal.Application.Clients;
using INSS.Platform.Portal.Infrastructure.Clients;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace INSS.Platform.Shared.Web.Cache.Configuration;

/// <summary>
/// Provides extension methods for configuring cache-related services in the application.
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Adds cache configuration services to the specified <see cref="IServiceCollection"/>.
    /// Configures session state, Cosmos DB client, and cache repositories.
    /// </summary>
    /// <param name="services">The service collection to add the cache configuration to.</param>
    /// <param name="environment">The host environment providing application and environment names.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the Cosmos DB connection string is not configured in the application settings.
    /// </exception>
    public static IServiceCollection AddCacheConfiguration(
        this IServiceCollection services,
        IHostEnvironment environment)
    {
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromDays(1);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.Name = CreateSessionCookieName(environment);
        });

        services.AddSingleton(provider =>
        {
            IConfiguration configuration = provider.GetRequiredService<IConfiguration>();
            string? cosmosConnectionString = configuration.GetConnectionString("CosmosCacheDb");

            if (string.IsNullOrEmpty(cosmosConnectionString))
            {
                throw new InvalidOperationException("Cosmos DB connection string is not configured. Configure ConnectionStrings:CosmosCacheDb");
            }

            CosmosClientOptions cosmosClientOptions = new()
            {
                SerializerOptions = new CosmosSerializationOptions
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase,
                    IgnoreNullValues = true,
                }
            };

            return new CosmosClient(cosmosConnectionString, cosmosClientOptions);
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
