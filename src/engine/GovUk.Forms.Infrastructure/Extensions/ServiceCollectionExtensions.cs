using Azure.Identity;
using GovUk.Forms.Application.Providers;
using GovUk.Forms.Infrastructure.Options;
using GovUk.Forms.Infrastructure.Providers;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GovUk.Forms.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure(IConfiguration configuration)
        {
            CosmosDbOptions cosmosDbOptions = new();
            configuration.GetSection("CosmosDb").Bind(cosmosDbOptions);
            
            services.AddSingleton<IFormStorageProvider>(_ =>
            {
                if (!string.IsNullOrWhiteSpace(cosmosDbOptions.ConnectionString) ||
                    !string.IsNullOrWhiteSpace(cosmosDbOptions.AccountEndpoint))
                {
                    var client = cosmosDbOptions.ConnectionString is not null 
                        ? new CosmosClient(cosmosDbOptions.ConnectionString)
                        : new CosmosClient(cosmosDbOptions.AccountEndpoint, new DefaultAzureCredential());
                    return new CosmosFormStorageProvider(client, cosmosDbOptions.DatabaseName, cosmosDbOptions.ContainerName);
                }

                return new TestFormStorageProvider();
            });
            
            services.AddSingleton<IStaticContentProvider, TestStaticContentProvider>();
            services.AddSingleton<IFormProvider, TestFormProvider>();
            return services;
        }
    }
}