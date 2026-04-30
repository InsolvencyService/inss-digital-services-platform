using GovUk.Forms.HostApp.UI.Test.Builders;
using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Extensions;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Pages.Common;
using GovUk.Forms.HostApp.UI.Test.Pages.Upload;
using GovUk.Forms.HostApp.UI.Test.Support;
using static GovUk.Forms.HostApp.UI.Test.Models.TestData;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators;

public class UploadDocumentCoordinator(
    IUploadDocumentPage uploadDocumentPage,
    ScenarioContext scenarioContext,
    IReqnrollOutputHelper outputHelper,
    IAllureReportingHelper allure,
    IPlaywrightDriver playwrightDriver,
    ICommonPage commonPage,
    TestArtifacts testArtifacts) : BaseCoordinator(testArtifacts)
{
    public async Task VerifyUploadDocumentPageIsDisplayedAsync()
    {
        await allure.StepAsync("File Upload Page is displayed", async () =>
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

    public async Task UploadFileAsync(string filePath)
    {
        await allure.StepAsync($"Upload file '{Path.GetFileName(filePath)}'", async () =>
        {
            await uploadDocumentPage.UploadFileAsync(filePath);

            StoreUploadedFileInScenarioContext(filePath);
            AttachUploadedFile(filePath);

            await allure.AttachScreenshotAsync(
                playwrightDriver.Page,
                "After Upload File");
        });
    }

    public async Task UploadValidRp14aAsync()
    {
        string xml = new Rp14aBuilder()
            .Build();

        await UploadRp14aAsync(xml, "Upload valid RP14A file");
    }

    public async Task UploadRp14aWithCaseReferenceAsync(string caseReference)
    {
        string xml = new Rp14aBuilder()
            .WithCaseReference(caseReference)
            .Build();

        await UploadRp14aAsync(
            xml,
            $"Upload RP14A file with case reference '{caseReference}'");
    }

    public async Task UploadRp14aWithEmployerNameAsync(string employerName)
    {
        string xml = new Rp14aBuilder()
            .WithEmployerName(employerName)
            .Build();

        await UploadRp14aAsync(
            xml,
            $"Upload RP14A file with employer name length {employerName.Length}");
    }

    public async Task UploadRp14aWithEmployerNameLengthAsync(int length)
    {
        string xml = new Rp14aBuilder()
            .WithEmployerNameLength(length)
            .Build();

        await UploadRp14aAsync(
            xml,
            $"Upload RP14A file with employer name length {length}");
    }

    public async Task UploadAndSubmitRp14aWithEmployerNameLengthAsync(int length)
    {
        await UploadRp14aWithEmployerNameLengthAsync(length);
        await NavigateToSubmitPageAsync();
    }

    private async Task UploadRp14aAsync(string xml, string stepName)
    {
        string filePath = await Rp14aFileFactory.CreateAsync(xml);

        await allure.StepAsync(stepName, async () =>
        {
            await uploadDocumentPage.UploadFileAsync(filePath);

            StoreUploadedFileInScenarioContext(filePath);
            AttachUploadedFile(filePath);

            await allure.AttachScreenshotAsync(
                playwrightDriver.Page,
                "After Upload File");
        });
    }

    private void StoreUploadedFileInScenarioContext(string filePath)
    {
        string fileName = Path.GetFileName(filePath);

        scenarioContext.Set(fileName, ScenarioConstant.UploadedFileName);
        scenarioContext.Set(filePath, ScenarioConstant.UploadedFilePath);
    }

    private void AttachUploadedFile(string filePath)
    {
        string fileName = Path.GetFileName(filePath);

        outputHelper.WriteLine($"Uploading file: {fileName}");
        outputHelper.WriteLine($"Full path: {filePath}");

        outputHelper.AddAttachmentAsLink(filePath);

        allure.AttachFile(filePath, $"Uploaded RP14A File - {fileName}");
    }

    public async Task VerifyThatFileIsUploadedAsync()
    {
        string expectedFileName =
            scenarioContext.Get<string>(ScenarioConstant.UploadedFileName);

        string actualUploadedFileText =
            await uploadDocumentPage.GetUploadedFileNameAsync();

        Assert.That(
            actualUploadedFileText,
            Is.EqualTo(expectedFileName),
            $"Expected file '{expectedFileName}' not found. Actual: {actualUploadedFileText}");
    }

    public async Task VerifyOnlyOneFileUploadedAsync()
    {
        string expectedFileName =
            scenarioContext.Get<string>(ScenarioConstant.UploadedFileName);

        IReadOnlyList<string> uploadedFiles =
            await uploadDocumentPage.GetUploadedFileNamesAsync();

        int count = uploadedFiles.Count(file =>
            file.Equals(expectedFileName, StringComparison.OrdinalIgnoreCase));

        Assert.That(
            count,
            Is.EqualTo(1),
            $"Expected only 1 instance of '{expectedFileName}', but found {count}. " +
            $"Actual files: {string.Join(", ", uploadedFiles)}");
    }

    public async Task<string> CaptureUploadDocumentPageVisualAsync()
    {
        return await CapturePageVisualAsync(
            () => commonPage.CaptureVisualAsync(playwrightDriver.Page),
            ScenarioConstant.UploadPage);
    }

    public async Task ExpandCommonIssuesWhenUploadingRP14AFormsAsync()
    {
        await uploadDocumentPage.ExpandCommonIssuesWhenUploadingRP14AFormsAsync();
    }

    public async Task NavigateToFeedbackPageAsync()
    {
        IPage feedbackPage =
            await NavigateAsync(uploadDocumentPage.ClickOnGiveFeedbackLinkAsync);

        scenarioContext.Set(feedbackPage);
    }

    public async Task NavigateToSubmitPageAsync()
    {
        await allure.StepAsync("Navigate to submit page", async () =>
        {
            await uploadDocumentPage.ClickOnContinueButtonAsync();

            await allure.AttachScreenshotAsync(
                playwrightDriver.Page,
                "Submit Page");
        });
    }

    public async Task VerifyInvalidFileExtensionErrorAsync(UploadFileError uploadFileError)
    {
        await uploadDocumentPage.VerifyUploadFileErrorAsync(uploadFileError);
    }
}
