using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Pages.Upload;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;

/// <summary>
/// Implementation of navigation operations.
/// THREAD-SAFE: Browser navigation is scenario-scoped
/// RESPONSIBILITY: Encapsulates all page navigation and transitions
/// </summary>
public class UploadNavigationCoordinator : IUploadNavigationCoordinator
{
    private readonly IUploadDocumentPage _uploadDocumentPage;
    private readonly ScenarioContext _scenarioContext;
    private readonly IAllureReportingHelper _allure;
    private readonly IPlaywrightDriver _playwrightDriver;

    public UploadNavigationCoordinator(
        IUploadDocumentPage uploadDocumentPage,
        ScenarioContext scenarioContext,
        IAllureReportingHelper allure,
        IPlaywrightDriver playwrightDriver)
    {
        _uploadDocumentPage = uploadDocumentPage;
        _scenarioContext = scenarioContext;
        _allure = allure;
        _playwrightDriver = playwrightDriver;
    }

    public async Task ClickOnContinueButtonAsync()
    {
        await _uploadDocumentPage.ClickOnContinueButtonAsync();
    }

    public async Task ClickOnBackButtonAsync()
    {
        await _uploadDocumentPage.ClickOnBackButtonAsync();
    }

    public async Task NavigateToFeedbackPageAsync()
    {
        IPage feedbackPage = await NavigateAsync(_uploadDocumentPage.ClickOnGiveFeedbackLinkAsync);
        _scenarioContext.Set(feedbackPage);
    }

    public async Task NavigateToSubmitPageAsync()
    {
        await _allure.StepAsync("Navigate to submit page", async () =>
        {
            await _uploadDocumentPage.ClickOnContinueButtonAsync();

            await _allure.AttachScreenshotAsync(
                _playwrightDriver.Page,
                "Submit Page");
        });
    }

    private async Task<IPage> NavigateAsync(Func<Task> navigationAction)
    {
        await navigationAction.Invoke();
        return _playwrightDriver.Page;
    }
}

