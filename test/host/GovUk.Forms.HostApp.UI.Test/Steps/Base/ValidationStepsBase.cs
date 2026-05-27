using GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Support;
using GovUk.Forms.HostApp.UI.Test.Tags;
using System.Globalization;


namespace GovUk.Forms.HostApp.UI.Test.Steps.Base;

public abstract class ValidationStepsBase
{
    protected UploadDocumentCoordinator UploadDocumentCoordinator { get; }
    protected UploadErrorDetailsCoordinator UploadErrorDetailsCoordinator { get; }
    protected ScenarioContext ScenarioContext { get; }
    protected const string AffectedEmployeesKey = "AffectedEmployees";
    protected const string InvalidBasicPayPerWeekKey = "InvalidBasicPayPerWeek";
    protected const string InvalidHolidayOwedKey = "InvalidHolidayOwed";
    protected const string AffectedEmployeesByErrorTypeKey = "AffectedEmployeesByErrorType";
    protected const string UploadErrorsContextKey = "UploadErrors";
    protected const string InvalidArrearsCountKey = "InvalidArrearsOfPayOwedCount";


    protected ValidationStepsBase(
        UploadDocumentCoordinator uploadDocumentCoordinator,
        UploadErrorDetailsCoordinator uploadErrorDetailsCoordinator,
        ScenarioContext scenarioContext)
    {
        UploadDocumentCoordinator = uploadDocumentCoordinator;
        UploadErrorDetailsCoordinator = uploadErrorDetailsCoordinator;
        ScenarioContext = scenarioContext;
    }

    [When("I attempt to submit the RP14A")]
    public async Task WhenIAttemptToSubmitTheRp14a()
    {
        await UploadDocumentCoordinator.NavigateToSubmitPageAsync();
    }

    protected async Task VerifySingleEmployeeErrorDetailsAsync(
        ErrorDetailsHeaderType headerType)
    {
        UploadErrorSummary expectedError =
            ScenarioContext.Get<UploadErrorSummary>();

        AffectedEmployee affectedEmployee =
            ScenarioContext.Get<AffectedEmployee>();

        await UploadErrorDetailsCoordinator.VerifyErrorDetailsAsync(
            expectedError,
            affectedEmployee,
            headerType);
    }

    protected static UploadErrorSummary CreateEmployeeErrorSummary(
        string errorType,
        string errorMessage,
        string? hintText = null)
    {
        return new UploadErrorSummary(
            Category: "Employee",
            ErrorType: errorType,
            ErrorMessage: errorMessage,
            HintText: hintText);
    }

    protected static UploadErrorSummary CreateErrorSummary(
        string type,
        string message,
        string? hint = null,
        string category = "")
    {
        return new UploadErrorSummary(
            Category: category,
            ErrorType: type,
            ErrorMessage: message,
            HintText: hint);
    }

    protected static string UiDateOfBirth()
    {
        return DateTime
            .ParseExact(
                ScenarioConstant.DOB,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture)
            .ToString("d/M/yyyy", CultureInfo.InvariantCulture);
    }

    protected static string FormatDateRange(string start, string end)
    {
        return $"{DateTime.Parse(start, CultureInfo.InvariantCulture):M/d/yyyy}, {DateTime.Parse(end, CultureInfo.InvariantCulture):M/d/yyyy}";
    }


    protected static string FormatDateRange(
    DateOnly? startDate,
    DateOnly? endDate)
    {
        return $"{FormatUiDate(startDate)}, {FormatUiDate(endDate)}";
    }

    protected static DateOnly? ParseDateOrNull(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            value.Equals("<empty>", StringComparison.OrdinalIgnoreCase) ||
            value.Equals("<null>", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        if (!DateOnly.TryParseExact(
                value,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateOnly parsedDate))
        {
            throw new ArgumentException(
                $"Date '{value}' must be in yyyy-MM-dd format.",
                nameof(value));
        }

        return parsedDate;
    }

    protected static string FormatUiDate(DateOnly? date)
    {
        return date?.ToString(
                   "M/d/yyyy",
                   CultureInfo.InvariantCulture)
               ?? string.Empty;
    }

}
