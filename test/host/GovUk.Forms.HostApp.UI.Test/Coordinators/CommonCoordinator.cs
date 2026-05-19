using GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;
using GovUk.Forms.HostApp.UI.Test.Factories;
using GovUk.Forms.HostApp.UI.Test.Models;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators;

public class CommonCoordinator(
    StartPageCoordinator startPageCoordinator,
    SignInCoordinator signInCoordinator,
    DeclarationCoordinator declarationCoordinator,
    UploadDocumentCoordinator uploadDocument)
{
    public async Task VerifyThatUploadDocumentPageIsDisplayedAsync(TestUser? user = null)
    {
        user ??= UserFactory.DefaultUser();

        await startPageCoordinator.NavigateToLoginPageAsync();
        await signInCoordinator.SignInAsync(user.Email, user.Password);
        await declarationCoordinator.VerifyDeclarationPageIsDisplayedAsync();
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

    public async Task UploadAValidRP14AAndVerifyAsync(TestUser? user = null)
    {
        await VerifyThatUploadDocumentPageIsDisplayedAsync(user);
        await uploadDocument.UploadValidRp14aAsync();
        await uploadDocument.VerifyThatFileIsUploadedAsync();
    }
}
