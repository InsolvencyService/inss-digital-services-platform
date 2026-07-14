using Microsoft.Extensions.Logging;

namespace Inss.Common.Infrastructure.Extensions;

public static partial class LoggerExtensions
{
    [LoggerMessage(EventId = 600, Level = LogLevel.Information, Message = "Retry {RetryAttempt} after {TotalSeconds}s due to \"due to {Message}.")]
    public static partial void RetryOccurred(this ILogger logger, int retryAttempt, double totalSeconds, string message);
    
    [LoggerMessage(EventId = 601, Level = LogLevel.Warning, Message = "Circuit now open for {TotalSeconds}s due to {Message}.")]
    public static partial void CircuitOpen(this ILogger logger, double totalSeconds, string message);
    
    [LoggerMessage(EventId = 602, Level = LogLevel.Information, Message = "Circuit closed.")]
    public static partial void CircuitClosed(this ILogger logger);
    
    [LoggerMessage(EventId = 603, Level = LogLevel.Information, Message = "Circuit half open.")]
    public static partial void CircuitHalfOpen(this ILogger logger);

}