namespace GovUk.Forms.HostApp.UI.Test.Pages.Login;

public interface ISignInPage
{
    Task EnterEmailAsync(string email);
    Task EnterPasswordAsync(string password);
    Task TogglePasswordVisibilityAsync();
    Task SubmitAsync();
    Task SignInAsync(string email, string password);
    Task ClickForgotPasswordAsync();
    Task ClickBackAsync();

    Task<string> GetEmailValueAsync();
    Task<string> GetPasswordValueAsync();
    Task VerifySignInPageIsDisplayedAsync();
    Task VerifyPasswordIsMaskedAsync();
    Task VerifyPasswordIsVisibleAsync();
    Task VerifyErrorMessagesAsync(List<string> expectedMessages);
    Task VerifyFieldErrorsAsync();
}
