using GovUk.Forms.HostApp.UI.Test.Pages.Login;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators;

public class SignInCoordinator(ISignInPage signInPage, IStartPage startPage)
{
    public async Task NavigateToSignInPageAsync()
    {
        await startPage.ClickOnStartNowAsync();
        await signInPage.VerifySignInPageIsDisplayedAsync();
    }

    public async Task SignInToServiceAsync(string email, string password)
    {
        await signInPage.SignInAsync(email, password);
    }

    public async Task EnterCredentialsAsync(string email, string password)
    {
        await EnterEmailAsync(email);
        await EnterPasswordAsync(password);
    }

    public async Task ShowPasswordAsync(string email, string password)
    {
        await EnterCredentialsAsync(email, password);
        await signInPage.VerifyPasswordIsMaskedAsync();
        await signInPage.TogglePasswordVisibilityAsync();
    }

    public async Task VerifyPasswordIsVisibleAsync()
    {
        await signInPage.VerifyPasswordIsVisibleAsync();
    }

    public async Task EnterEmailAsync(string email)
    {
        await signInPage.EnterEmailAsync(email);
    }

    public async Task EnterPasswordAsync(string password)
    {
        await signInPage.EnterPasswordAsync(password);
    }

    public async Task ClickSignInButtonAsync()
    {
        await signInPage.SubmitAsync();
    }

    public async Task VerifyErrorMessagesAsync(List<string> expectedMessages)
    {
        await signInPage.VerifyErrorMessagesAsync(expectedMessages);
        await signInPage.VerifyFieldErrorsAsync();
    }
}
