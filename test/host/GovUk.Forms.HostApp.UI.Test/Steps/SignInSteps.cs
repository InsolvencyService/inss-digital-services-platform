using GovUk.Forms.HostApp.UI.Test.Coordinators;
using GovUk.Forms.HostApp.UI.Test.Support;

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

    [When("I sign in with valid credential {string} {string}")]
    public async Task WhenISignInWithValidCredential(string emailAddress, string password)
    {
        await _signInCoordinator.SignInToServiceAsync(emailAddress, password);
    }

    [When("I choose to view my password")]
    public async Task WhenIChooseToViewMyPassword()
    {
        await _signInCoordinator.ShowPasswordAsync(ScenarioConstant.EmailAddress, ScenarioConstant.Password);
    }


    [Then("I should be on the declaration page")]
    public async Task ThenIShouldBeOnTheDeclarationPage()
    {
        await _declarationCoordinator.VerifyDeclarationPageIsDisplayedAsync();
    }

    [Then("I should be able to see the password I entered")]
    public async Task ThenIShouldBeAbleToSeeThePasswordIEntered()
    {
        await _signInCoordinator.VerifyPasswordIsVisibleAsync();
    }



}
