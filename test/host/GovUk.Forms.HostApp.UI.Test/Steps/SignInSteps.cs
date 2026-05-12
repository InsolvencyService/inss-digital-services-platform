using GovUk.Forms.HostApp.UI.Test.Coordinators;
using GovUk.Forms.HostApp.UI.Test.Support;
using static GovUk.Forms.HostApp.UI.Test.Models.TestData;

namespace GovUk.Forms.HostApp.UI.Test.Steps;

[Scope(Feature = "SignIn")]
[Binding]

public sealed class SignInSteps
{
    private const string EmptyValuePlaceholder = "<empty>";
    private const string WhitespaceValuePlaceholder = "<whitespace>";

    private readonly SignInCoordinator _signInCoordinator;
    private readonly DeclarationCoordinator _declarationCoordinator;

    public SignInSteps(
     SignInCoordinator signInCoordinator,
     DeclarationCoordinator declarationCoordinator)
    {
        _signInCoordinator = signInCoordinator;
        _declarationCoordinator = declarationCoordinator;
    }


    [Given("I am on the sign in page")]
    public async Task GivenIAmOnTheSignInPage()
    {
        await _signInCoordinator.NavigateToSignInPageAsync();
    }

    [Given("I provide valid credentials")]
    public async Task GivenIProvideValidCredentials()
    {
        const string validEmail = ScenarioConstant.EmailAddress;
        const string validPassword = ScenarioConstant.Password;

        await _signInCoordinator.EnterCredentilasAsync(validEmail, validPassword);
    }

    [When("I provide valid sign in details")]
    public async Task WhenIProvideValidSignInDetails()
    {
        const string validEmail = ScenarioConstant.EmailAddress;
        const string validPassword = ScenarioConstant.Password;

        await _signInCoordinator.SignInAsync(validEmail, validPassword);
    }


    [When("I choose to view my password")]
    public async Task WhenIChooseToViewMyPassword()
    {
        await _signInCoordinator.TogglePasswordVisibilityAsync();
    }


    [When("I choose to sign in")]
    public async Task WhenIChooseToSignIn()
    {
        await _signInCoordinator.ClickOnSignInAsync();
    }

    [When("I enter {string} into the email address field")]
    public async Task WhenIEnterIntoTheEmailAddressField(string email)
    {
        string resolvedEmail = ResolveSpecialValues(email);
        await _signInCoordinator.EnterEmailAsync(resolvedEmail);
    }

    [When("I enter {string} into the password field")]
    public async Task WhenIEnterIntoThePasswordField(string password)
    {
        string resolvedPassword = ResolveSpecialValues(password);
        await _signInCoordinator.EnterPasswordAsync(resolvedPassword);
    }

    [When("I submit the sign in form")]
    public async Task WhenISubmitTheSignInForm()
    {
        await _signInCoordinator.ClickOnSignInAsync();
    }

    [Then("I should be able to view the declaration page")]
    public async Task ThenIShouldBeAbleToViewTheDeclarationPage()
    {
        await _declarationCoordinator.VerifyDeclarationPageIsDisplayedAsync();
    }

    [Then("I should be able to see the password I entered")]
    public async Task ThenIShouldBeAbleToSeeThePasswordIEntered()
    {
        await _signInCoordinator.VerifyPasswordIsVisibleAsync();
    }


    [Then("I should see the following error messages:")]
    public async Task ThenIShouldSeeTheFollowingErrorMessages(DataTable table)
    {

        try
        {
            List<string> errors = table.CreateSet<Errors>()
                .Select(e => e.Message)
                .Where(m => !string.IsNullOrWhiteSpace(m))
                .ToList();

            if (errors.Count == 0)
            {
                throw new ArgumentException(
                    "DataTable must contain at least one error message.",
                    nameof(table));
            }

            await _signInCoordinator.VerifyErrorSummaryAsync(errors);
        }
        catch (ArgumentException ex) when (!ex.ParamName?.Equals(nameof(table), StringComparison.OrdinalIgnoreCase) ?? false)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "Failed to parse error messages from DataTable. " +
                "Expected format: | Message | with at least one row.", ex);
        }
    }


    [Then(@"I should see ""(.*)"" for the ""(.*)"" field")]
    public async Task ThenIShouldSeeErrorForField(string errorMessage, string field)
    {
        if (!Enum.TryParse(
            field,
            ignoreCase: true,
            out SignInCoordinator.FieldErrorType fieldType))
        {
            string validFields = string.Join(", ", Enum.GetNames<SignInCoordinator.FieldErrorType>());
            throw new ArgumentException(
                $"Invalid field '{field}'. Valid values: {validFields}",
                nameof(field));
        }

        await _signInCoordinator.VerifyFieldErrorAsync(fieldType, errorMessage);
    }

    [Then("I should see the error message {string}")]
    public async Task ThenIShouldSeeTheErrorMessage(string errorMessage)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(errorMessage);

        await _signInCoordinator.VerifyAccountIsBlockedAsync(errorMessage);
    }

    [Then("I should be on to view the declaration page")]
    public async Task ThenIShouldBeOnToViewTheDeclarationPage()
    {
        await _declarationCoordinator.VerifyDeclarationPageIsDisplayedAsync();
    }


    private static string ResolveSpecialValues(string value)
    {
        return value switch
        {
            EmptyValuePlaceholder => string.Empty,
            WhitespaceValuePlaceholder => " ",
            _ => value
        };
    }
}

