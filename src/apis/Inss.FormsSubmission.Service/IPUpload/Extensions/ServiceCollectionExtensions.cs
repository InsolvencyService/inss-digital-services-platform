using System.Net.Http.Headers;
using System.Net.Mime;
using Azure.Identity;
using Inss.Common.Infrastructure;
using Inss.Common.IPUpload;
using Inss.FormsSubmission.Service.Handlers;
using Inss.FormsSubmission.Service.Infrastructure.Serialization;
using Inss.FormsSubmission.Service.IPUpload.Clients;
using Inss.FormsSubmission.Service.IPUpload.Mapping;
using Inss.FormsSubmission.Service.IPUpload.Persistence;
using Inss.FormsSubmission.Service.IPUpload.Processing;
using Inss.FormsSubmission.Service.Options;
using Microsoft.Azure.Cosmos;
using Microsoft.Identity.Web.TokenCacheProviders.InMemory;

namespace Inss.FormsSubmission.Service.IPUpload.Extensions;

internal static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        internal IServiceCollection AddIPUploadServices(WebHostBuilderContext context)
        {
            services.AddSingleton<IMapperFactory, MapperFactory>();
            services.AddTransient<IHandler<SubmitIPUploadRequest, SubmitIPUploadResponse>, SubmitIPUploadHandler>();

            if (context.HostingEnvironment.IsDevelopment())
            {
                services.AddSingleton<IDynamicsStoreProvider, MockDynamicsStoreProvider>();
                services.AddHttpClient<IDynamicsClient, MockDynamicsClient>();
            }
            else
            {
                CosmosDbOptions cosmosDbOptions = new();
                context.Configuration.GetSection("CosmosDb").Bind(cosmosDbOptions);
                
                CosmosClientOptions options = new() { Serializer = new CosmosModelSerializer() };
                CosmosClient cosmosClient = cosmosDbOptions.ConnectionString is not null
                    ? new CosmosClient(cosmosDbOptions.ConnectionString, options)
                    : new CosmosClient(cosmosDbOptions.AccountEndpoint, new DefaultAzureCredential(), options);
                services.AddTransient<IDynamicsStoreProvider>(
                    _ => new DynamicsStoreProvider(cosmosClient, cosmosDbOptions.DatabaseName, cosmosDbOptions.ContainerName));
                
                DynamicsOptions dynamicsOptions = context.Configuration.GetSection("Dynamics").Get<DynamicsOptions>()!;
            
                services.AddHttpClient<IDynamicsClient, MockDynamicsClient>(client =>
                    {
                        client.BaseAddress = new Uri($"{dynamicsOptions.Url}/");
                        client.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                        client.DefaultRequestHeaders.Add("OData-Version", "4.0");
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
                    })
                    .ConfigurePrimaryHttpMessageHandler(() => new DynamicsAuthDelegatingHandler(dynamicsOptions))
                    .SetHandlerLifetime(TimeSpan.FromMinutes(dynamicsOptions.LifetimeMinutes))
                    .AddPolicyHandler((sp, _) => Resilience.GetRetryPolicy(sp, dynamicsOptions.RetryCount))
                    .AddPolicyHandler((sp, _) => Resilience.GetCircuitBreaker(
                        sp, dynamicsOptions.CountBeforeBreaking, dynamicsOptions.BreakDurationSeconds));
            }
            
            services.AddSingleton<IBackgroundDynamicsQueue, BackgroundDynamicsQueue>();
            services.AddHostedService<QueuedDynamicsHostedService>();

            services.AddInMemoryTokenCaches();

            return services;
        }
    }
}