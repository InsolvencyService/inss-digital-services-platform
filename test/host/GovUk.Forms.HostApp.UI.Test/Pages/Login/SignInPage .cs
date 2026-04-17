
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
    private ILocator EmailError => Page.GetByText("Error: Enter an email address");
    private ILocator PasswordError => Page.GetByText("Error: Enter a password");
    private ILocator ErrorSummary => Page.Locator(".govuk-error-summary");
    private ILocator ErrorSummaryItems => ErrorSummary.GetByRole(AriaRole.Link);

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

    public async Task VerifyPasswordIsMaskedAsync()
    {
        string? type = await PasswordInput.GetAttributeAsync("type");

        if (!string.Equals(type, "password", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Expected password field type to be 'password' but found '{type}'.");
        }
    }

    public async Task VerifyPasswordIsVisibleAsync()
    {
        string? type = await PasswordInput.GetAttributeAsync("type");

        if (!string.Equals(type, "text", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Expected password field type to be 'password' but found '{type}'.");
        }
    }

    public async Task VerifyFieldErrorsAsync()
    {
        await Expect(EmailError).ToBeVisibleAsync();
        await Expect(PasswordError).ToBeVisibleAsync();
    }

    public async Task VerifyErrorMessagesAsync(List<string> expectedMessages)
    {
        IReadOnlyList<string> actualMessages = await ErrorSummaryItems.AllInnerTextsAsync();

        foreach (string expected in expectedMessages)
        {
            if (!actualMessages.Any(m => m.Contains(expected)))
            {
                throw new InvalidOperationException(
                    $"Expected error message '{expected}' was not found. Actual: {string.Join(", ", actualMessages)}");
            }
        }
    }
}
