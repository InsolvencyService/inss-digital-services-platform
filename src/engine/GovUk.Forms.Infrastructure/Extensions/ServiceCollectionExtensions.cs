using Azure.Identity;
using GovUk.Forms.Application.Providers;
using GovUk.Forms.Infrastructure.Options;
using GovUk.Forms.Infrastructure.Providers;
using GovUk.Forms.Infrastructure.Serialization;
using Microsoft.AspNetCore.Http;
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
            return services;
        }
    }
}