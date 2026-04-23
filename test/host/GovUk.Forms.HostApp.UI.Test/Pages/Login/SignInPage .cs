
using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Pages.Common;

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
    private ILocator BackButton => Page.GetByRole(AriaRole.Link, new() { Name = SharedLoactors.BackButton, Exact = true });
    private ILocator GOVUKLink => Page.GetByRole(AriaRole.Img, new() { Name = SharedLoactors.GOVUKLink });
    private ILocator UploadRedundancyPaymentFormsLink => Page.GetByRole(AriaRole.Link, new() { Name = SignInLocators.Labels.UploadRedundancyPaymentForms });
    private ILocator ErrorSummary => Page.Locator(SignInLocators.Selectors.ErrorSummary);
    private ILocator ErrorSummaryItems => ErrorSummary.GetByRole(AriaRole.Link);
    private ILocator EmailError => Page.Locator(SignInLocators.Selectors.EmailError);
    private ILocator PasswordError => Page.Locator(SignInLocators.Selectors.PasswordError);
    private ILocator BetaText => Page.GetByText(SharedLoactors.Beta, new() { Exact = true });

    protected override async Task PageContentLoadedAsync()
    {
        await Expect(GOVUKLink).ToBeVisibleAsync();
        await Expect(BetaText).ToBeVisibleAsync();
        await Expect(Heading).ToBeVisibleAsync();
        await Expect(EmailInput).ToBeVisibleAsync();
        await Expect(PasswordInput).ToBeVisibleAsync();
        await Expect(SignInButton).ToBeVisibleAsync();
        await Expect(UploadRedundancyPaymentFormsLink).ToBeVisibleAsync();
        await Expect(BackButton).ToBeVisibleAsync();
        await Expect(ForgotPasswordLink).ToBeVisibleAsync();
    }

    public async Task EnterEmailAsync(string email)
    {
        ArgumentNullException.ThrowIfNull(email);
        await EmailInput.FillAsync(email);
    }

    public async Task EnterPasswordAsync(string password)
    {
        ArgumentNullException.ThrowIfNull(password);
        await PasswordInput.FillAsync(password);
    }

    public async Task EnterCredentialsAsync(string email, string password)
    {
        await EnterEmailAsync(email);
        await EnterPasswordAsync(password);
    }

    public async Task TogglePasswordVisibilityAsync()
    {
        await ShowPasswordButton.ClickAsync();
    }

    public async Task SubmitAsync()
    {
        await SignInButton.ClickAsync();
    }

    public async Task SignInAsync(string email, string password)
    {
        await WaitForPageToLoadAsync();
        await EnterCredentialsAsync(email, password);
        await SubmitAsync();
    }

    public async Task ClickForgotPasswordAsync()
    {
        await ForgotPasswordLink.ClickAsync();
    }

    public async Task ClickBackAsync()
    {
        await BackButton.ClickAsync();
    }

    public async Task<string> GetEmailValueAsync()
    {
        return await EmailInput.InputValueAsync();
    }

    public async Task<string> GetPasswordValueAsync()
    {
        return await PasswordInput.InputValueAsync();
    }

    public async Task VerifyPasswordIsMaskedAsync()
    {
        string? type = await PasswordInput.GetAttributeAsync("type");

        if (!string.Equals(type, SignInLocators.password, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Expected password field type to be 'password' but found '{type}'.");
        }
    }

    public async Task VerifyPasswordIsVisibleAsync()
    {
        string? type = await PasswordInput.GetAttributeAsync("type");

        if (!string.Equals(type, SignInLocators.text, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Expected password field type to be 'text' but found '{type}'.");
        }
    }

    public async Task VerifyEmailErrorAsync(string expectedMessage)
    {
        await Expect(EmailError).ToBeVisibleAsync();
        await Expect(EmailError).ToContainTextAsync(expectedMessage);
    }

    public async Task VerifyPasswordErrorAsync(string expectedMessage)
    {
        await Expect(PasswordError).ToBeVisibleAsync();
        await Expect(PasswordError).ToContainTextAsync(expectedMessage);
    }

    public async Task VerifyEmailAndPasswordFieldErrorsAsync()
    {
        await Expect(EmailError).ToBeVisibleAsync();
        await Expect(PasswordError).ToBeVisibleAsync();
    }

    public async Task VerifyErrorMessagesAsync(List<string> expectedMessages)
    {
        IReadOnlyList<string> actualMessages = await ErrorSummaryItems.AllInnerTextsAsync();

        foreach (string expected in expectedMessages)
        {
            if (!actualMessages.Any(m => m.Contains(expected, StringComparison.Ordinal)))
            {
                throw new InvalidOperationException(
                    $"Expected error message '{expected}' was not found. Actual: {string.Join(", ", actualMessages)}");
            }
        }
    }
}
