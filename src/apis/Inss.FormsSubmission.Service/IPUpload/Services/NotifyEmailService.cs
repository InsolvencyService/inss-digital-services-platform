using System.Globalization;
using System.Text;
using Inss.FormsSubmission.Service.Extensions;
using Inss.FormsSubmission.Service.Options;
using Microsoft.Extensions.Options;
using Notify.Interfaces;
using Notify.Models.Responses;

namespace Inss.FormsSubmission.Service.IPUpload.Services;

public sealed class NotifyEmailService : INotifyEmailService
{
    private readonly INotificationClient _notificationClient;
    private readonly IOptions<NotifyOptions> _notifyOptions;
    private readonly ILogger<NotifyEmailService> _logger;
    private const string RP14A = "RP14A";
    private const string RP14 = "RP14";
    private const string Succeeded = "succeeded";
    private const string Failed = "failed";
    private const string FormTypeKey = "formType";
    private const string ReferenceNumberKey = "referenceNumber";
    private const string SucceededFailedKey = "succeeded/failed";
    private const string UploadDateAndTimeKey = "uploadDateAndTime";
    private const string ErrorCollectionKey = "errorCollection";
    
    public NotifyEmailService(
        INotificationClient notificationClient, 
        IOptions<NotifyOptions> notifyOptions, 
        ILogger<NotifyEmailService> logger)
    {
        _notificationClient = notificationClient;
        _notifyOptions = notifyOptions;
        _logger = logger;
    }
    
    public void SendExternalEmail(
        string email,
        string reference, 
        DateTimeOffset submissionDate,
        bool isEmployeeSubmission, 
        DynamicsSubmission[] submissions)
    {
        bool submissionFailed = submissions.Any(s => s.ErrorInfo is not null);
        Dictionary<string, dynamic> personalisation = new()
        {
            { FormTypeKey, isEmployeeSubmission ? RP14A : RP14 },
            { ReferenceNumberKey, reference },
            { SucceededFailedKey, submissionFailed ? Failed : Succeeded },
            { UploadDateAndTimeKey, $"{submissionDate:F}" }
        };
        
        EmailNotificationResponse response = _notificationClient.SendEmail(
            email, 
            _notifyOptions.Value.IPUploadExternalTemplateId,
            personalisation);

        if (string.IsNullOrWhiteSpace(response.id))
        {
            _logger.ExternalEmailFailed(email, reference);
        }
    }
    
    public void SendInternalEmail(
        string reference, 
        DateTimeOffset submissionDate,
        bool isEmployeeSubmission, 
        DynamicsSubmission[] submissions)
    {
        string errors = BuildInternalErrors(submissions);
        _logger.SubmissionFailedErrors(reference, errors);
        Dictionary<string, dynamic> personalisation = new()
        {
            { FormTypeKey, isEmployeeSubmission ? RP14A : RP14 },
            { ReferenceNumberKey, reference },
            { UploadDateAndTimeKey, $"{submissionDate:F}" },
            { ErrorCollectionKey, errors }
        };
        
        EmailNotificationResponse response = _notificationClient.SendEmail(
            _notifyOptions.Value.IPUploadInternalEmail, 
            _notifyOptions.Value.IPUploadInternalTemplateId,
            personalisation);

        if (string.IsNullOrWhiteSpace(response.id))
        {
            _logger.InternalEmailFailed(_notifyOptions.Value.IPUploadInternalEmail, reference);
        }
    }

    private static string BuildInternalErrors(DynamicsSubmission[] submissions)
    {
        const string markdownBullet = "- ";
        StringBuilder errorMessage = new(Environment.NewLine);

        foreach (DynamicsSubmission submission in submissions.Where(s => s.ErrorInfo is not null))
        {
            ErrorInfo errorInfo = submission.ErrorInfo!;
            errorMessage.Append(markdownBullet);
            errorMessage.AppendLine(CultureInfo.InvariantCulture, $"Correlation ID: {submission.Id}");
            errorMessage.AppendLine(CultureInfo.InvariantCulture, $"Error Code: {errorInfo.Error.Code}");
            errorMessage.AppendLine(CultureInfo.InvariantCulture, $"Error Message: {errorInfo.Error.Message}");
            errorMessage.AppendLine();
        }
        
        return errorMessage.ToString();
    }
}