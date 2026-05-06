using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Pages.Upload;
using GovUk.Forms.HostApp.UI.Test.Support;
using System.Globalization;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators;

public sealed class UploadErrorDetailsCoordinator(
    IUploadErrorDetailsPage uploadErrorDetailsPage,
    IUploadErrorsPage uploadErrorsPage,
    IPlaywrightDriver playwrightDriver,
    ScenarioContext scenarioContext,
    IAllureReportingHelper allure)
{
    public async Task VerifyUploadErrorPageIsDisplayedAsync()
    {
        await uploadErrorsPage.WaitForPageToLoadAsync();
    }

    public async Task VerifyThatUploadErrorDetailsPageIsDisplayedAsync()
    {
        await uploadErrorDetailsPage.WaitForPageToLoadAsync();
    }

    public async Task VerifyErrorSummaryIsDisplayedAsync(UploadErrorSummary expectedError)
    {
        string expectedFileName =
            scenarioContext.Get<string>(ScenarioConstant.UploadedFileName);

        await allure.StepAsync("Verify upload errors summary page", async () =>
        {
            await uploadErrorsPage.WaitForPageToLoadAsync();
            await uploadErrorsPage.VerifyUploadedFileNameAsync(expectedFileName);
            await uploadErrorsPage.VerifyErrorSummaryAsync(expectedError);

            await allure.AttachScreenshotAsync(
                playwrightDriver.Page,
                "Upload Errors Page");
        });
    }

    public async Task OpenErrorDetailsAsync(UploadErrorSummary expectedError)
    {
        await allure.StepAsync("Open upload error details page", async () =>
        {
            await uploadErrorsPage.WaitForPageToLoadAsync();
            await uploadErrorsPage.ClickOnViewDetailsAsync(expectedError);

            await allure.AttachScreenshotAsync(
                playwrightDriver.Page,
                "Upload Errors Summary Page");
        });
    }

    public async Task VerifyValidationErrorAsync(string expectedError)
    {
        await allure.StepAsync("Verify validation error", async () =>
        {
            await uploadErrorsPage.WaitForPageToLoadAsync();

            string resolvedError =
                await ResolveValidationErrorMessageAsync(expectedError);

            await uploadErrorsPage.VerifyErrorMessageAsync(resolvedError);

            await allure.AttachScreenshotAsync(
                playwrightDriver.Page,
                "Validation Error");
        });
    }

    public async Task<string> ResolveValidationErrorMessageAsync(string expectedError)
    {
        if (!expectedError.Contains("[COUNT]", StringComparison.Ordinal))
        {
            return expectedError;
        }

        string errorKey = ExtractErrorKey(expectedError);
        int count = await uploadErrorsPage.GetErrorCountAsync(errorKey);

        return expectedError.Replace(
            "[COUNT]",
            count.ToString(CultureInfo.InvariantCulture),
            StringComparison.Ordinal);
    }

    public async Task VerifyEmployeeNationalInsuranceNumberErrorDetailsAsync(
        UploadErrorSummary expectedError,
        AffectedEmployee affectedEmployee)
    {
        await VerifyEmployeeErrorDetailsAsync(
            expectedError,
            affectedEmployee,
            ErrorDetailsHeaderType.NationalInsuranceNumber,
            "Employee national insurance number error details page",
            "Employee National Insurance Number Error Details Page");
    }

    public async Task VerifyMoneyOwedToEmployerErrorDetailsAsync(
        UploadErrorSummary expectedError,
        AffectedEmployee affectedEmployee)
    {
        await VerifyEmployeeErrorDetailsAsync(
            expectedError,
            affectedEmployee,
            ErrorDetailsHeaderType.MoneyOwedToEmployer,
            "Money owed to employer error details page",
            "Money Owed To Employer Error Details Page");
    }

    public async Task VerifyEmploymentDatesErrorDetailsAsync(
        UploadErrorSummary expectedError,
        AffectedEmployee affectedEmployee)
    {
        await VerifyEmployeeErrorDetailsAsync(
            expectedError,
            affectedEmployee,
            ErrorDetailsHeaderType.EmploymentDates,
            "Employee employment dates error details page",
            "Employment Dates Error Details Page");
    }

    public async Task VerifyArrearsOfPayDatesErrorDetailsAsync(
        UploadErrorSummary expectedError,
        AffectedEmployee affectedEmployee)
    {
        await VerifyEmployeeErrorDetailsAsync(
            expectedError,
            affectedEmployee,
            ErrorDetailsHeaderType.ArrearsOfPayDates,
            "Arrears of pay dates error details page",
            "Arrears Of Pay Dates Error Details Page");
    }

    public async Task VerifyEmployeeSurnameErrorDetailsAsync(
    UploadErrorSummary expectedError,
    AffectedEmployee affectedEmployee)
    {
        await VerifyEmployeeErrorDetailsAsync(
            expectedError,
            affectedEmployee,
            ErrorDetailsHeaderType.EmployeeSurname,
            "Employee surname error details page",
            "Employee Surname Error Details Page");
    }

    private async Task VerifyEmployeeErrorDetailsAsync(
        UploadErrorSummary expectedError,
        AffectedEmployee affectedEmployee,
        ErrorDetailsHeaderType headerType,
        string stepName,
        string screenshotName)
    {
        await OpenErrorDetailsAsync(expectedError);

        await allure.StepAsync(stepName, async () =>
        {
            await VerifyThatUploadErrorDetailsPageIsDisplayedAsync();

            await VerifyHeaderAsync(headerType);

            await uploadErrorDetailsPage.VerifyErrorMessageAsync(expectedError.ErrorMessage);

            if (!string.IsNullOrWhiteSpace(expectedError.HintText))
            {
                await uploadErrorDetailsPage.VerifyErrorMessageAsync(expectedError.HintText);
            }

            await uploadErrorDetailsPage.VerifyAffectedEmployeeTableHeadersAsync();
            await uploadErrorDetailsPage.VerifyAffectedEmployeeAsync(affectedEmployee);

            await allure.AttachScreenshotAsync(
                playwrightDriver.Page,
                screenshotName);
        });
    }

    private async Task VerifyHeaderAsync(ErrorDetailsHeaderType headerType)
    {
        switch (headerType)
        {
            case ErrorDetailsHeaderType.EmploymentDates:
                await uploadErrorDetailsPage.VerifyEmploymentDatesHeaderIsDisplayedAsync();
                break;

            case ErrorDetailsHeaderType.ArrearsOfPayDates:
                await uploadErrorDetailsPage.VerifyArrearsOfPayDatesHeaderIsDisplayedAsync();
                break;

            case ErrorDetailsHeaderType.NationalInsuranceNumber:
                await uploadErrorDetailsPage.VerifyEmployeeNationalInsuranceNumberHeaderIsDisplayedAsync();
                break;

            case ErrorDetailsHeaderType.MoneyOwedToEmployer:
                await uploadErrorDetailsPage.VerifyMoneyOwedToEmployerHeaderIsDisplayedAsync();
                break;
            case ErrorDetailsHeaderType.EmployeeSurname:
                await uploadErrorDetailsPage.VerifyEmployeeErrorSummaryIsDisplayedAsync();
                break;
            case ErrorDetailsHeaderType.ArrearsOfPayOwed:
                await uploadErrorDetailsPage.VerifyEmployeeArrearsOfPaymentOwedErrorSummaryIsDisplayedAsync();
                break;
            case ErrorDetailsHeaderType.EmployerName:
                await uploadErrorDetailsPage.VerifyEmployerErrorSummaryIsDisplayedAsync();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(headerType), headerType, null);
        }
    }

    private static string ExtractErrorKey(string expectedError)
    {
        return expectedError
            .Replace("[COUNT]", string.Empty, StringComparison.Ordinal)
            .Trim();
    }

    public async Task ClickOnBackButtonAndVerifyUploadErrorPageIsDisplayedAsync()
    {
        await allure.StepAsync("Click Back and verify Upload Error page is displayed", async () =>
        {
            await uploadErrorDetailsPage.ClickBackButtonAsync();
            await VerifyUploadErrorPageIsDisplayedAsync();

            await allure.AttachScreenshotAsync(
                playwrightDriver.Page,
                "Upload Error Page after clicking Back");
        });
    }

    public async Task NavigateToCheckAnswersPageAsync()
    {
        await uploadErrorsPage.ClickContinueAsync();
    }

    public async Task VerifyEmployeeArrearsOfPayOwedErrorDetailsAsync(
    UploadErrorSummary expectedError,
    AffectedEmployee affectedEmployee)
    {
        await VerifyEmployeeErrorDetailsAsync(
            expectedError,
            affectedEmployee,
            ErrorDetailsHeaderType.ArrearsOfPayOwed,
            "Employee arrears of payment owed",
            "Employee arrears of payment owed");
    }

    public async Task VerifyEmployerNameErrorDetailsAsync(
    UploadErrorSummary expectedError,
    AffectedEmployee affectedEmployee)
    {
        await VerifyEmployeeErrorDetailsAsync(
            expectedError,
            affectedEmployee,
            ErrorDetailsHeaderType.EmployerName,
            "Employer name error details page",
            "Employer Name Error Details Page");
    }

    public async Task VerifyEmployeeArrearsOfPayOwedErrorDetailsAsync(
    UploadErrorSummary expectedError,
    IReadOnlyCollection<AffectedEmployee> affectedEmployees)
    {
        await VerifyEmployeeErrorDetailsAsync(
            expectedError,
            affectedEmployees,
            ErrorDetailsHeaderType.ArrearsOfPayOwed,
            "Employee arrears of payment owed",
            "Employee arrears of payment owed");
    }

    private async Task VerifyEmployeeErrorDetailsAsync(
    UploadErrorSummary expectedError,
    IReadOnlyCollection<AffectedEmployee> affectedEmployees,
    ErrorDetailsHeaderType headerType,
    string stepName,
    string screenshotName)
    {
        await OpenErrorDetailsAsync(expectedError);

        await allure.StepAsync(stepName, async () =>
        {
            await VerifyThatUploadErrorDetailsPageIsDisplayedAsync();

            await VerifyHeaderAsync(headerType);

            await uploadErrorDetailsPage.VerifyErrorMessageAsync(expectedError.ErrorMessage);

            if (!string.IsNullOrWhiteSpace(expectedError.HintText))
            {
                await uploadErrorDetailsPage.VerifyErrorMessageAsync(expectedError.HintText);
            }

            await uploadErrorDetailsPage.VerifyAffectedEmployeeTableHeadersAsync();

            foreach (AffectedEmployee affectedEmployee in affectedEmployees)
            {
                await uploadErrorDetailsPage.VerifyAffectedEmployeeAsync(affectedEmployee);
            }

            await allure.AttachScreenshotAsync(
                playwrightDriver.Page,
                screenshotName);
        });
    }
}

public enum ErrorDetailsHeaderType
{
    EmployeeSurname,
    EmployerName,
    EmploymentDates,
    ArrearsOfPayDates,
    ArrearsOfPayOwed,
    NationalInsuranceNumber,
    MoneyOwedToEmployer
}

