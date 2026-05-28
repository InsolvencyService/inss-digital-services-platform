using Inss.Common.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Polly.Retry;

namespace Inss.Common.Infrastructure;

public sealed class Resilience
{
    public static AsyncRetryPolicy<HttpResponseMessage> GetRetryPolicy(IServiceProvider serviceProvider, int retryCount)
    {
        var jitter = new Random();
        ILogger<Resilience> logger = serviceProvider.GetRequiredService<ILogger<Resilience>>();
            
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                retryCount: retryCount,
                sleepDurationProvider: retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) // exponential
                    + TimeSpan.FromMilliseconds(jitter.Next(0, 1000)), // jitter
                onRetry: (outcome, timespan, retryAttempt, _) =>
                {
                    logger.RetryOccurred(retryAttempt, timespan.TotalSeconds,
                        outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString());
                });
    }

    public static AsyncCircuitBreakerPolicy<HttpResponseMessage> GetCircuitBreaker(
        IServiceProvider serviceProvider, 
        int countBeforeBreaking, 
        int breakDurationSeconds)
    {
        ILogger<Resilience> logger = serviceProvider.GetRequiredService<ILogger<Resilience>>();
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: countBeforeBreaking,
                durationOfBreak: TimeSpan.FromSeconds(breakDurationSeconds),
                onBreak: (outcome, breakDelay) =>
                {
                    logger.CircuitOpen(breakDelay.TotalSeconds, outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString());
                },
                onReset: logger.CircuitClosed,
                onHalfOpen: logger.CircuitHalfOpen
            );
    }
}