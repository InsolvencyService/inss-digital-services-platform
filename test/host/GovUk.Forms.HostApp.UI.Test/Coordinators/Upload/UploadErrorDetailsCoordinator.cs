using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Pages.Upload;
using GovUk.Forms.HostApp.UI.Test.Support;
using GovUk.Forms.HostApp.UI.Test.Tags;
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
        _uploadErrorDetailsPage = uploadErrorDetailsPage
            ?? throw new ArgumentNullException(nameof(uploadErrorDetailsPage));

        _uploadErrorsPage = uploadErrorsPage
            ?? throw new ArgumentNullException(nameof(uploadErrorsPage));

        _playwrightDriver = playwrightDriver
            ?? throw new ArgumentNullException(nameof(playwrightDriver));

        _scenarioContext = scenarioContext
            ?? throw new ArgumentNullException(nameof(scenarioContext));

        _reportingHelper = allure
            ?? throw new ArgumentNullException(nameof(allure));
    }

    public async Task VerifyUploadErrorPageIsDisplayedAsync()
    {
        await _uploadErrorsPage.WaitForPageToLoadAsync();
    }

    public async Task VerifyUploadErrorDetailsPageIsDisplayedAsync()
    {
        await _uploadErrorDetailsPage.WaitForPageToLoadAsync();
    }

    public async Task VerifyErrorSummaryIsDisplayedAsync(UploadErrorSummary expectedError)
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
    public async Task VerifyErrorDetailsHeaderOnlyAsync(
        UploadErrorSummary expectedError,
        ErrorDetailsHeaderType headerType)
    {
        ArgumentNullException.ThrowIfNull(expectedError);

        (ErrorConfig config, Func<Task> verifyHeader) = GetHeaderConfig(headerType);

        await OpenErrorDetailsAsync(expectedError);

        await _reportingHelper.StepAsync(config.StepName, async () =>
        {
            await VerifyUploadErrorDetailsPageIsDisplayedAsync();
            await verifyHeader();
            await VerifyErrorMessageContentAsync(expectedError);

            await _reportingHelper.AttachScreenshotAsync(
                _playwrightDriver.Page,
                config.ScreenshotName);
        });
    }

    public async Task VerifyValidationCategoryIsDisplayedAsync(string category)
    {
        await _reportingHelper.StepAsync("Verify validation category is displayed", async () =>
        {
            await _uploadErrorsPage.VerifyValidationCategoryIsDisplayedAsync(category);

            await _reportingHelper.AttachScreenshotAsync(
                _playwrightDriver.Page,
                "Validation Category");
        });
    }

    public async Task OpenErrorDetailsAsync(UploadErrorSummary expectedError)
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

            string resolvedError = await ResolveValidationErrorMessageAsync(expectedError);

            await _uploadErrorsPage.VerifyErrorMessageAsync(resolvedError);

            await _reportingHelper.AttachScreenshotAsync(
                _playwrightDriver.Page,
                "Validation Error");
        });
    }

    public async Task<string> ResolveValidationErrorMessageAsync(string expectedError)
    {
        if (string.IsNullOrWhiteSpace(expectedError))
        {
            throw new ArgumentException(
                "Expected error cannot be null or empty.",
                nameof(expectedError));
        }

        if (!expectedError.Contains(CountPlaceholder, StringComparison.Ordinal))
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

        await VerifyErrorDetailsInternalAsync(expectedError, [affectedEmployee], headerType);
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

        await VerifyErrorDetailsInternalAsync(expectedError, affectedEmployees, headerType);
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
        await _reportingHelper.StepAsync("Navigate to check answers page", async () =>
        {
            await _uploadErrorsPage.ClickContinueAsync();

            await _reportingHelper.AttachScreenshotAsync(
                _playwrightDriver.Page,
                "Check Answers Page");
        });
    }

    private async Task VerifyErrorDetailsInternalAsync(
        UploadErrorSummary expectedError,
        IReadOnlyCollection<AffectedEmployee> affectedEmployees,
        ErrorDetailsHeaderType headerType)
    {
        (ErrorConfig config, Func<Task> verifyHeader) = GetHeaderConfig(headerType);

        await OpenErrorDetailsAsync(expectedError);

        await _reportingHelper.StepAsync(config.StepName, async () =>
        {
            await VerifyUploadErrorDetailsPageIsDisplayedAsync();
            await verifyHeader();
            await VerifyErrorMessageContentAsync(expectedError);
            await VerifyAffectedEmployeesTableAsync(affectedEmployees);

            await _reportingHelper.AttachScreenshotAsync(
                _playwrightDriver.Page,
                config.ScreenshotName);
        });
    }

    private (ErrorConfig Config, Func<Task> VerifyAction) GetHeaderConfig(
        ErrorDetailsHeaderType headerType) =>
        headerType switch
        {
            ErrorDetailsHeaderType.EmploymentDates => (
                new ErrorConfig(
                    "Employee employment dates error details page",
                    "Employment Dates Error Details Page"),
                _uploadErrorDetailsPage.VerifyEmploymentDatesHeaderIsDisplayedAsync),

            ErrorDetailsHeaderType.ArrearsOfPayDates => (
                new ErrorConfig(
                    "Arrears of pay dates error details page",
                    "Arrears Of Pay Dates Error Details Page"),
                _uploadErrorDetailsPage.VerifyArrearsOfPayDatesHeaderIsDisplayedAsync),

            ErrorDetailsHeaderType.NationalInsuranceNumber => (
                new ErrorConfig(
                    "Employee national insurance number error details page",
                    "Employee National Insurance Number Error Details Page"),
                _uploadErrorDetailsPage.VerifyEmployeeNationalInsuranceNumberHeaderIsDisplayedAsync),

            ErrorDetailsHeaderType.MoneyOwedToEmployer => (
                new ErrorConfig(
                    "Money owed to employer error details page",
                    "Money Owed To Employer Error Details Page"),
                _uploadErrorDetailsPage.VerifyMoneyOwedToEmployerHeaderIsDisplayedAsync),

            ErrorDetailsHeaderType.BasicPayPerWeek => (
                new ErrorConfig(
                    "Basic pay per week error details page",
                    "Basic Pay Per Week Error Details Page"),
                _uploadErrorDetailsPage.VerifyBasicPayPerWeekHeaderIsDisplayedAsync),

            ErrorDetailsHeaderType.EmployeeSurname => (
                new ErrorConfig(
                    "Employee surname error details page",
                    "Employee Surname Error Details Page"),
                _uploadErrorDetailsPage.VerifyEmployeeErrorSummaryIsDisplayedAsync),

            ErrorDetailsHeaderType.ArrearsOfPayOwed => (
                new ErrorConfig(
                    "Employee arrears of payment owed error details page",
                    "Employee Arrears Of Payment Owed Error Details Page"),
                _uploadErrorDetailsPage.VerifyEmployeeArrearsOfPaymentOwedErrorSummaryIsDisplayedAsync),

            ErrorDetailsHeaderType.EmployerName => (
                new ErrorConfig(
                    "Employer name error details page",
                    "Employer Name Error Details Page"),
                _uploadErrorDetailsPage.VerifyEmployerErrorSummaryIsDisplayedAsync),

            ErrorDetailsHeaderType.CaseReference => (
                new ErrorConfig(
                    "Case reference error details page",
                    "Case Reference Error Details Page"),
                _uploadErrorDetailsPage.VerifyCaseReferenceHeaderIsDisplayedAsync),

            ErrorDetailsHeaderType.HolidayContractedEntitlementDays => (
                new ErrorConfig(
                    "Contracted holiday entitlement error details page",
                    "Contracted Holiday Entitlement Error Details Page"),
                _uploadErrorDetailsPage.VerifyContractedHolidayEntitlementHeaderIsDisplayedAsync),

            ErrorDetailsHeaderType.HolidayDaysCarriedForward => (
                new ErrorConfig(
                    "Holiday days carried forward error details page",
                    "Holiday Days Carried Forward Error Details Page"),
                _uploadErrorDetailsPage.VerifyHolidayDaysCarriedForwardHeaderIsDisplayedAsync),

            ErrorDetailsHeaderType.HolidayDaysTaken => (
                new ErrorConfig(
                    "Holiday days taken error details page",
                    "Holiday Days Taken Error Details Page"),
                _uploadErrorDetailsPage.VerifyHolidayDaysTakenHeaderIsDisplayedAsync),

            ErrorDetailsHeaderType.NoDaysHolidayOwed => (
                new ErrorConfig(
                    "Holiday owed error details page",
                    "Holiday Owed Error Details Page"),
                _uploadErrorDetailsPage.VerifyNoDaysHolidayOwedHeaderIsDisplayedAsync),

            _ => throw new InvalidOperationException($"Unsupported header type: {headerType}")
        };

    private async Task VerifyErrorMessageContentAsync(UploadErrorSummary error)
    {
        await _uploadErrorDetailsPage.VerifyErrorMessageAsync(error.ErrorMessage);

        if (!string.IsNullOrWhiteSpace(error.HintText))
        {
            await _uploadErrorDetailsPage.VerifyErrorMessageAsync(error.HintText);
        }
    }

    private async Task VerifyAffectedEmployeesTableAsync(
        IReadOnlyCollection<AffectedEmployee> affectedEmployees)
    {
        await _uploadErrorDetailsPage.VerifyAffectedEmployeeTableHeadersAsync();

        foreach (AffectedEmployee employee in affectedEmployees)
        {
            await _uploadErrorDetailsPage.VerifyAffectedEmployeeAsync(employee);
        }
    }

    private string GetUploadedFileName()
    {
        if (!_scenarioContext.TryGetValue(ScenarioConstant.UploadedFileName, out string? fileName))
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
}
