using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;
using GovUk.Forms.HostApp.UI.Test.Factories;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Pages.Common;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators;

public class CommonCoordinator(
    StartPageCoordinator startPageCoordinator,
    SignInCoordinator signInCoordinator,
    DeclarationCoordinator declarationCoordinator,
    UploadDocumentCoordinator uploadDocument,
    CheckYourAnswersCoordinator checkYourAnswers,
    SubmissionConfirmationCoordinator submissionConfirmation,
    IPlaywrightDriver playwrightDriver,
    ICommonPage commonPage)
{
    public async Task VerifyThatUploadDocumentPageIsDisplayedAsync(TestUser? user = null)
    {
        user ??= UserFactory.GetUser("InssTestManTwo");

        await VerifyThatDeclarationPageIsDisplayedAsync(user);
        await declarationCoordinator.NavigateToUploadAFilePageAsync();
        await uploadDocument.VerifyUploadDocumentPageIsDisplayedAsync();
    }

    public async Task VerifyThatDeclarationPageIsDisplayedAsync(TestUser? user = null)
    {
        user ??= UserFactory.DefaultUser();

        await startPageCoordinator.NavigateToLoginPageAsync();
        await signInCoordinator.SignInAsync(user.Email, user.Password);
        await declarationCoordinator.VerifyDeclarationPageIsDisplayedAsync();
    }

    public async Task VerifyThatStartPageIsDisplayedAsync(TestUser? user = null)
    {
        user ??= UserFactory.DefaultUser();

        await startPageCoordinator.NavigateToLoginPageAsync();
        await signInCoordinator.SignInAsync(user.Email, user.Password);
        await declarationCoordinator.ReturnToStartPageAsync();
        await startPageCoordinator.VerifyStartPageIsDisplayedAsync();
    }

    public async Task UploadAValidRP14AAndVerifyAsync()
    {
        await uploadDocument.UploadValidRp14aAsync();
        await uploadDocument.VerifyThatFileIsUploadedAsync();
    }

    public async Task VerifySubmissionComfirmationPageIsDisplayedAsync()
    {
        await UploadAValidRP14AAndVerifyAsync();
        await VerifyThatCheckYourAnswersPageIsDisplayedAsync();
        await checkYourAnswers.ClickOnSubmitButtonAsync();
        await submissionConfirmation.VerifySubmissionConfirmationPageIsDisplayedAsync();
    }

    public async Task VerifyThatCheckYourAnswersPageIsDisplayedAsync()
    {
        await UploadAValidRP14AAndVerifyAsync();
        await uploadDocument.NavigateToSubmitPageAsync();
        await checkYourAnswers.VerifyCheckYourAnswersPageIsDisplayedAsync();
    }

    public async Task SignOutAndVerifyAsync()
    {
        await commonPage.SignOutAsync(playwrightDriver.Page);
        await startPageCoordinator.VerifyStartPageIsDisplayedAsync();
    }
}
