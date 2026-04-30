using Microsoft.Extensions.Logging;

namespace GovUk.Forms.Components.Extensions;

public static partial class LoggerExtensions
{
    [LoggerMessage(EventId = 200, Level = LogLevel.Error, Message = "Unexpected error occurred in request {Path}: {Message}.")]
    public static partial void UnexpectedError(this ILogger logger, string path, string message);
    
    [LoggerMessage(EventId = 201, Level = LogLevel.Error, Message = "Unexpected error occurred authenticating with the broker: {Message}.")]
    public static partial void AuthenticationFailed(this ILogger logger, string message);
    
    [LoggerMessage(EventId = 203, Level = LogLevel.Error, Message = "Status code error occurred in request {Path}: {StatusCode}.")]
    public static partial void StatusCodeError(this ILogger logger, string path, int statusCode);
}