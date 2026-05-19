using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Pages.Login;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators;

public class SignInCoordinator
{
    private readonly ISignInPage _signInPage;
    private readonly IStartPage _startPage;
    private readonly IAllureReportingHelper _allure;
    private readonly IPlaywrightDriver _playwrightDriver;

    public SignInCoordinator(
        ISignInPage signInPage,
        IStartPage startPage,
        IAllureReportingHelper allure,
        IPlaywrightDriver playwrightDriver)
    {
        _signInPage = signInPage;
        _startPage = startPage;
        _allure = allure;
        _playwrightDriver = playwrightDriver;
    }

    public async Task NavigateToSignInPageAsync()
    {
        await _allure.StepAsync("Navigate to Sign-In page", async () =>
        {
            await _startPage.ClickOnStartNowAsync();
            await VerifySignInPageIsLoadedAsync();
            await _allure.AttachScreenshotAsync(_playwrightDriver.Page, "Sign-In page loaded");
        });
    }

    public async Task SignInAsync(string email, string password)
    {
        await _allure.StepAsync("Sign in user", async () =>
        {
            _allure.AddParameter("Email", email);
            _allure.AddParameter("Password", "***"); // mask password

            await EnterCredentilasAsync(email, password);
            await _signInPage.SubmitAsync();

            await _allure.AttachScreenshotAsync(_playwrightDriver.Page, "After sign-in submission");
        });
    }

    public async Task TogglePasswordVisibilityAsync()
    {
        await _allure.StepAsync("Toggle password visibility", async () =>
        {
            await _signInPage.VerifyPasswordIsMaskedAsync();
            await _signInPage.TogglePasswordVisibilityAsync();
            await _signInPage.VerifyPasswordIsVisibleAsync();
        });
    }

    public async Task VerifyFieldErrorAsync(FieldErrorType fieldType, string expectedMessage)
    {
        await _allure.StepAsync($"Verify field error: {fieldType}", async () =>
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(expectedMessage);

            switch (fieldType)
            {
                case FieldErrorType.Email:
                    await _signInPage.VerifyEmailErrorAsync(expectedMessage);
                    break;

                case FieldErrorType.Password:
                    await _signInPage.VerifyPasswordErrorAsync(expectedMessage);
                    break;

                case FieldErrorType.Summary:
                    await _signInPage.VerifyErrorMessagesAsync([expectedMessage]);
                    break;

                default:
                    throw new ArgumentException($"Unexpected field type: {fieldType}", nameof(fieldType));
            }

            await _allure.AttachScreenshotAsync(_playwrightDriver.Page, $"Verified {fieldType} error");
        });
    }

    public async Task VerifyErrorSummaryAsync(IEnumerable<string> expectedMessages)
    {
        await _allure.StepAsync("Verify error summary messages", async () =>
        {
            if (expectedMessages == null)
            {
                throw new ArgumentNullException(nameof(expectedMessages), "Error messages collection cannot be null.");
            }

            List<string> messageList = expectedMessages.ToList();

            if (messageList.Count == 0)
            {
                throw new ArgumentException("At least one error message must be provided.", nameof(expectedMessages));
            }

            if (messageList.Any(string.IsNullOrWhiteSpace))
            {
                throw new ArgumentException("All error messages must be non-empty strings.", nameof(expectedMessages));
            }

            await _signInPage.VerifyErrorMessagesAsync(messageList);
            await _allure.AttachScreenshotAsync(_playwrightDriver.Page, "Error summary verified");
        });
    }

    public async Task VerifyFieldLevelErrorsAsync()
    {
        await _allure.StepAsync("Verify field-level errors", async () =>
        {
            await _signInPage.VerifyEmailAndPasswordFieldErrorsAsync();
            await _allure.AttachScreenshotAsync(_playwrightDriver.Page, "Field-level errors verified");
        });
    }

    public async Task VerifyAccountIsBlockedAsync(string errorMessage)
    {
        await _allure.StepAsync("Verify account is blocked", async () =>
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(errorMessage);
            await VerifyErrorSummaryAsync([errorMessage]);
            await _signInPage.VerifyEmailErrorAsync(errorMessage);
        });
    }

    public async Task VerifySignInPageIsLoadedAsync()
    {
        await _allure.StepAsync("Verify Sign-In page is loaded", async () =>
        {
            await _signInPage.WaitForPageToLoadAsync();
            await _allure.AttachScreenshotAsync(_playwrightDriver.Page, "Sign-In page loaded");
        });
    }

    public async Task EnterCredentilasAsync(string email, string password)
    {
        await _allure.StepAsync("Enter user credentials", async () =>
        {
            await EnterEmailAsync(email);
            await EnterPasswordAsync(password);
            await _allure.AttachScreenshotAsync(_playwrightDriver.Page, "Credentials entered");
        });
    }

    public async Task ClickOnSignInAsync()
    {
        await _allure.StepAsync("Click on Sign-In button", async () =>
        {
            await _signInPage.SubmitAsync();
            await _allure.AttachScreenshotAsync(_playwrightDriver.Page, "Sign-In clicked");
        });
    }

    public async Task VerifyPasswordIsVisibleAsync()
    {
        await _allure.StepAsync("Verify password visibility", _signInPage.VerifyPasswordIsVisibleAsync);
    }

    public async Task EnterEmailAsync(string email)
    {
        await _allure.StepAsync($"Enter email: {email}", async () =>
        {
            await _signInPage.EnterEmailAsync(email);
        });
    }

    public async Task EnterPasswordAsync(string password)
    {
        await _allure.StepAsync("Enter password", async () =>
        {
            await _signInPage.EnterPasswordAsync(password);
        });
    }

    public enum FieldErrorType
    {
        Email,
        Password,
        Summary
    }
}



