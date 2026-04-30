using GovUk.Forms.HostApp.UI.Test.Support;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators;

public class CommonCoordinator(
    StartPageCoordinator startPageCoordinator,
    SignInCoordinator signInCoordinator,
    DeclarationCoordinator declarationCoordinator,
    UploadDocumentCoordinator uploadDocument)
{

    public async Task VerifyThatUploadDocumentPageIsDisplayedAsync(string emailAddress = ScenarioConstant.EmailAddress, string password = ScenarioConstant.Password)
    {
        await startPageCoordinator.NavigateToLoginPageAsync();
        await signInCoordinator.SignInToServiceAsync(emailAddress, password);
        await declarationCoordinator.VerifyDeclarationPageIsDisplayedAsync();
        await declarationCoordinator.NavigateToUploadAFilePageAsync();
        await uploadDocument.VerifyUploadDocumentPageIsDisplayedAsync();
    }

    public async Task VerifyThatDeclarationPageIsDisplayedAsync()
    {
        await startPageCoordinator.NavigateToLoginPageAsync();
        await signInCoordinator.SignInToServiceAsync(ScenarioConstant.EmailAddress, ScenarioConstant.Password);
        await declarationCoordinator.VerifyDeclarationPageIsDisplayedAsync();
    }

    public async Task VerifyThatStartPageIsDisplayedAsync()
    {
        await startPageCoordinator.NavigateToLoginPageAsync();
        await signInCoordinator.SignInToServiceAsync(ScenarioConstant.EmailAddress, ScenarioConstant.Password);
        await declarationCoordinator.ReturnToStartPageAsync();
        await startPageCoordinator.VerifyStartPageIsDisplayedAsync();
    }
}
