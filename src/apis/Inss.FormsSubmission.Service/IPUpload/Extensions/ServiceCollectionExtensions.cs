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

            if (!context.HostingEnvironment.IsDevelopment())
            {
                services.AddSingleton<IDynamicsStoreProvider, MockDynamicsStoreProvider>();
            }
            else
            {
                CosmosDbOptions cosmosDbOptions = new();
                context.Configuration.GetSection("CosmosDb").Bind(cosmosDbOptions);
                
                CosmosClientOptions options = new() { Serializer = new CosmosModelSerializer() };
                CosmosClient client = cosmosDbOptions.ConnectionString is not null
                    ? new CosmosClient(cosmosDbOptions.ConnectionString, options)
                    : new CosmosClient(cosmosDbOptions.AccountEndpoint, new DefaultAzureCredential(), options);
                services.AddTransient<IDynamicsStoreProvider>(
                    _ => new DynamicsStoreProvider(client, cosmosDbOptions.DatabaseName, cosmosDbOptions.ContainerName));
            }

            DynamicsOptions dynamicsOptions = context.Configuration.GetSection("Dynamics").Get<DynamicsOptions>()!;
            
            services.AddHttpClient<IDynamicsClient, MockDynamicsClient>(client =>
                {
                    client.BaseAddress = new Uri(dynamicsOptions.Url);
                })
                .ConfigurePrimaryHttpMessageHandler(() => new DynamicsAuthDelegatingHandler(dynamicsOptions))
                .SetHandlerLifetime(TimeSpan.FromMinutes(dynamicsOptions.LifetimeMinutes))
                .AddPolicyHandler(Resilience.GetRetryPolicy(dynamicsOptions.RetryCount))
                .AddPolicyHandler((Resilience.GetCircuitBreaker(dynamicsOptions.CountBeforeBreaking, dynamicsOptions.BreakDurationSeconds)));
            
            services.AddSingleton<IBackgroundDynamicsQueue, BackgroundDynamicsQueue>();
            services.AddHostedService<QueuedDynamicsHostedService>();

            services.AddInMemoryTokenCaches();

            return services;
        }
    }
}

/*
public sealed class DynamicsAuthDelegatingHandler : DelegatingHandler
{
    private readonly DynamicOptions _options;
    private readonly AuthenticationContext _context;

    public DynamicsAuthDelegatingHandler(DynamicOptions options)
    {
        _options = options;
        _context = new AuthenticationContext(Authority, false);
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Aquire a token
        // Add to header
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "");//_auth.AcquireToken().AccessToken);
        return base.SendAsync(request, cancellationToken);
    }
}*/