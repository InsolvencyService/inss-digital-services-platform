namespace Inss.FormsSubmission.Service.Extensions;

public static partial class LoggerExtensions
{
    [LoggerMessage(EventId = 500, Level = LogLevel.Information, Message = "Token validated successfully.")]
    public static partial void TokenValidated(this ILogger logger);
    
    [LoggerMessage(EventId = 501, Level = LogLevel.Error, Message = "Token failed to validate: {Error}")]
    public static partial void TokenValidationFailed(this ILogger logger, string error);
    
    [LoggerMessage(EventId = 502, Level = LogLevel.Information, Message = "Submitting IP upload to Dynamics.")]
    public static partial void SubmittingIPUpload(this ILogger logger);
    
    [LoggerMessage(EventId = 503, Level = LogLevel.Error, Message = "Dynamics backgrond task unexpectedly failed: {Error}.")]
    public static partial void DynamicsBackgroundTaskFailed(this ILogger logger, string error);
}