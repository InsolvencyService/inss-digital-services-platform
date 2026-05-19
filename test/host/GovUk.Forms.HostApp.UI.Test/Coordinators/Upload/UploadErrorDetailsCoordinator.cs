using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Pages.Upload;
using GovUk.Forms.HostApp.UI.Test.Support;
using System.Globalization;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;

public sealed class UploadErrorDetailsCoordinator
{
    private const string CountPlaceholder = "[COUNT]";

    private readonly IUploadErrorDetailsPage _uploadErrorDetailsPage;
    private readonly IUploadErrorsPage _uploadErrorsPage;
    private readonly IPlaywrightDriver _playwrightDriver;
    private readonly ScenarioContext _scenarioContext;
    private readonly IAllureReportingHelper _reportingHelper;

    public UploadErrorDetailsCoordinator(
        IUploadErrorDetailsPage uploadErrorDetailsPage,
        IUploadErrorsPage uploadErrorsPage,
        IPlaywrightDriver playwrightDriver,
        ScenarioContext scenarioContext,
        IAllureReportingHelper allure)
    {
        _uploadErrorDetailsPage = uploadErrorDetailsPage;
        _uploadErrorsPage = uploadErrorsPage;
        _playwrightDriver = playwrightDriver;
        _scenarioContext = scenarioContext;
        _reportingHelper = allure;
    }

    public async Task VerifyUploadErrorPageIsDisplayedAsync()
    {
        await _uploadErrorsPage.WaitForPageToLoadAsync();
    }

    public async Task VerifyUploadErrorDetailsPageIsDisplayedAsync()
    {
        await _uploadErrorDetailsPage.WaitForPageToLoadAsync();
    }

    public async Task VerifyErrorSummaryIsDisplayedAsync(
        UploadErrorSummary expectedError)
    {
        ArgumentNullException.ThrowIfNull(expectedError);

        string expectedFileName = GetUploadedFileName();

        await _reportingHelper.StepAsync("Verify upload errors summary page", async () =>
        {
            await _uploadErrorsPage.WaitForPageToLoadAsync();
            await _uploadErrorsPage.VerifyUploadedFileNameAsync(expectedFileName);
            await _uploadErrorsPage.VerifyErrorSummaryAsync(expectedError);

            await _reportingHelper.AttachScreenshotAsync(
                _playwrightDriver.Page,
                "Upload Errors Page");
        });
    }

    public async Task OpenErrorDetailsAsync(
        UploadErrorSummary expectedError)
    {
        ArgumentNullException.ThrowIfNull(expectedError);

        await _reportingHelper.StepAsync("Open upload error details page", async () =>
        {
            await _uploadErrorsPage.WaitForPageToLoadAsync();
            await _uploadErrorsPage.ClickOnViewDetailsAsync(expectedError);

            await _reportingHelper.AttachScreenshotAsync(
                _playwrightDriver.Page,
                "Upload Errors Summary Page");
        });
    }

    public async Task VerifyValidationErrorAsync(string expectedError)
    {
        if (string.IsNullOrWhiteSpace(expectedError))
        {
            throw new ArgumentException(
                "Expected error cannot be null or empty.",
                nameof(expectedError));
        }

        await _reportingHelper.StepAsync("Verify validation error", async () =>
        {
            await _uploadErrorsPage.WaitForPageToLoadAsync();

            string resolvedError = await ResolveValidationErrorMessageAsync(
                expectedError);

            await _uploadErrorsPage.VerifyErrorMessageAsync(resolvedError);

            await _reportingHelper.AttachScreenshotAsync(
                _playwrightDriver.Page,
                "Validation Error");
        });
    }

    public async Task<string> ResolveValidationErrorMessageAsync(
        string expectedError)
    {
        if (string.IsNullOrWhiteSpace(expectedError))
        {
            throw new ArgumentException(
                "Expected error cannot be null or empty.",
                nameof(expectedError));
        }

        if (!expectedError.Contains(
                CountPlaceholder,
                StringComparison.Ordinal))
        {
            return expectedError;
        }

        string errorKey = ExtractErrorKey(expectedError);

        int count = await _uploadErrorsPage.GetErrorCountAsync(errorKey);

        return expectedError.Replace(
            CountPlaceholder,
            count.ToString(CultureInfo.InvariantCulture),
            StringComparison.Ordinal);
    }

