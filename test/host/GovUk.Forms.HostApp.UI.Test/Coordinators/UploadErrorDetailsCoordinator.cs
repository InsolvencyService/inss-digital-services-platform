using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Pages.Upload;
using GovUk.Forms.HostApp.UI.Test.Support;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators;

public class UploadErrorDetailsCoordinator(
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

    public async Task VerifyEmployeeErrorMessageSummaryAsync(AffectedEmployee affectedEmployee)
    {
        await allure.StepAsync("Upload Errors Page", async () =>
        {
            await VerifyThatUploadErrorDetailsPageIsDisplayedAsync();
            await uploadErrorDetailsPage.VerifyAffectedEmployeeTableHeadersAsync();
            await uploadErrorDetailsPage.VerifyAffectedEmployeeAsync(affectedEmployee);
            await allure.AttachScreenshotAsync(
                    playwrightDriver.Page,
                    "Upload Errors Page");
        });
    }

    public async Task VerifyUploadErrorsPageIsDisplyedAsync()
    {
        await allure.StepAsync("Upload Errors Page", async () =>
        {
            await uploadErrorsPage.WaitForPageToLoadAsync();
            await allure.AttachScreenshotAsync(
                    playwrightDriver.Page,
                    "Upload Errors Page");
        });
    }

    public async Task VerifyCaseReferenceErrorsIsDisplayedAsync(string error)
    {
        await uploadErrorsPage.VerifyThatCaseReferenceErrorsAreDisplayedAsync(error);
    }


    public async Task VerifyErrorSummaryIsDisplayedAsync(UploadErrorSummary expectedError)
    {
        string expectedFileName = scenarioContext.Get<string>(ScenarioConstant.UploadedFileName);
        await allure.StepAsync("Upload Errors Page", async () =>
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
        await allure.StepAsync("Upload Errors Summary Page", async () =>
        {
            await uploadErrorsPage.WaitForPageToLoadAsync();
            await uploadErrorsPage.ClickOnViewDetailsAsync(expectedError);
            await allure.AttachScreenshotAsync(
                    playwrightDriver.Page,
                    "Upload Errors Summary Page");
        });
    }

    public async Task VerifyErrorDetailsDoesNotContainAsync(string text)
    {
        await uploadErrorDetailsPage.VerifyErrorDetailsDoesNotContainAsync(text);
    }

    public async Task VerifyEmployerErrorSummaryIsDisplayedAsync()
    {
        await uploadErrorDetailsPage.EmployerErrorSummaryIsDisplayedAsync();
    }
}

