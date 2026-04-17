
using GovUk.Forms.HostApp.UI.Test.Config.Driver;

namespace GovUk.Forms.HostApp.UI.Test.Pages.Login;

public class SignInPage : BasePage, ISignInPage
{
    private readonly IPlaywrightDriver _playwrightDriver;
    public SignInPage(IPlaywrightDriver playwrightDriver)
    {
        _playwrightDriver = playwrightDriver;
    }

    private new IPage Page => _playwrightDriver.Page;

    private ILocator Heading => Page.GetByRole(AriaRole.Heading, new() { Name = SignInLocators.Labels.Heading });
    private ILocator EmailInput => Page.GetByRole(AriaRole.Textbox, new() { Name = SignInLocators.Labels.EmailAddress, Exact = true });
    private ILocator PasswordInput => Page.GetByRole(AriaRole.Textbox, new() { Name = SignInLocators.Labels.PasswordInput, Exact = true });
    private ILocator ShowPasswordButton => Page.GetByRole(AriaRole.Button, new() { Name = SignInLocators.Labels.ShowPasswordButton });
    private ILocator SignInButton => Page.GetByRole(AriaRole.Button, new() { Name = SignInLocators.Labels.SignInButton });
    private ILocator ForgotPasswordLink => Page.GetByRole(AriaRole.Link, new() { Name = SignInLocators.Labels.ForgotPasswordLink });
    private ILocator BackLink => Page.GetByRole(AriaRole.Link, new() { Name = SignInLocators.Labels.BackLink });

    public async Task EnterEmailAsync(string email)
    {
        ArgumentException.ThrowIfNullOrEmpty(email, nameof(email));
        await PageContentLoadedAsync();
        await EmailInput.FillAsync(email);
    }


    public async Task EnterPasswordAsync(string password)
    {
        ArgumentException.ThrowIfNullOrEmpty(password, nameof(password));
        await PageContentLoadedAsync();
        await PasswordInput.FillAsync(password);
    }

    public async Task TogglePasswordVisibilityAsync()
    {
        await ShowPasswordButton.ClickAsync();
    }
    protected override async Task PageContentLoadedAsync()
    {
        await Expect(Heading).ToBeVisibleAsync();
        await Expect(EmailInput).ToBeVisibleAsync();
        await Expect(PasswordInput).ToBeVisibleAsync();
        await Expect(SignInButton).ToBeVisibleAsync();
    }

    public async Task SubmitAsync()
    {
        await SignInButton.ClickAsync();
    }

    public async Task SignInAsync(string email, string password)
    {
        await PageContentLoadedAsync();
        await EnterEmailAsync(email);
        await EnterPasswordAsync(password);
        await SubmitAsync();
    }

    public async Task ClickForgotPasswordAsync()
    {
        await ForgotPasswordLink.ClickAsync();
    }

    public async Task ClickBackAsync()
    {
        await BackLink.ClickAsync();
    }

    public async Task<string> GetEmailValueAsync()
    {
        return await EmailInput.InputValueAsync();
    }

    public async Task<string> GetPasswordValueAsync()
    {
        return await PasswordInput.InputValueAsync();
    }
    public async Task VerifySignInPageIsDisplayedAsync()
    {
        await WaitForPageToLoadAsync();
    }

    public async Task<bool> IsPasswordMaskedAsync()
    {
        string? type = await PasswordInput.GetAttributeAsync("type");
        return type == "password";
    }

    public async Task<bool> IsPasswordVisibleAsync()
    {
        string? type = await PasswordInput.GetAttributeAsync("type");
        return type == "text";
    }
}
