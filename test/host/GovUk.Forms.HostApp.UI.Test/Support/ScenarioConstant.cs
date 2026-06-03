namespace GovUk.Forms.HostApp.UI.Test.Support;

public sealed class ScenarioConstant
{
    public const string StartPage = nameof(StartPage);
    public const string SnapShots = nameof(SnapShots);
    public const string DeclarationPage = nameof(DeclarationPage);
    public const string UploadPage = nameof(UploadPage);
    public const string UploadPageWithWarning = nameof(UploadPageWithWarning);
    public const string UploadPageWithCommonIssuesSection = nameof(UploadPageWithCommonIssuesSection);

    // TODO: move test credentials to appsettings.json / environment variables
    public const string EmailAddress = "test1@gmail.com";
    public const string Password = "password";

    public const string UploadedFilePath = "UploadedFilePath";
    public const string UploadedFileName = "UploadedFileName";
    public const string ErrorMessage = "ErrorMessage";
    public const string SubmissionOutcome = "SubmissionOutcome";

    public const int ElementTimeout = 8000;
    public const int WaitForVisual = 1000;

    // Default employee details
    public const string Surname = "Surname Test";
    public const string Forename = "Forname Test";
    public const string Title = "Mr";
    public const string EmployerName = "Employer Test";
    public const string DOB = "1990-01-01";
    public const string NationalInsuranceNumber = "BP011752C";
}
