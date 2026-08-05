using Azure;
using Azure.Search.Documents;
using GovUk.Forms.Application.Services;
using Inss.Platform.Application.Clients;
using Inss.Platform.Application.Providers;
using Inss.Platform.Application.Services;
using Inss.Platform.Infrastructure.Clients;
using Inss.Platform.Infrastructure.Options;
using Inss.Platform.Infrastructure.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Inss.Platform.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure()
        {
            services.AddSingleton<IAppProvider, TempAppProvider>();
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