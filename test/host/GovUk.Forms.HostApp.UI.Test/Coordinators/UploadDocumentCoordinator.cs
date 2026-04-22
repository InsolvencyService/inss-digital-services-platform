using GovUk.Forms.HostApp.UI.Test.Pages.Upload;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators;

public class UploadDocumentCoordinator(IUploadDocumentPage uploadDocumentPage, ScenarioContext scenarioContext) : BaseCoordinator
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

}
