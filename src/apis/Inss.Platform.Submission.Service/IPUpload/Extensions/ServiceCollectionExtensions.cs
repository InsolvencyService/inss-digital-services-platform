using System.Net.Http.Headers;
using System.Net.Mime;
using Azure.Identity;
using Azure.Storage.Blobs;
using Inss.Common.Infrastructure;
using Inss.Common.IPUpload;
using Inss.Platform.Submission.Service.Handlers;
using Inss.Platform.Submission.Service.Infrastructure.Serialization;
using Inss.Platform.Submission.Service.IPUpload.Clients;
using Inss.Platform.Submission.Service.IPUpload.Mapping;
using Inss.Platform.Submission.Service.IPUpload.Persistence;
using Inss.Platform.Submission.Service.IPUpload.Processing;
using Inss.Platform.Submission.Service.IPUpload.Services;
using Inss.Platform.Submission.Service.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web.TokenCacheProviders.InMemory;

namespace Inss.Platform.Submission.Service.IPUpload.Extensions;

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
                services.AddSingleton<IUploadContentBlobClient, MockUploadContentBlobClient>();
            }
            else
            {
                CosmosDbOptions cosmosDbOptions = new();
                context.Configuration.GetSection("CosmosDb").Bind(cosmosDbOptions);
                
                if (!string.IsNullOrWhiteSpace(cosmosDbOptions.ConnectionString))
                {
                    CosmosClientOptions options = new() { Serializer = new CosmosModelSerializer() };
                    CosmosClient cosmosClient = new(cosmosDbOptions.ConnectionString, options);
                    services.AddTransient<IDynamicsStoreProvider>(
                    _ => new DynamicsStoreProvider(cosmosClient, cosmosDbOptions.DatabaseName, cosmosDbOptions.ContainerName));
                }
                else if (!string.IsNullOrWhiteSpace(cosmosDbOptions.AccountEndpoint))
                {
                    CosmosClientOptions options = new() { Serializer = new CosmosModelSerializer() };
                    CosmosClient cosmosClient = new(cosmosDbOptions.AccountEndpoint, new DefaultAzureCredential(), options);
                    services.AddTransient<IDynamicsStoreProvider>(
                    _ => new DynamicsStoreProvider(cosmosClient, cosmosDbOptions.DatabaseName, cosmosDbOptions.ContainerName));
                }
                else
                {
                    throw new InvalidOperationException("No connection string or account endpoint for CosmosDb has been provided.");
                }
                
                DynamicsOptions dynamicsOptions = context.Configuration.GetSection("Dynamics").Get<DynamicsOptions>()!;
            
                services.AddHttpClient<IDynamicsClient, DynamicsClient>(client =>
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
                
                UploadBlobOptions uploadBlobOptions = new();
                context.Configuration.GetSection("UploadBlob").Bind(uploadBlobOptions);
                
                services.AddTransient<IUploadContentBlobClient>(
                    _ => new UploadContentBlobClient(new BlobServiceClient(uploadBlobOptions.ConnectionString)));
            }

            services.AddTransient<INotifyEmailService, NotifyEmailService>();
            services.AddSingleton<IBackgroundDynamicsQueue, BackgroundDynamicsQueue>();
            services.AddHostedService<QueuedDynamicsHostedService>();

            services.AddInMemoryTokenCaches();

            return services;
        }
    }
}