    public async Task VerifyErrorDetailsAsync(
        UploadErrorSummary expectedError,
        AffectedEmployee affectedEmployee,
        ErrorDetailsHeaderType headerType)
    {
        ArgumentNullException.ThrowIfNull(expectedError);
        ArgumentNullException.ThrowIfNull(affectedEmployee);

        await VerifyErrorDetailsInternalAsync(
            expectedError,
            [affectedEmployee],
            headerType);
    }

    public async Task VerifyErrorDetailsAsync(
        UploadErrorSummary expectedError,
        IReadOnlyCollection<AffectedEmployee> affectedEmployees,
        ErrorDetailsHeaderType headerType)
    {
        ArgumentNullException.ThrowIfNull(expectedError);
        ArgumentNullException.ThrowIfNull(affectedEmployees);

        if (affectedEmployees.Count == 0)
        {
            throw new ArgumentException(
                "At least one affected employee must be provided.",
                nameof(affectedEmployees));
        }

        await VerifyErrorDetailsInternalAsync(
            expectedError,
            affectedEmployees,
            headerType);
    }

    public async Task ClickBackAndVerifyUploadErrorPageIsDisplayedAsync()
    {
        await _reportingHelper.StepAsync(
            "Click Back and verify Upload Error page is displayed",
            async () =>
            {
                await _uploadErrorDetailsPage.ClickBackButtonAsync();
                await VerifyUploadErrorPageIsDisplayedAsync();

                await _reportingHelper.AttachScreenshotAsync(
                    _playwrightDriver.Page,
                    "Upload Error Page after clicking Back");
            });
    }

    public async Task NavigateToCheckAnswersPageAsync()
    {
        await _uploadErrorsPage.ClickContinueAsync();
    }

    private async Task VerifyErrorDetailsInternalAsync(
        UploadErrorSummary expectedError,
        IReadOnlyCollection<AffectedEmployee> affectedEmployees,
        ErrorDetailsHeaderType headerType)
    {
        ErrorConfig config = GetErrorConfig(headerType);

        await OpenErrorDetailsAsync(expectedError);

        await _reportingHelper.StepAsync(config.StepName, async () =>
        {
            await VerifyUploadErrorDetailsPageIsDisplayedAsync();
            await VerifyHeaderAsync(headerType);
            await VerifyErrorMessageContentAsync(expectedError);
            await VerifyAffectedEmployeesTableAsync(affectedEmployees);

            await _reportingHelper.AttachScreenshotAsync(
                _playwrightDriver.Page,
                config.ScreenshotName);
        });
    }

    private async Task VerifyHeaderAsync(ErrorDetailsHeaderType headerType)
    {
        Func<Task> verifyAction = headerType switch
        {
            ErrorDetailsHeaderType.EmploymentDates =>
                _uploadErrorDetailsPage
                    .VerifyEmploymentDatesHeaderIsDisplayedAsync,

            ErrorDetailsHeaderType.ArrearsOfPayDates =>
                _uploadErrorDetailsPage
                    .VerifyArrearsOfPayDatesHeaderIsDisplayedAsync,

            ErrorDetailsHeaderType.NationalInsuranceNumber =>
                _uploadErrorDetailsPage
                    .VerifyEmployeeNationalInsuranceNumberHeaderIsDisplayedAsync,

            ErrorDetailsHeaderType.MoneyOwedToEmployer =>
                _uploadErrorDetailsPage
                    .VerifyMoneyOwedToEmployerHeaderIsDisplayedAsync,

            ErrorDetailsHeaderType.EmployeeSurname =>
                _uploadErrorDetailsPage
                    .VerifyEmployeeErrorSummaryIsDisplayedAsync,

            ErrorDetailsHeaderType.ArrearsOfPayOwed =>
                _uploadErrorDetailsPage
                    .VerifyEmployeeArrearsOfPaymentOwedErrorSummaryIsDisplayedAsync,

            ErrorDetailsHeaderType.EmployerName =>
                _uploadErrorDetailsPage
                    .VerifyEmployerErrorSummaryIsDisplayedAsync,
            ErrorDetailsHeaderType.CaseReference =>
                _uploadErrorDetailsPage
                    .VerifyCaseReferenceHeaderIsDisplayedAsync,

            _ => throw new InvalidOperationException(
                $"Unsupported header type: {headerType}")
        };

        await verifyAction();
    }

