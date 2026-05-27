using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Pages.Submit;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators;

public sealed class SubmissionConfirmationCoordinator(
    ISubmissionConfirmationPage submissionConfirmation,
    TestArtifacts testArtifacts)
    : BaseCoordinator(testArtifacts)
{
    public async Task VerifySubmissionConfirmationPageIsDisplayedAsync()
    {
        await ExecuteStepAsync(
            "Verify Submission Confirmation page is displayed",
            async () =>
            {
                await submissionConfirmation.WaitForPageToLoadAsync();

                AddAllureLog(
                    "SubmissionConfirmation",
                    "Submission Confirmation page displayed successfully");
            });
    }

    public async Task UploadAnotherFormAsync()
    {
        await ExecuteStepAsync(
            "Upload another form",
            async () =>
            {
                await submissionConfirmation.WaitForPageToLoadAsync();

                await submissionConfirmation
                    .ClickUploadAnotherFormButtonAsync();

                AddAllureLog(
                    "UploadAnotherForm",
                    "Upload another form button clicked successfully");
            });
    }
}