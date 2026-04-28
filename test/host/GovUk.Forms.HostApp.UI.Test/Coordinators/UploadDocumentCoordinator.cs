using GovUk.Forms.HostApp.UI.Test.Builders;
using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Extensions;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Pages.Upload;
using GovUk.Forms.HostApp.UI.Test.Support;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators;

public class UploadDocumentCoordinator(
    IUploadDocumentPage uploadDocumentPage,
    ScenarioContext scenarioContext,
    IReqnrollOutputHelper outputHelper,
    IAllureReportingHelper allure,
    IPlaywrightDriver playwrightDriver) : BaseCoordinator
{

    public async Task VerifyUploadDocumentPageIsDisplayedAsync()
    {
        await allure.StepAsync("File Upload Page", async () =>
        {
            await uploadDocumentPage.WaitForPageToLoadAsync();
            await allure.AttachScreenshotAsync(
                    playwrightDriver.Page,
                    "Upload File Page");
        });
    }

    public async Task ClickOnContinueButtonAsync()
    {
        await uploadDocumentPage.ClickOnContinueButtonAsync();
    }

    public async Task ClickOnBackButtonAsync()
    {
        await uploadDocumentPage.ClickOnBackButtonAsync();
    }

    public async Task UploadFileAsync(string file)
    {
        await allure.StepAsync("File Upload", async () =>
        {
            await uploadDocumentPage.UploadFileAsync(file);
            await allure.AttachScreenshotAsync(
                    playwrightDriver.Page,
                    "After Upload File");
        });
    }

    public async Task NavigateToFeedbackPageAsync()
    {
        IPage feedbackPage = await NavigateAsync(uploadDocumentPage.ClickOnGiveFeedbackLinkAsync);
        scenarioContext.Set(feedbackPage);
    }

    public async Task UploadValidRp14aAsync()
    {
        string xml = new Rp14aBuilder()
            .Build();

        string filePath = await Rp14aFileFactory.CreateAsync(xml);
        string fileName = Path.GetFileName(filePath);

        await uploadDocumentPage.UploadFileAsync(filePath);
        scenarioContext.Set(fileName, ScenarioConstant.UploadedFileName);
        scenarioContext.Set(filePath, ScenarioConstant.UploadedFilePath);

        outputHelper.WriteLine($"Uploading file: {fileName}");
        outputHelper.WriteLine($"Full path: {filePath}");
        outputHelper.AddAttachmentAsLink(filePath);
        allure.AttachFile(filePath, "Uploaded RP14a File");

    }

    public async Task VerifyThatFileIsUploadedAsync()
    {
        string expectedFileName = scenarioContext.Get<string>(ScenarioConstant.UploadedFileName);

        string actualUploadedFileText =
            await uploadDocumentPage.GetUploadedFileNameAsync();

        Assert.That(actualUploadedFileText, Is.EqualTo(expectedFileName),
            $"Expected file '{expectedFileName}' not found. Actual: {actualUploadedFileText}");
    }

    public async Task VerifyOnlyOneFileUploadedAsync()
    {
        string expectedFileName = scenarioContext.Get<string>(ScenarioConstant.UploadedFileName);

        IReadOnlyList<string> uploadedFiles = await uploadDocumentPage.GetUploadedFileNamesAsync();

        int count = uploadedFiles.Count(f => f.Contains(expectedFileName));

        Assert.That(count, Is.EqualTo(1),
            $"Expected only 1 instance of '{expectedFileName}', but found {count}. " +
            $"Actual files: {string.Join(", ", uploadedFiles)}");
    }

    public async Task<string> CaptureUploadDocumentPageVisualAsync()
    {
        return await CapturePageVisualAsync(uploadDocumentPage.CapturePageVisualAsync, ScenarioConstant.UploadPage);
    }

    public async Task ExpandCommonIssuesWhenUploadingRP14AFormsAsync()
    {
        await uploadDocumentPage.ExpandCommonIssuesWhenUploadingRP14AFormsAsync();
    }

}