    private async Task VerifyErrorMessageContentAsync(
        UploadErrorSummary error)
    {
        await _uploadErrorDetailsPage.VerifyErrorMessageAsync(
            error.ErrorMessage);

        if (!string.IsNullOrWhiteSpace(error.HintText))
        {
            await _uploadErrorDetailsPage.VerifyErrorMessageAsync(
                error.HintText);
        }
    }

    private async Task VerifyAffectedEmployeesTableAsync(
        IReadOnlyCollection<AffectedEmployee> affectedEmployees)
    {
        await _uploadErrorDetailsPage
            .VerifyAffectedEmployeeTableHeadersAsync();

        foreach (AffectedEmployee employee in affectedEmployees)
        {
            await _uploadErrorDetailsPage
                .VerifyAffectedEmployeeAsync(employee);
        }
    }

    private string GetUploadedFileName()
    {
        if (!_scenarioContext.TryGetValue(
                ScenarioConstant.UploadedFileName,
                out string? fileName))
        {
            throw new InvalidOperationException(
                $"Scenario context missing required key: {ScenarioConstant.UploadedFileName}");
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new InvalidOperationException(
                $"Scenario context value for '{ScenarioConstant.UploadedFileName}' cannot be null or empty.");
        }

        return fileName;
    }

    private static string ExtractErrorKey(string errorMessage)
    {
        if (string.IsNullOrWhiteSpace(errorMessage))
        {
            throw new ArgumentException(
                "Error message cannot be null or empty.",
                nameof(errorMessage));
        }

        return errorMessage
            .Replace(CountPlaceholder, string.Empty, StringComparison.Ordinal)
            .Trim();
    }

    private static ErrorConfig GetErrorConfig(
        ErrorDetailsHeaderType headerType)
    {
        return headerType switch
        {
            ErrorDetailsHeaderType.NationalInsuranceNumber =>
                new ErrorConfig(
                    "Employee national insurance number error details page",
                    "Employee National Insurance Number Error Details Page"),

            ErrorDetailsHeaderType.MoneyOwedToEmployer =>
                new ErrorConfig(
                    "Money owed to employer error details page",
                    "Money Owed To Employer Error Details Page"),

            ErrorDetailsHeaderType.EmploymentDates =>
                new ErrorConfig(
                    "Employee employment dates error details page",
                    "Employment Dates Error Details Page"),

            ErrorDetailsHeaderType.ArrearsOfPayDates =>
                new ErrorConfig(
                    "Arrears of pay dates error details page",
                    "Arrears Of Pay Dates Error Details Page"),

            ErrorDetailsHeaderType.EmployeeSurname =>
                new ErrorConfig(
                    "Employee surname error details page",
                    "Employee Surname Error Details Page"),

            ErrorDetailsHeaderType.ArrearsOfPayOwed =>
                new ErrorConfig(
                    "Employee arrears of payment owed error details page",
                    "Employee Arrears Of Payment Owed Error Details Page"),

            ErrorDetailsHeaderType.EmployerName =>
                new ErrorConfig(
                    "Employer name error details page",
                    "Employer Name Error Details Page"),
            ErrorDetailsHeaderType.CaseReference =>
                new ErrorConfig(
                    "Case reference error details page",
                    "Case Reference Error Details Page"),

            _ => throw new InvalidOperationException(
                $"No configuration defined for error type: {headerType}")
        };
    }
}

public sealed record ErrorConfig(
    string StepName,
    string ScreenshotName);

public enum ErrorDetailsHeaderType
{
    EmployeeSurname,
    EmployerName,
    EmploymentDates,
    ArrearsOfPayDates,
    ArrearsOfPayOwed,
    NationalInsuranceNumber,
    MoneyOwedToEmployer,
    CaseReference
}

