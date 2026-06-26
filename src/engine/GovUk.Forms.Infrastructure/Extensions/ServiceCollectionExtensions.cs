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
                if (!string.IsNullOrWhiteSpace(cosmosDbOptions.ConnectionString) ||
                    !string.IsNullOrWhiteSpace(cosmosDbOptions.AccountEndpoint))
                {
                    CosmosClientOptions options = new() { Serializer = new CosmosModelSerializer() };
                    CosmosClient client = cosmosDbOptions.ConnectionString is not null
                        ? new CosmosClient(cosmosDbOptions.ConnectionString, options)
                        : new CosmosClient(cosmosDbOptions.AccountEndpoint, new DefaultAzureCredential(), options);
                    IHttpContextAccessor httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
                    return new CosmosFormStorageProvider(
                        client, cosmosDbOptions.DatabaseName, cosmosDbOptions.ContainerName, httpContextAccessor);
                }

                return new TestFormStorageProvider();
            });

            services.AddScoped<IPagePropertiesProvider, PagePropertiesProvider>();

            // services.AddSingleton<ISearchConfigProvider, SearchConfigProvider>();
            // SearchPersonOptions searchOptions = new();
            // configuration
            //     .GetSection("EIIRPersonSearch")
            //     .Bind(searchOptions);
            // services.AddSingleton(searchOptions);
            //
            // services.AddScoped<ISearchService, SearchService>();
            // services.AddSingleton(serviceProvider =>
            // {
            //     SearchPersonOptions options =
            //         serviceProvider.GetRequiredService<SearchPersonOptions>();
            //
            //     return new SearchClient(
            //         new Uri(options.Endpoint),
            //         options.IndexName,
            //         new AzureKeyCredential(options.ApiKey));
            // });

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
                    new Uri(searchOptions.Endpoint), searchOptions.IndexName, new AzureKeyCredential(searchOptions.ApiKey));
                ILogger<SearchService> logger = provider.GetRequiredService<ILogger<SearchService>>();
                return new MockSearchClient();// new AzureSearchClient(searchClient, logger);
            });
            
            return services;
        }
    }
}