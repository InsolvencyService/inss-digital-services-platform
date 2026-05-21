using System.Diagnostics.CodeAnalysis;
using System.Net;
using Inss.Common.Infrastructure;
using Inss.FormsSubmission.Service.Options;

namespace Inss.FormsSubmission.Service.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddTypedClient<TClientInterface, TClient>(ExternalApiOptions options) 
            where TClient : class, TClientInterface 
            where TClientInterface : class
        {
            services.AddHttpClient<TClientInterface, TClient>(client =>
                {
                    client.BaseAddress = new Uri(options.Url);
                })
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    HttpClientHandler clientHandler = new() { AllowAutoRedirect = options.AllowAutoRedirect };

                    if (options.CreateCookieContainer)
                    {
                        clientHandler.CookieContainer = new CookieContainer();
                    }
                    
                    return clientHandler;
                })
                .SetHandlerLifetime(TimeSpan.FromMinutes(options.LifetimeMinutes))
                .AddPolicyHandler(Resilience.GetRetryPolicy(options.RetryCount))
                .AddPolicyHandler((Resilience.GetCircuitBreaker(options.CountBeforeBreaking, options.BreakDurationSeconds)));
            return services;
        }
    }
}