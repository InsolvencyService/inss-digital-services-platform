using Allure.Net.Commons;
using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Pages.Common;
using GovUk.Forms.HostApp.UI.Test.Pages.Upload;
using GovUk.Forms.HostApp.UI.Test.Support;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators;

public sealed class CaseReferenceCoordinator(
    ICaseReferenceNumberPage caseReferenceNumberPage,
    IEmployerDetailsPage employerDetailsPage,
    ICommonPage commonPage,
    IPlaywrightDriver playwrightDriver,
    ScenarioContext scenarioContext,
    TestArtifacts testArtifacts)
    : BaseCoordinator(testArtifacts)
{
    public async Task VerifyCaseReferencePageIsDisplayedAsync()
    {
        await ExecuteStepAsync(
            "Verify Case Reference Number page is displayed",
            async () =>
            {
                await caseReferenceNumberPage.WaitForPageToLoadAsync();

                await AttachCurrentPageScreenshotAsync(
                    "Case Reference Number page displayed");
            });
    }

    public async Task EnterCaseReferenceAndContinueAsync(string caseReference)
    {
        await ExecuteStepAsync(
            "Enter case reference number and continue",
            async () =>
            {
                await caseReferenceNumberPage.EnterCaseReferenceNumberAsync(caseReference);
                await caseReferenceNumberPage.ClickContinueAsync();

                scenarioContext.Set(caseReference, ScenarioConstant.CaseReference);

                await AttachCurrentPageScreenshotAsync(
                    $"Case reference '{caseReference}' entered and continued");
            });
    }

    public async Task VerifyEmployerDetailsPageIsDisplayedAsync()
    {
        await ExecuteStepAsync(
            "Verify Employer Details page is displayed",
            async () =>
            {
                await employerDetailsPage.WaitForPageToLoadAsync();

                await AttachCurrentPageScreenshotAsync(
                    "Employer Details page displayed");
            });
    }

    public async Task<string> GetEmployerNameAsync()
    {
        return await ExecuteStepAsync(
            "Get employer name from Employer Details page",
            async () =>
            {
                string employerName = await employerDetailsPage.GetEmployerNameAsync();

                AddAllureLog($"Employer name: {employerName}");

                return employerName;
            });
    }

    public async Task<string> GetCaseReferenceNumberAsync()
    {
        return await ExecuteStepAsync(
            "Get case reference number from Employer Details page",
            async () =>
            {
                string caseReference = await employerDetailsPage.GetCaseReferenceNumberAsync();

                AddAllureLog($"Case reference: {caseReference}");

                return caseReference;
            });
    }

    public async Task ConfirmCorrectEmployerAsync()
    {
        await ExecuteStepAsync(
            "Confirm correct employer (Yes) and continue",
            async () =>
            {
                await employerDetailsPage.SelectYesAsync();
                await employerDetailsPage.ClickContinueAsync();

                await AttachCurrentPageScreenshotAsync(
                    "Correct employer confirmed");
            });
    }

    public async Task DeclineCorrectEmployerAsync()
    {
        await ExecuteStepAsync(
            "Decline correct employer (No) and continue",
            async () =>
            {
                await employerDetailsPage.SelectNoAsync();
                await employerDetailsPage.ClickContinueAsync();

                await AttachCurrentPageScreenshotAsync(
                    "Correct employer declined");
            });
    }

    public async Task EnterCaseReferenceNumberAsync(string caseReference)
    {
        await ExecuteStepAsync(
            "Enter case reference number",
            async () =>
            {
                await caseReferenceNumberPage.EnterCaseReferenceNumberAsync(caseReference);
                scenarioContext.Set(caseReference, ScenarioConstant.CaseReference);
                AddAllureLog($"Case reference entered: {caseReference}");
            });
    }

    public async Task ClickContinueAsync()
    {
        await ExecuteStepAsync(
            "Click Continue",
            async () =>
            {
                Task<IResponse> responseTask = playwrightDriver.Page.WaitForResponseAsync(response =>
                    response.Url.Contains("/case-reference-match") &&
                    response.Status == 200);

                await Task.WhenAll(
                    responseTask,
                    playwrightDriver.Page
                        .GetByRole(AriaRole.Button, new() { Name = SharedLocactors.ContinueButton })
                        .ClickAsync());

                await AttachCurrentPageScreenshotAsync("Continue clicked");
            });
    }

    public async Task SelectYesAsync()
    {
        await ExecuteStepAsync(
            "Select Yes on Employer Details page",
            async () =>
            {
                await employerDetailsPage.SelectYesAsync();
                AddAllureLog("Yes selected");
            });
    }

    public async Task SelectNoAsync()
    {
        await ExecuteStepAsync(
            "Select No on Employer Details page",
            async () =>
            {
                await employerDetailsPage.SelectNoAsync();
                AddAllureLog("No selected");
            });
    }

    public async Task VerifyValidationErrorAsync(string errorMessage)
    {
        await ExecuteStepAsync(
            $"Verify validation error: {errorMessage}",
            async () =>
            {
                await caseReferenceNumberPage.VerifyErrorMessageAsync(errorMessage);
                AddAllureLog($"Validation error verified: {errorMessage}");
            });
    }

    public async Task VerifyCaseReferenceInSummaryAsync(string expectedReference)
    {
        await ExecuteStepAsync(
            "Verify case reference number shown in Employer Details summary",
            async () =>
            {
                string displayed = await employerDetailsPage.GetCaseReferenceNumberAsync();

                Assert.That(displayed, Is.EqualTo(expectedReference),
                    $"Expected case reference '{expectedReference}' but Employer Details page shows '{displayed}'.");

                AddAllureLog($"Case reference '{displayed}' confirmed in summary.");
            });
    }

    public async Task VerifyEmployerNameIsDisplayedAsync()
    {
        await ExecuteStepAsync(
            "Verify employer name is shown in Employer Details summary",
            async () =>
            {
                string employerName = await employerDetailsPage.GetEmployerNameAsync();

                Assert.That(employerName, Is.Not.Empty,
                    "Employer name was empty on the Employer Details page.");

                AddAllureLog($"Employer name '{employerName}' confirmed in summary.");
            });
    }

    public async Task VerifyCaseReferenceAriaSnapshotAsync()
    {
        await ExecuteStepAsync(
            "Verify Case Reference Number page ARIA snapshot",
            caseReferenceNumberPage.VerifyAriaSnapshotAsync);
    }

    public async Task VerifyEmployerDetailsAriaSnapshotAsync()
    {
        await ExecuteStepAsync(
            "Verify Employer Details page ARIA snapshot",
            employerDetailsPage.VerifyAriaSnapshotAsync);
    }

    private async Task<string> CaptureCurrentPageVisualAsync(string screenshotName)
    {
        return await CapturePageVisualAsync(
            () => commonPage.CaptureVisualAsync(playwrightDriver.Page),
            screenshotName);
    }

    private async Task AttachCurrentPageScreenshotAsync(string attachmentName)
    {
        string screenshotPath = await CaptureCurrentPageVisualAsync(
            SanitizeFileName(attachmentName));

        AllureApi.AddAttachment(
            attachmentName,
            "image/png",
            screenshotPath);

        AddAllureLog($"Screenshot attached: {attachmentName}");
    }

    private static void AddAllureLog(string message)
    {
        AllureApi.AddAttachment(
            $"Log_{DateTime.UtcNow:HHmmssfff}",
            "text/plain",
            System.Text.Encoding.UTF8.GetBytes(message),
            "txt");
    }

    private static string SanitizeFileName(string value)
    {
        foreach (char invalidChar in Path.GetInvalidFileNameChars())
        {
            value = value.Replace(invalidChar, '_');
        }

        return value.Replace(' ', '_');
    }
}
