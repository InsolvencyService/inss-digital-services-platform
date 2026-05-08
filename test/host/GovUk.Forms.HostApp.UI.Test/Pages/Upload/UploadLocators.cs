namespace GovUk.Forms.HostApp.UI.Test.Pages.Upload;

public class UploadLocators
{
    public class Labels
    {
        public const string UploadFile = "Upload a file";
        public const string NoFileChosen = "No file chosen";
        public const string Guidance = "Guidance";
        public const string ThereIsAProblem = "There is a problem";
        public const string RPSStakeholderEmail = "RPS.Stakeholder@insolvency.gov.uk";
        public const string CommonIssuesWhenUploadingRP14AForms = "Common issues when uploading RP14/A forms";

        // Error Page
        public const string ErrorPageTitle = "Your form has errors";
        public const string CaseReference = "Case reference";
        public const string AffectedEmployees = "Affected employees";
    }

    public class Selectors
    {
        public const string UploadForm = "#Contents";
        public const string FileInput = "input[type='file']";
        public const string UploadStatus = ".govuk-file-upload-button__status";
        public const string ErrorSummary = ".govuk-error-summary";
        public const string ErrorGroupForm = ".govuk-form-group.govuk-form-group--error";
        public const string ContentError = "#Contents-error";

        // error Page
        public const string ErrorRowSelector = ".govuk-summary-list__row";
        public const string HintSelector = ".govuk-hint";
        public const int ErrorCountParseIndex = 0;
        public const string UploadFileName = "p.govuk-body strong";
    }
}
