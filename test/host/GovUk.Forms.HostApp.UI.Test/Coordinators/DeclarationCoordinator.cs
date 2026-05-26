using Allure.Net.Commons;
using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Pages.Common;
using GovUk.Forms.HostApp.UI.Test.Pages.Declaration;
using GovUk.Forms.HostApp.UI.Test.Support;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators;

public sealed class DeclarationCoordinator(
    IDeclarationPage declarationPage,
    ISection187Page section187Page,
    IPlaywrightDriver playwrightDriver,
    ICommonPage commonPage,
    TestArtifacts testArtifacts)
    : BaseCoordinator(testArtifacts)
{
    public async Task VerifyDeclarationPageIsDisplayedAsync()
    {
        await ExecuteStepAsync(
            "Verify Declaration page is displayed",
            async () =>
            {
                await declarationPage.WaitForPageToLoadAsync();

                await AttachCurrentPageScreenshotAsync(
                    "Declaration page displayed");
            });
    }

    public async Task OpenSection187Async()
    {
        await ExecuteStepAsync(
            "Open Section 187 page",
            async () =>
            {
                IPage page = await NavigateAsync(
                    declarationPage.ClickOnSection187LinkAsync);

                section187Page.AttachTo(page);

                await AttachCurrentPageScreenshotAsync(
                    "Section 187 page opened");
            });
    }

    public async Task VerifySection187PageContentAsync()
    {
        await ExecuteStepAsync(
            "Verify Section 187 page content",
            async () =>
            {
                await section187Page.VerifyThatSection187PageIsDisplayedAsync();

                await AttachCurrentPageScreenshotAsync(
                    "Section 187 content verified");
            });
    }

    public async Task ReturnToStartPageAsync()
    {
        await ExecuteStepAsync(
            "Return to Start page",
            async () =>
            {
                await declarationPage.ClickOnBackButtonAsync();

                await AttachCurrentPageScreenshotAsync(
                    "Returned to Start page");
            });
    }

    public async Task ClickAgreeAndContinueButtonAsync()
    {
        await ExecuteStepAsync(
            "Click Agree and Continue",
            async () =>
            {
                await declarationPage.ClickOnAgreeAndContinueButtonAsync();

                await AttachCurrentPageScreenshotAsync(
                    "Agree and Continue clicked");
            });
    }

    public async Task<string> CaptureDeclarationVisualAsync()
    {
        return await ExecuteStepAsync(
            "Capture Declaration page visual snapshot",
            async () =>
            {
                string screenshotPath = await CaptureCurrentPageVisualAsync(
                    ScenarioConstant.DeclarationPage);

                AllureApi.AddAttachment(
                    "Declaration page visual snapshot",
                    "image/png",
                    screenshotPath);

                return screenshotPath;
            });
    }

    public async Task NavigateToUploadAFilePageAsync()
    {
        await ExecuteStepAsync(
            "Navigate to Upload File page",
            async () =>
            {
                await declarationPage.WaitForPageToLoadAsync();

                await declarationPage.ClickOnAgreeAndContinueButtonAsync();

                await AttachCurrentPageScreenshotAsync(
                    "Navigated to Upload File page");
            });
    }

    private async Task<string> CaptureCurrentPageVisualAsync(
        string screenshotName)
    {
        return await CapturePageVisualAsync(
            () => commonPage.CaptureVisualAsync(playwrightDriver.Page),
            screenshotName);
    }

    private async Task AttachCurrentPageScreenshotAsync(
        string attachmentName)
    {
        string screenshotPath = await CaptureCurrentPageVisualAsync(
            SanitizeFileName(attachmentName));

        AllureApi.AddAttachment(
            attachmentName,
            "image/png",
            screenshotPath);

        AddAllureLog(
            $"Screenshot attached: {attachmentName}");
    }
    private static void AddAllureLog(string message)
    {
        string tempFile = Path.Join(
            Path.GetTempPath(),
            $"{Guid.NewGuid()}.txt");

        File.WriteAllText(tempFile, message);

        AllureApi.AddAttachment(
            $"Log_{DateTime.UtcNow:HHmmssfff}",
            "text/plain",
            tempFile);
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
