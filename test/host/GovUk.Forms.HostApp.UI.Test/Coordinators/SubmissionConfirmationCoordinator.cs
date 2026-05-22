using GovUk.Forms.HostApp.UI.Test.Pages.Submit;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators;

public class SubmissionConfirmationCoordinator
    (ISubmissionConfirmationPage submissionConfirmation)
{

    public async Task VerifySubmissionConfirmationPageIsDisplayedAsync()
    {
        await submissionConfirmation.WaitForPageToLoadAsync();
    }

    public async Task UploadAnotherFormAsync()
    {
        await VerifySubmissionConfirmationPageIsDisplayedAsync();
        await submissionConfirmation.ClickUploadAnotherFormButtonAsync();
    }
}
