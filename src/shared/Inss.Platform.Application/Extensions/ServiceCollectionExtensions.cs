using Inss.Platform.Application.Clients;
using Inss.Platform.Application.Navigators;
using Inss.Platform.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Inss.Platform.Application.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplication()
        {
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INextPageNavigator, DefaultNextPageNavigator>();
            return services;
        }
        
        public IServiceCollection AddSearch(string configKey)
        {
            services.AddKeyedTransient<ISearchService>(configKey, (provider, _) =>
            {
                ISearchClient searchClient = provider.GetRequiredKeyedService<ISearchClient>(configKey);
                ILogger<SearchService> logger = provider.GetRequiredService<ILogger<SearchService>>();
                return new SearchService(searchClient, logger);
            });
            
            return services;
        }
        
        public IServiceCollection AddSearch<TSearchDecorator>(string configKey) where TSearchDecorator : class, ISearchService
        {
            // When using the decorator, because of the keyed context to allow the search to be used in multiple places
            // the construction of a decorator must be of:
            // - configKey
            // - IServiceProvider
            // and your decorated instance then creates the underlying search service
            services.AddKeyedTransient<ISearchService>(configKey, (provider, _) 
                => (TSearchDecorator)Activator.CreateInstance(typeof(TSearchDecorator), configKey, provider)!);
            
            return services;
        }
    }
}