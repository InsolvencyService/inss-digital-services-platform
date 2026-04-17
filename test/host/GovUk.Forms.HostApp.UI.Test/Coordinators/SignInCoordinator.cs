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

    public async Task ShowPasswordAsync(string email, string password)
    {
        await signInPage.EnterEmailAsync(email);
        await signInPage.EnterPasswordAsync(password);
        // Assert initially masked
        bool isMasked = await signInPage.IsPasswordMaskedAsync();
        Assert.True(isMasked);
        await signInPage.TogglePasswordVisibilityAsync();
    }

    public async Task VerifyPasswordIsVisibleAsync()
    {
        bool isVisible = await signInPage.IsPasswordVisibleAsync();
        Assert.True(isVisible);
    }
}
