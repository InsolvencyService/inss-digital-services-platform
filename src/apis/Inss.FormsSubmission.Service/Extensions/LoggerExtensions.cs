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
    
    [LoggerMessage(EventId = 504, Level = LogLevel.Information, Message = "Submitting message {Id} and {Reference} to Dynamics.")]
    public static partial void SubmittingDynamicsMessage(this ILogger logger, string id, string reference);
    
    [LoggerMessage(EventId = 505, Level = LogLevel.Information, Message = "Updating the Dynamics response for {Id} and {Reference}.")]
    public static partial void UpdatingDynamicsResponseInStore(this ILogger logger, string id, string reference);
    
    [LoggerMessage(EventId = 506, Level = LogLevel.Information, Message = "Sending email via Gov Notify for {Reference}.")]
    public static partial void SendingGovNotifyEmail(this ILogger logger, string reference);
    
    [LoggerMessage(EventId = 507, Level = LogLevel.Error, Message = "Unable to find the stored message for {Id} and {Reference}.")]
    public static partial void StoredMessageNotFound(this ILogger logger, string id, string reference);
    
    [LoggerMessage(EventId = 508, Level = LogLevel.Information, Message = "Submitted message to Dynamics successfully for {CorrelationId}.")]
    public static partial void SuccessfulSubmissionToDynamics(this ILogger logger, string correlationId);
    
    [LoggerMessage(EventId = 509, Level = LogLevel.Error, Message = "Failed to successfully submit message to Dynamics for {CorrelationId}.")]
    public static partial void FailedSubmissionToDynamics(this ILogger logger, string correlationId);
    
    [LoggerMessage(EventId = 510, Level = LogLevel.Information, Message = "Loading all the submitted Dynamics messages for {Reference}.")]
    public static partial void LoadSubmittedDynamicsMessages(this ILogger logger, string reference);
    
    [LoggerMessage(EventId = 511, Level = LogLevel.Error, Message = "Failed to send an email for {Reference}.")]
    public static partial void EmailFailed(this ILogger logger, string reference);
}