using GovUk.Forms.HostApp.UI.Test.Models.TestData;
using GovUk.Forms.HostApp.UI.Test.Pages.Upload;
using GovUk.Forms.HostApp.UI.Test.Support;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;

public class UploadVerificationCoordinator : IUploadVerificationCoordinator
{
    private readonly IUploadDocumentPage _uploadDocumentPage;
    private readonly ScenarioContext _scenarioContext;

    public UploadVerificationCoordinator(
        IUploadDocumentPage uploadDocumentPage,
        ScenarioContext scenarioContext)
    {
        _uploadDocumentPage = uploadDocumentPage;
        _scenarioContext = scenarioContext;
    }

    public async Task VerifyThatFileIsUploadedAsync()
    {
        string expectedFileName =
            _scenarioContext.Get<string>(ScenarioConstant.UploadedFileName);

        string actualUploadedFileName =
            await _uploadDocumentPage.GetUploadedFileNameAsync();

        Assert.That(
            actualUploadedFileName,
            Is.EqualTo(expectedFileName),
            $"Expected uploaded file '{expectedFileName}', but actual was '{actualUploadedFileName}'.");
    }

    public async Task VerifyOnlyOneFileUploadedAsync()
    {
        string expectedFileName =
            _scenarioContext.Get<string>(ScenarioConstant.UploadedFileName);

        IReadOnlyList<string> uploadedFiles =
            await _uploadDocumentPage.GetUploadedFileNamesAsync();

        int matchingFileCount = uploadedFiles.Count(file =>
            file.Equals(expectedFileName, StringComparison.OrdinalIgnoreCase));

        Assert.That(
            matchingFileCount,
            Is.EqualTo(1),
            $"Expected only one instance of '{expectedFileName}', but found {matchingFileCount}. " +
            $"Actual files: {string.Join(", ", uploadedFiles)}");
    }

    public async Task VerifyInvalidFileExtensionErrorAsync(UploadFileError uploadFileError)
    {
        await _uploadDocumentPage.VerifyUploadFileErrorAsync(uploadFileError);
    }
}
