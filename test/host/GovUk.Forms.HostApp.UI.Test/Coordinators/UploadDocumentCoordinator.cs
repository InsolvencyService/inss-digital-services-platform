using GovUk.Forms.HostApp.UI.Test.Builders;
using GovUk.Forms.HostApp.UI.Test.Extensions;
using GovUk.Forms.HostApp.UI.Test.Pages.Upload;
using GovUk.Forms.HostApp.UI.Test.Support;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators;

public class UploadDocumentCoordinator(
    IUploadDocumentPage uploadDocumentPage,
    ScenarioContext scenarioContext,
    IReqnrollOutputHelper outputHelper) : BaseCoordinator
{

    public async Task VerifyUploadDocumentPageIsDisplayedAsync()
    {
        await uploadDocumentPage.WaitForPageToLoadAsync();
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
        await uploadDocumentPage.UploadFileAsync(file);
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
