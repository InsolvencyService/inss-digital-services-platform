using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Pages.Upload;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;

/// <summary>
/// Implementation of upload page display operations.
/// THREAD-SAFE: All data stored in scenario-scoped context.
/// </summary>
public class UploadPageCoordinator : IUploadPageCoordinator
{
    private readonly IUploadDocumentPage _uploadDocumentPage;
    private readonly IAllureReportingHelper _allure;
    private readonly IPlaywrightDriver _playwrightDriver;
    public UploadPageCoordinator(
        IUploadDocumentPage uploadDocumentPage,
        IAllureReportingHelper allure,
        IPlaywrightDriver playwrightDriver)
    {
        _uploadDocumentPage = uploadDocumentPage;
        _allure = allure;
        _playwrightDriver = playwrightDriver;
    }

    public async Task VerifyUploadDocumentPageIsDisplayedAsync()
    {
        await _allure.StepAsync("File upload page is displayed", async () =>
        {
            await _uploadDocumentPage.WaitForPageToLoadAsync();

            await _allure.AttachScreenshotAsync(
                _playwrightDriver.Page,
                "Upload File Page");
        });
    }

    public async Task ExpandCommonIssuesWhenUploadingRP14AFormsAsync()
    {
        await _uploadDocumentPage.ExpandCommonIssuesWhenUploadingRP14AFormsAsync();
    }

}
