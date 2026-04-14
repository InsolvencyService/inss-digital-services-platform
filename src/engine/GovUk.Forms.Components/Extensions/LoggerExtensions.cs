using Microsoft.Extensions.Logging;

namespace GovUk.Forms.Components.Extensions;

public static partial class LoggerExtensions
{
    [LoggerMessage(EventId = 200, Level = LogLevel.Information, Message = "Unexpected error occurred in request {Path}: {Message}.")]
    public static partial void UnexpectedError(this ILogger logger, string path, string message);
    
    [LoggerMessage(EventId = 201, Level = LogLevel.Error, Message = "Unexpected error occurred authenticating with the broker: {Message}.")]
    public static partial void AuthenticationFailed(this ILogger logger, string message);
}