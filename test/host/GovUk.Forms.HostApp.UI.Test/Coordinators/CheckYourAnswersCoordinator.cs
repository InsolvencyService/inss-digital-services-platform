using GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;
using GovUk.Forms.HostApp.UI.Test.Pages.Submit;
using GovUk.Forms.HostApp.UI.Test.Support;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators;

public class CheckYourAnswersCoordinator(
        IUploadDocumentSummaryPage summaryPage,
        IUploadNavigationCoordinator uploadNavigationCoordinator,
        ISubmissionConfirmationPage submitCompletedPage,
        ScenarioContext scenarioContext)
{
    public async Task VerifyCheckYourAnswersPageIsDisplayedAsync()
    {
        await summaryPage.WaitForPageToLoadAsync();

        string uploadedFileName =
            scenarioContext.Get<string>(ScenarioConstant.UploadedFileName);

        await summaryPage.VerifyUploadedDocumentAsync(uploadedFileName);
    }

    public async Task ClickOnSubmitButtonAsync()
    {
        await summaryPage.ClickSubmitAsync();
    }

    public async Task ChangeUploadedDocumentAsync()
    {
        await summaryPage.ClickChangeAsync();
    }

    public async Task NavigateBackAsync()
    {
        await uploadNavigationCoordinator.ClickOnBackButtonAsync();
    }

    public async Task VerifySubmitCompletedPageIsDisplayedAsync()
    {
        await submitCompletedPage.WaitForPageToLoadAsync();
    }
}

