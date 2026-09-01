using Azure;
using Azure.Identity;
using Azure.Search.Documents;
using GovUk.Forms.Application.Clients;
using GovUk.Forms.Application.Providers;
using GovUk.Forms.Application.Services;
using GovUk.Forms.Infrastructure.Clients;
using GovUk.Forms.Infrastructure.Options;
using GovUk.Forms.Infrastructure.Providers;
using GovUk.Forms.Infrastructure.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GovUk.Forms.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure(IConfiguration configuration)
        {
            CosmosDbOptions cosmosDbOptions = new();
            configuration.GetSection("CosmosDb").Bind(cosmosDbOptions);

            services.AddSingleton<IFormStorageProvider>(provider =>
            {
                if (!string.IsNullOrWhiteSpace(cosmosDbOptions.ConnectionString))
                {
                    CosmosClientOptions options = new() { Serializer = new CosmosModelSerializer() };
                    CosmosClient client = new(cosmosDbOptions.ConnectionString, options);
                    IHttpContextAccessor httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
                    ILogger<CosmosFormStorageProvider> logger = provider.GetRequiredService<ILogger<CosmosFormStorageProvider>>();
                    return new CosmosFormStorageProvider(
                        client, cosmosDbOptions.DatabaseName, cosmosDbOptions.ContainerName, httpContextAccessor, logger);
                }

                if (!string.IsNullOrWhiteSpace(cosmosDbOptions.AccountEndpoint))
                {
                    CosmosClientOptions options = new() { Serializer = new CosmosModelSerializer() };
                    CosmosClient client = new(cosmosDbOptions.AccountEndpoint, new DefaultAzureCredential(), options);
                    IHttpContextAccessor httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
                    ILogger<CosmosFormStorageProvider> logger = provider.GetRequiredService<ILogger<CosmosFormStorageProvider>>();
                    return new CosmosFormStorageProvider(
                        client, cosmosDbOptions.DatabaseName, cosmosDbOptions.ContainerName, httpContextAccessor, logger);
                }

                return new TestFormStorageProvider();
            });

            services.AddScoped<IPagePropertiesProvider, PagePropertiesProvider>();

            return services;
        }

        public IServiceCollection AddMockSearchInfrastructure<TMockClient>(
            IConfiguration configuration, 
            string configKey) 
            where TMockClient : class, ISearchClient
        {
            AzureSearchOptions searchOptions = configuration.BindAndValidate<AzureSearchOptions>(configKey);

            services.AddKeyedSingleton<ISearchConfigProvider>(configKey, (provider, _) =>
            {
                ILogger<SearchConfigProvider> logger = provider.GetRequiredService<ILogger<SearchConfigProvider>>();
                return new SearchConfigProvider(searchOptions.ConfigPath, logger);
            });
            services.AddKeyedSingleton<ISearchClient, TMockClient>(configKey);
            return services;
        }

        public IServiceCollection AddSearchInfrastructure(IConfiguration configuration, string configKey)
        {
            AzureSearchOptions searchOptions = configuration.BindAndValidate<AzureSearchOptions>(configKey);

            services.AddKeyedSingleton<ISearchConfigProvider>(configKey, (provider, _) =>
            {
                ILogger<SearchConfigProvider> logger = provider.GetRequiredService<ILogger<SearchConfigProvider>>();
                return new SearchConfigProvider(searchOptions.ConfigPath, logger);
            });

            services.AddKeyedSingleton<ISearchClient>(configKey, (provider, _) =>
            {
                SearchClient searchClient = new(
                    new Uri(searchOptions.Endpoint), 
                    searchOptions.IndexName, 
                    new AzureKeyCredential(searchOptions.ApiKey));
                ILogger<SearchService> logger = provider.GetRequiredService<ILogger<SearchService>>();
                return new AzureSearchClient(searchClient, logger);
            });

            return services;
        }
    }
}
