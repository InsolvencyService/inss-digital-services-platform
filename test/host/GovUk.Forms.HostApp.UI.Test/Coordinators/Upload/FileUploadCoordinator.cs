using GovUk.Forms.HostApp.UI.Test.Builders;
using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Extensions;
using GovUk.Forms.HostApp.UI.Test.Factories;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Pages.Upload;
using GovUk.Forms.HostApp.UI.Test.Support;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;


public sealed class FileUploadCoordinator : IFileUploadCoordinator
{
    private readonly IUploadDocumentPage _uploadDocumentPage;
    private readonly IAllureReportingHelper _allure;
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
        _allure = allure;
        _playwrightDriver = playwrightDriver;
        _scenarioContext = scenarioContext;
        _outputHelper = outputHelper;
    }

    public async Task UploadFileAsync(string filePath)
    {
        ValidateFilePath(filePath);

        await UploadFileInternalAsync(
            filePath,
            $"Upload file '{Path.GetFileName(filePath)}'");
    }

    public async Task UploadValidRp14aAsync()
    {
        Rp14aScenarioBuilder scenario = Rp14aScenarioBuilder.Create()
            .WithCaseReference(Rp14aCaseReferences.Default);

        await UploadRp14aAsync(scenario);
    }

    public async Task UploadRp14aAsync(Rp14aScenarioBuilder scenarioBuilder)
    {
        ArgumentNullException.ThrowIfNull(scenarioBuilder);

        string xml = scenarioBuilder.BuildXml();

        ValidateXml(xml);

        string description = scenarioBuilder.GetDescription();

        string stepName = string.IsNullOrWhiteSpace(description)
            ? "Upload RP14A"
            : $"Upload RP14A: {description}";

        await UploadRp14aInternalAsync(xml, stepName);
    }

    private async Task UploadRp14aInternalAsync(string xml, string stepName)
    {
        ValidateXml(xml);

        if (string.IsNullOrWhiteSpace(stepName))
        {
            stepName = "Upload RP14A";
        }

        string filePath = await Rp14aFileFactory.CreateAsync(xml);

        await UploadFileInternalAsync(filePath, stepName);
    }

    private async Task UploadFileInternalAsync(string filePath, string stepName)
    {
        ValidateFilePath(filePath);

        if (string.IsNullOrWhiteSpace(stepName))
        {
            stepName = $"Upload file '{Path.GetFileName(filePath)}'";
        }

        await _allure.StepAsync(stepName, async () =>
        {
            await _uploadDocumentPage.UploadFileAsync(filePath);

            StoreUploadedFileInScenarioContext(filePath);
            AttachUploadedFile(filePath);

            await _allure.AttachScreenshotAsync(
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

        _outputHelper.AddAttachmentAsLink(filePath);

        _allure.AttachFile(
            filePath,
            $"Uploaded RP14A File - {fileName}");
    }

    private static void ValidateXml(string xml)
    {
        if (string.IsNullOrWhiteSpace(xml))
        {
            throw new ArgumentException(
                "Generated RP14A XML cannot be null or empty.",
                nameof(xml));
        }
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
