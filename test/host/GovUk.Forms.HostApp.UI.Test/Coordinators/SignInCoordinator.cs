using GovUk.Forms.HostApp.UI.Test.Pages.Login;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators;

public class SignInCoordinator(ISignInPage signInPage, IStartPage startPage)
{
    public async Task NavigateToSignInPageAsync()
    {
        await startPage.ClickOnStartNowAsync();
        await VerifyThatSignInPageIsDisplayedAsync();
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

    public async Task ShowPasswordAsync()
    {
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

    public async Task VerifyFieldErrorAsync(string field, string expectedMessage)
    {
        switch (field.ToLowerInvariant())
        {
            case "email":
                await signInPage.VerifyEmailErrorAsync(expectedMessage);
                break;

            case "password":
                await signInPage.VerifyPasswordErrorAsync(expectedMessage);
                break;

            case "summary":
                await signInPage.VerifyErrorMessagesAsync([expectedMessage]);
                break;

            default:
                throw new ArgumentException($"Unsupported field: {field}");
        }
    }

    public async Task VerifyThatSignInPageIsDisplayedAsync()
    {
        await signInPage.WaitForPageToLoadAsync();
    }
}
