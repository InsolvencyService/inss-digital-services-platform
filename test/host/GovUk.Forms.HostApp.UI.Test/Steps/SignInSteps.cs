using GovUk.Forms.HostApp.UI.Test.Coordinators;
using GovUk.Forms.HostApp.UI.Test.Support;
using static GovUk.Forms.HostApp.UI.Test.Models.TestData;

namespace GovUk.Forms.HostApp.UI.Test.Steps;

[Binding]
public sealed class SignInSteps
{
    private readonly SignInCoordinator _signInCoordinator;
    private readonly DeclarationCoordinator _declarationCoordinator;

    public SignInSteps(SignInCoordinator signInCoordinator,
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

    [When("I provide valid sign in details")]
    public async Task WhenIProvideValidSignInDetails()
    {
        await _signInCoordinator.SignInToServiceAsync(ScenarioConstant.EmailAddress, ScenarioConstant.Password);
    }

    [When("I choose to view my password")]
    public async Task WhenIChooseToViewMyPassword()
    {
        await _signInCoordinator.ShowPasswordAsync(ScenarioConstant.EmailAddress, ScenarioConstant.Password);
    }

    [When("I submit the sign in form")]
    public async Task WhenISubmitTheSignInForm()
    {
        await _signInCoordinator.ClickSignInButtonAsync();
    }

    [When("I enter {string} into the email address field")]
    public async Task WhenIEnterIntoTheEmailAddressField(string email)
    {
        await _signInCoordinator.EnterEmailAsync(email);
    }

    [When("I enter {string} into the password field")]
    public async Task WhenIEnterIntoThePasswordField(string password)
    {
        await _signInCoordinator.EnterPasswordAsync(password);
    }

    [When("I choose to sign in")]
    public async Task WhenIChooseToSignIn()
    {
        await _signInCoordinator.ClickSignInButtonAsync();
    }


    [Then("I should be on to view the declaration page")]
    public async Task ThenIShouldBeOnTheDeclarationPage()
    {
        await _declarationCoordinator.VerifyDeclarationPageIsDisplayedAsync();
    }

    [Then("I should be able to see the password I entered")]
    public async Task ThenIShouldBeAbleToSeeThePasswordIEntered()
    {
        await _signInCoordinator.VerifyPasswordIsVisibleAsync();
    }

    //[Then("I should see the error message {string}")]
    //public async Task ThenIShouldSeeTheErrorMessage(string errorMessage)
    //{
    //    throw new PendingStepException();
    //}

    [Then("I should see the following error messages:")]
    public async Task ThenIShouldSeeTheFollowingErrorMessages(DataTable table)
    {
        List<string> errors = table.CreateSet<Errors>()
                    .Select(e => e.Message)
                    .ToList();

        await _signInCoordinator.VerifyErrorMessagesAsync(errors);

    }




}
