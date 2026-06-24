namespace GovUk.Forms.HostApp.UI.Test.Pages.Upload;

public class UploadLocators
{
    public class Labels
    {
        public const string UploadFile = "Upload a file";
        public const string NoFileChosen = "No file chosen";
        public const string CommonUploadErrors = "Errors during the upload process";
        public const string ThereIsAProblem = "There is a problem";
        public const string RPSStakeholderEmail = "RPS.Stakeholder@insolvency.gov.uk";
        public const string CommonIssuesWhenUploadingRP14AForms = "Common issues when uploading RP14/A forms";

        // Error Page
        public const string ErrorPageTitle = "Your form has errors";
        public const string CaseReference = "Case reference";
        public const string AffectedEmployees = "Affected employees";

        // Case Reference Number Page
        public const string CaseReferenceNumberHeading = "Whats the case reference number?";
        public const string CaseReferenceNumberHint = "For example, 'CN123456K'. This must match the case reference number in your form.";

        // Employer Details Page
        public const string EmployerDetailsHeading = "Employer details";
        public const string WeHaveMatchedText = "We have matched to the following employer:";
        public const string IsThisTheCorrectEmployerNameHeading = "Is this the correct employer name?";
        public const string MatchesWithEmployerText = "This case reference number you have provided matches with this employer in our system.";
        public const string EmployerNameLabel = "Employer name";
        public const string CaseReferenceNumberLabel = "Case reference number";
        public const string YesOption = "Yes";
        public const string NoOption = "No";
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

        // Case Reference Number Page
        public const string CaseReferenceNumberInput = "#caseReferenceNumber";
    }
}
