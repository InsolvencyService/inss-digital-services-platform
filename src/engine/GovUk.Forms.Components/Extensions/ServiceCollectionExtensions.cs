using System.Diagnostics.CodeAnalysis;
using GovUk.Forms.Components.Options;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Polly.Retry;

namespace GovUk.Forms.Components.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddTypedClient<TClientInterface, TClient>(
            ExternalApiOptions options, 
            HttpClientHandler? clientHandler = null) 
            where TClient : class, TClientInterface 
            where TClientInterface : class
        {
            services.AddHttpClient<TClientInterface, TClient>(client =>
                {
                    client.BaseAddress = new Uri(options.Url);
                })
                .ConfigurePrimaryHttpMessageHandler(() => clientHandler ?? new HttpClientHandler())
                .SetHandlerLifetime(TimeSpan.FromMinutes(options.LifetimeMinutes))
                .AddPolicyHandler(IServiceCollection.GetRetryPolicy(options))
                .AddPolicyHandler((IServiceCollection.GetCircuitBreaker(options)));
            return services;
        }
        
        private static AsyncRetryPolicy<HttpResponseMessage> GetRetryPolicy(ExternalApiOptions options)
        {
            var jitter = new Random();
            
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                    retryCount: options.RetryCount,
                    sleepDurationProvider: retryAttempt =>
                        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) // exponential
                        + TimeSpan.FromMilliseconds(jitter.Next(0, 1000)), // jitter
                    onRetry: (outcome, timespan, retryAttempt, _) =>
                    {
                        Console.WriteLine($"Retry {retryAttempt} after {timespan.TotalSeconds:F1}s " +
                                          $"due to {outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()}");
                    });
        }

        private static AsyncCircuitBreakerPolicy<HttpResponseMessage> GetCircuitBreaker(ExternalApiOptions options)
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: options.CountBeforeBreaking,
                    durationOfBreak: TimeSpan.FromSeconds(options.BreakDurationSeconds),
                    onBreak: (outcome, breakDelay) =>
                    {
                        Console.WriteLine($"Circuit broken for {breakDelay.TotalSeconds}s " +
                                          $"due to {outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()}");
                    },
                    onReset: () => Console.WriteLine("Circuit reset."),
                    onHalfOpen: () => Console.WriteLine("Circuit in half-open state.")
                );
        }
    }
}