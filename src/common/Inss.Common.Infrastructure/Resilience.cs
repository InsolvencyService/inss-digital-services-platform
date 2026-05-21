using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Polly.Retry;

namespace Inss.Common.Infrastructure;

public static class Resilience
{
    public static AsyncRetryPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount)
    {
        var jitter = new Random();
            
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                retryCount: retryCount,
                sleepDurationProvider: retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) // exponential
                    + TimeSpan.FromMilliseconds(jitter.Next(0, 1000)), // jitter
                onRetry: (outcome, timespan, retryAttempt, _) =>
                {
                    Console.WriteLine($"Retry {retryAttempt} after {timespan.TotalSeconds:F1}s " +
                                      $"due to {outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()}");
                });
    }

    public static AsyncCircuitBreakerPolicy<HttpResponseMessage> GetCircuitBreaker(int countBeforeBreaking, int breakDurationSeconds)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                handledEventsAllowedBeforeBreaking: countBeforeBreaking,
                durationOfBreak: TimeSpan.FromSeconds(breakDurationSeconds),
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