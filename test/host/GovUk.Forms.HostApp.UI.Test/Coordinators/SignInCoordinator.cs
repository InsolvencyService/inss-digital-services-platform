using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Pages.Login;
using GovUk.Forms.HostApp.UI.Test.Tags;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators;

public sealed class SignInCoordinator
{
    private readonly ISignInPage _signInPage;
    private readonly IStartPage _startPage;
    private readonly IAllureReportingHelper _reportingHelper;
    private readonly IPlaywrightDriver _playwrightDriver;

    public SignInCoordinator(
        ISignInPage signInPage,
        IStartPage startPage,
        IAllureReportingHelper reportingHelper,
        IPlaywrightDriver playwrightDriver)
    {
        _signInPage = signInPage
            ?? throw new ArgumentNullException(nameof(signInPage));

        _startPage = startPage
            ?? throw new ArgumentNullException(nameof(startPage));

        _reportingHelper = reportingHelper
            ?? throw new ArgumentNullException(nameof(reportingHelper));

        _playwrightDriver = playwrightDriver
            ?? throw new ArgumentNullException(nameof(playwrightDriver));
    }

    public async Task NavigateToSignInPageAsync()
    {
        await _reportingHelper.StepAsync("Navigate to Sign-In page", async () =>
        {
            await _startPage.ClickOnStartNowAsync();
            await VerifySignInPageIsLoadedAsync();
        });
    }

    public async Task SignInAsync(string email, string password)
    {
        await _reportingHelper.StepAsync("Sign in user", async () =>
        {
            _reportingHelper.AddParameter("Email", email);
            _reportingHelper.AddParameter("Password", "***");

            await EnterCredentialsAsync(email, password);
            await _signInPage.SubmitAsync();

            await _reportingHelper.AttachScreenshotAsync(_playwrightDriver.Page, "After sign-in submission");
        });
    }

    public async Task TogglePasswordVisibilityAsync()
    {
        await _reportingHelper.StepAsync("Toggle password visibility", async () =>
        {
            await _signInPage.VerifyPasswordIsMaskedAsync();
            await _signInPage.TogglePasswordVisibilityAsync();
            await _signInPage.VerifyPasswordIsVisibleAsync();
        });
    }

    public async Task VerifyFieldErrorAsync(FieldErrorType fieldType, string expectedMessage)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(expectedMessage);

        await _reportingHelper.StepAsync($"Verify field error: {fieldType}", async () =>
        {
            Func<string, Task> verifyAction = fieldType switch
            {
                FieldErrorType.Email => _signInPage.VerifyEmailErrorAsync,
                FieldErrorType.Password => _signInPage.VerifyPasswordErrorAsync,
                FieldErrorType.Summary => msg => _signInPage.VerifyErrorMessagesAsync([msg]),
                _ => throw new ArgumentException(
                    $"Unexpected field type: {fieldType}",
                    nameof(fieldType))
            };

            await verifyAction(expectedMessage);

            await _reportingHelper.AttachScreenshotAsync(
                _playwrightDriver.Page,
                $"Verified {fieldType} error");
        });
    }

    public async Task VerifyErrorSummaryAsync(IEnumerable<string> expectedMessages)
    {
        ArgumentNullException.ThrowIfNull(expectedMessages);

        List<string> messageList = [.. expectedMessages];

        if (messageList.Count == 0)
        {
            throw new ArgumentException(
                "At least one error message must be provided.",
                nameof(expectedMessages));
        }

        if (messageList.Any(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException(
                "All error messages must be non-empty strings.",
                nameof(expectedMessages));
        }

        await _reportingHelper.StepAsync("Verify error summary messages", async () =>
        {
            await _signInPage.VerifyErrorMessagesAsync(messageList);
            await _reportingHelper.AttachScreenshotAsync(_playwrightDriver.Page, "Error summary verified");
        });
    }

    public async Task VerifyFieldLevelErrorsAsync()
    {
        await _reportingHelper.StepAsync("Verify field-level errors", async () =>
        {
            await _signInPage.VerifyEmailAndPasswordFieldErrorsAsync();
            await _reportingHelper.AttachScreenshotAsync(_playwrightDriver.Page, "Field-level errors verified");
        });
    }

    public async Task VerifyAccountIsBlockedAsync(string errorMessage)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(errorMessage);

        await _reportingHelper.StepAsync("Verify account is blocked", async () =>
        {
            await VerifyErrorSummaryAsync([errorMessage]);
            await _signInPage.VerifyEmailErrorAsync(errorMessage);
        });
    }

    public async Task VerifySignInPageIsLoadedAsync()
    {
        await _reportingHelper.StepAsync("Verify Sign-In page is loaded", async () =>
        {
            await _signInPage.WaitForPageToLoadAsync();
            await _reportingHelper.AttachScreenshotAsync(_playwrightDriver.Page, "Sign-In page loaded");
        });
    }

    public async Task EnterCredentialsAsync(string email, string password)
    {
        await _reportingHelper.StepAsync("Enter user credentials", async () =>
        {
            await EnterEmailAsync(email);
            await EnterPasswordAsync(password);
            await _reportingHelper.AttachScreenshotAsync(_playwrightDriver.Page, "Credentials entered");
        });
    }

    public async Task ClickOnSignInAsync()
    {
        await _reportingHelper.StepAsync("Click on Sign-In button", async () =>
        {
            await _signInPage.SubmitAsync();
            await _reportingHelper.AttachScreenshotAsync(_playwrightDriver.Page, "Sign-In clicked");
        });
    }

    public async Task VerifyPasswordIsVisibleAsync()
    {
        await _reportingHelper.StepAsync("Verify password visibility", _signInPage.VerifyPasswordIsVisibleAsync);
    }

    public async Task EnterEmailAsync(string email)
    {
        await _reportingHelper.StepAsync($"Enter email: {email}", async () =>
        {
            await _signInPage.EnterEmailAsync(email);
        });
    }

    public async Task EnterPasswordAsync(string password)
    {
        await _reportingHelper.StepAsync("Enter password", async () =>
        {
            await _signInPage.EnterPasswordAsync(password);
        });
    }

    public async Task GoBackAsync() =>
        await _reportingHelper.StepAsync("Navigate to previous page", _signInPage.ClickBackAsync);
}
