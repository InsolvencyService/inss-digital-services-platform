using GovUk.Forms.HostApp.UI.Test.Coordinators;

namespace GovUk.Forms.HostApp.UI.Test.Steps;

[Binding]
public class StartPageSteps
{
    private readonly StartPageCoordinator _homePageCoordinator;
    private readonly SignInCoordinator _signInCoordinator;
    public StartPageSteps(StartPageCoordinator homePageCoordinator, SignInCoordinator signInCoordinator)
    {
        _homePageCoordinator = homePageCoordinator;
        _signInCoordinator = signInCoordinator;
    }
    [Given("the user is on the IPUS start page")]
    public async Task GivenTheUserIsOnTheIPUSStartPage()
    {
        await _homePageCoordinator.VerifyStartPageIsDisplayedAsync();
    }

    [When("the user chooses to provide feedback")]
    public async Task WhenTheUserChoosesToProvideFeedback()
    {
        await _homePageCoordinator.NavigateToFeedbackPageAsync();
    }
    [When("the user chooses to start the application")]
    public async Task WhenTheUserChoosesToStartTheApplication()
    {
        await _homePageCoordinator.NavigateToLoginPageAsync();
    }

    [Then("a new browser tab should be opened")]
    public async Task ThenANewBrowserTabShouldBeOpened()
    {
        await _homePageCoordinator.VerifyNewBrowserTabIsOpenedAsync();
    }

    [Then("the Director Conduct Reporting Service customer feedback page should be displayed")]
    public async Task ThenTheDirectorConductReportingServiceCustomerFeedbackPageShouldBeDisplayed()
    {
        await _homePageCoordinator.VerifyDirectorConductReportingServicePageIsDisplayedAsync();
    }

    [Then("the user is redirected to the sign-in page")]
    public async Task ThenTheUserIsRedirectedToTheSignInPage()
    {
        await _signInCoordinator.VerifyThatSignInPageIsDisplayedAsync();
    }

}
