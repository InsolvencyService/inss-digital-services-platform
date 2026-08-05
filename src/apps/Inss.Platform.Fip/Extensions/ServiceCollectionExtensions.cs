using Inss.Platform.Application.Extensions;
using Inss.Platform.Component.Resolvers;
using Inss.Platform.Domain;
using Inss.Platform.Fip.Application.Services;
using Inss.Platform.Fip.Infrastructure.Clients;
using Inss.Platform.Infrastructure.Extensions;

namespace Inss.Platform.Fip.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddAppServices(IWebHostEnvironment environment, IConfiguration configuration)
        {
            const string configKey = "FIPSearch";

            services.AddSingleton<IStartPageResolver, StartPageResolver>();
            services.AddSearch<SearchEnrichmentService>(configKey);
            
            if (environment.IsDevelopment())
            {
                services.AddMockSearchInfrastructure<MockSearchClient>(configuration, configKey);
            }
            else
            {
                services.AddSearchInfrastructure(configuration, configKey);
            }

            return services;
        }
        
        public PagePathList BuildApp()
        {
            FipAppBuilder appBuilder = new();
            PagePathList pagePaths = [];
            pagePaths.AddRange(appBuilder.Build(services));
            return pagePaths;
        }
    }
}