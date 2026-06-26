using GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Pages.Submit;
using GovUk.Forms.HostApp.UI.Test.Support;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators;

public sealed class CheckYourAnswersCoordinator(
    IUploadDocumentSummaryPage summaryPage,
    IUploadNavigationCoordinator uploadNavigationCoordinator,
    ISubmissionConfirmationPage submitCompletedPage,
    ScenarioContext scenarioContext,
    TestArtifacts testArtifacts)
    : BaseCoordinator(testArtifacts)
{
    public async Task VerifyCheckYourAnswersPageIsDisplayedAsync()
    {
        await ExecuteStepAsync(
            "Verify Check Your Answers page is displayed",
            async () =>
            {
                await summaryPage.WaitForPageToLoadAsync();

                string caseReference =
                    scenarioContext.Get<string>(ScenarioConstant.CaseReference);

                string uploadedFileName =
                    scenarioContext.Get<string>(ScenarioConstant.UploadedFileName);

                await summaryPage.VerifyCaseReferenceNumberAsync(caseReference);
                await summaryPage.VerifyUploadedDocumentAsync(uploadedFileName);

                AddAllureLog(
                    "CheckYourAnswers",
                    $"Verified case reference: {caseReference}, uploaded document: {uploadedFileName}");
            });
    }

    public async Task ClickOnSubmitButtonAsync()
    {
        await ExecuteStepAsync(
            "Click Submit button",
            async () =>
            {
                await summaryPage.ClickSubmitAsync();

                AddAllureLog(
                    "SubmitButton",
                    "Submit button clicked successfully");
            });
    }

    public async Task ChangeUploadedDocumentAsync()
    {
        await ExecuteStepAsync(
            "Change uploaded document",
            async () =>
            {
                await summaryPage.ClickChangeAsync();

                AddAllureLog(
                    "ChangeDocument",
                    "Change uploaded document action triggered");
            });
    }

    public async Task NavigateBackAsync()
    {
        await ExecuteStepAsync(
            "Navigate back from Check Your Answers page",
            async () =>
            {
                await uploadNavigationCoordinator
                    .ClickOnBackButtonAsync();

                AddAllureLog(
                    "NavigateBack",
                    "Navigated back successfully");
            });
    }

    public async Task VerifySubmitCompletedPageIsDisplayedAsync()
    {
        await ExecuteStepAsync(
            "Verify Submission Completed page is displayed",
            async () =>
            {
                await submitCompletedPage.WaitForPageToLoadAsync();

                AddAllureLog(
                    "SubmissionCompleted",
                    "Submission completed page displayed successfully");
            });
    }
}

