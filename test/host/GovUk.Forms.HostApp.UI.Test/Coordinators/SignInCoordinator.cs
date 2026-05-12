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
        await _startPage.ClickOnStartNowAsync();
        await VerifySignInPageIsLoadedAsync();
    }

    public async Task SignInAsync(string email, string password)
    {
        await _allure.StepAsync("Sign in user", async () =>
        {
            _allure.AddParameter("Email", email);
            _allure.AddParameter("Password", "***");

            await EnterCredentilasAsync(email, password);
            await _allure.AttachScreenshotAsync(
                _playwrightDriver.Page,
                "Sign-in form before submission");

            await _signInPage.SubmitAsync();

            await _allure.AttachScreenshotAsync(
                _playwrightDriver.Page,
                "Sign-in form after submission");
        });
    }


    public async Task TogglePasswordVisibilityAsync()
    {
        await _signInPage.VerifyPasswordIsMaskedAsync();
        await _signInPage.TogglePasswordVisibilityAsync();
        await _signInPage.VerifyPasswordIsVisibleAsync();
    }

    public async Task VerifyFieldErrorAsync(FieldErrorType fieldType, string expectedMessage)
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
                throw new ArgumentException(
                    $"Unexpected field type: {fieldType}", nameof(fieldType));
        }
    }

    public async Task VerifyErrorSummaryAsync(IEnumerable<string> expectedMessages)
    {
        if (expectedMessages == null)
        {
            throw new ArgumentNullException(
                nameof(expectedMessages),
                "Error messages collection cannot be null.");
        }

        List<string> messageList = expectedMessages.ToList();

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

        await _signInPage.VerifyErrorMessagesAsync(messageList);
    }


    public async Task VerifyFieldLevelErrorsAsync()
    {
        await _signInPage.VerifyEmailAndPasswordFieldErrorsAsync();
    }

    public async Task VerifyAccountIsBlockedAsync(string errorMessage)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(errorMessage);

        await VerifyErrorSummaryAsync([errorMessage]);
        await _signInPage.VerifyEmailErrorAsync(errorMessage);
    }

    public async Task VerifySignInPageIsLoadedAsync()
    {
        await _signInPage.WaitForPageToLoadAsync();
    }

    public async Task EnterCredentilasAsync(string email, string password)
    {
        await EnterEmailAsync(email);
        await EnterPasswordAsync(password);
    }
    public async Task ClickOnSignInAsync() => await _signInPage.SubmitAsync();

    public async Task VerifyPasswordIsVisibleAsync()
    {
        await _signInPage.VerifyPasswordIsVisibleAsync();
    }

    public async Task EnterEmailAsync(string email)
    {
        await _signInPage.EnterEmailAsync(email);
    }

    public async Task EnterPasswordAsync(string password)
    {
        await _signInPage.EnterPasswordAsync(password);
    }

    public enum FieldErrorType
    {
        Email,
        Password,
        Summary
    }
}



