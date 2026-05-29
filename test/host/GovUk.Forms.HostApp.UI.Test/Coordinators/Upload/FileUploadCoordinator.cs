using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Pages.Upload;
using GovUk.Forms.HostApp.UI.Test.Support;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;


public sealed class FileUploadCoordinator : IFileUploadCoordinator
{
    private readonly IUploadDocumentPage _uploadDocumentPage;
    private readonly IAllureReportingHelper _reportingHelper;
    private readonly IPlaywrightDriver _playwrightDriver;
    private readonly ScenarioContext _scenarioContext;
    private readonly IReqnrollOutputHelper _outputHelper;

    public FileUploadCoordinator(
        IUploadDocumentPage uploadDocumentPage,
        IAllureReportingHelper allure,
        IPlaywrightDriver playwrightDriver,
        ScenarioContext scenarioContext,
        IReqnrollOutputHelper outputHelper)
    {
        _uploadDocumentPage = uploadDocumentPage;
        _reportingHelper = allure;
        _playwrightDriver = playwrightDriver;
        _scenarioContext = scenarioContext;
        _outputHelper = outputHelper;
    }

    public async Task UploadFileAsync(string filePath)
    {
        ValidateFilePath(filePath);

        string stepName = $"Upload file '{Path.GetFileName(filePath)}'";

        await UploadFileInternalAsync(filePath, stepName);
    }

    public async Task VerifyUploadDocumentContentSnapShotAsync()
    {
        await _uploadDocumentPage.VerifyUploadContentAriaSnapshotAsync();
    }

    public async Task VerifyUploadCommonIssuesContentVisualSnapShotAsync()
    {
        await _uploadDocumentPage.VerifyCommonIssuesWhenUploadingAriaSnapshotAsync();
    }

    private async Task UploadFileInternalAsync(string filePath, string stepName)
    {
        ValidateFilePath(filePath);

        if (string.IsNullOrWhiteSpace(stepName))
        {
            stepName = $"Upload file '{Path.GetFileName(filePath)}'";
        }

        await _reportingHelper.StepAsync(stepName, async () =>
        {
            await _uploadDocumentPage.UploadFileAsync(filePath);

            StoreUploadedFileInScenarioContext(filePath);
            AttachUploadedFile(filePath);

            await _reportingHelper.AttachScreenshotAsync(
                _playwrightDriver.Page,
                "After Upload File");
        });
    }

    private void StoreUploadedFileInScenarioContext(string filePath)
    {
        string fileName = Path.GetFileName(filePath);

        _scenarioContext.Set(fileName, ScenarioConstant.UploadedFileName);
        _scenarioContext.Set(filePath, ScenarioConstant.UploadedFilePath);
    }

    private void AttachUploadedFile(string filePath)
    {
        string fileName = Path.GetFileName(filePath);

        _outputHelper.WriteLine($"Uploading file: {fileName}");
        _outputHelper.WriteLine($"Full path: {filePath}");

        _reportingHelper.AttachFile(
            filePath,
            $"Uploaded File - {fileName}");
    }

    private static void ValidateFilePath(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException(
                "File path cannot be null or empty.",
                nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(
                $"File does not exist at path: {filePath}",
                filePath);
        }
    }
}
