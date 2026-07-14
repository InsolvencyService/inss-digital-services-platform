namespace Inss.FormsSubmission.Service.IPUpload.Services;

public interface INotifyEmailService
{
    void SendExternalEmail(
        string email,
        string reference,
        DateTimeOffset submissionDate,
        bool isEmployeeSubmission,
        DynamicsSubmission[] submissions);

    void SendInternalEmail(
        string reference,
        DateTimeOffset submissionDate,
        bool isEmployeeSubmission,
        DynamicsSubmission[] submissions);
}