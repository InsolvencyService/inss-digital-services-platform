namespace GovUk.Forms.HostApp.UI.Test.Pages.Login;

public class SignInLocators
{
    public const string password = nameof(password);
    public const string text = nameof(text);
    public const string Beta = nameof(Beta);
    public static class Labels
    {
        public const string Heading = "Sign in";
        public const string EmailAddress = "Email address";
        public const string PasswordInput = "Password";
        public const string ShowPasswordButton = "Show";
        public const string SignInButton = "Sign in";
        public const string ForgotPasswordLink = "Forgot password";
        public const string BackLink = "Back";
        public const string GOVUKLink = "GOV.UK";
        public const string UploadRedundancyPaymentForms = "Upload redundancy payment forms (RP14/A)";
    }

    public class Selectors
    {
        public const string ErrorSummary = ".govuk-error-summary";
        public const string EmailError = "#Email_Value-error";
        public const string PasswordError = "#Password-error";
    }
}
