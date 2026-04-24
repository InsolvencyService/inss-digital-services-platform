using GovUk.Forms.HostApp.UI.Test.Coordinators;
using GovUk.Forms.HostApp.UI.Test.Support;

namespace GovUk.Forms.HostApp.UI.Test.Steps;

[Scope(Feature = "Start Page Navigation")]
[Binding]
public class StartPageSteps
{
    private readonly StartPageCoordinator _startPageCoordinator;
    private readonly SignInCoordinator _signInCoordinator;
    public StartPageSteps(
        StartPageCoordinator homePageCoordinator,
        SignInCoordinator signInCoordinator)
    {
        _startPageCoordinator = homePageCoordinator;
        _signInCoordinator = signInCoordinator;
    }
    [Given("the user is on the IPUS start page")]
    public async Task GivenTheUserIsOnTheIPUSStartPage()
    {
        await _startPageCoordinator.VerifyStartPageIsDisplayedAsync();
    }

    [When("the user chooses to provide feedback")]
    public async Task WhenTheUserChoosesToProvideFeedback()
    {
        await _startPageCoordinator.NavigateToFeedbackPageAsync();
    }
    [When("the user chooses to start the application")]
    public async Task WhenTheUserChoosesToStartTheApplication()
    {
        await _startPageCoordinator.NavigateToLoginPageAsync();
    }

    [When("the use chooses to open {string} in the footer")]
    public async Task WhenTheUseChoosesToOpenInTheFooter(string linkText)
    {
        await _startPageCoordinator.ClickOnFooterLinkAsync(linkText);
    }

    [Then("a new browser tab should be opened")]
    public async Task ThenANewBrowserTabShouldBeOpened()
    {
        await _startPageCoordinator.VerifyNewBrowserTabIsOpenedAsync();
    }

    [Then("the Director Conduct Reporting Service customer feedback page should be displayed")]
    public async Task ThenTheDirectorConductReportingServiceCustomerFeedbackPageShouldBeDisplayed()
    {
        await _startPageCoordinator.VerifyDirectorConductReportingServicePageIsDisplayedAsync();
    }

    [Then("the user is redirected to the sign-in page")]
    public async Task ThenTheUserIsRedirectedToTheSignInPage()
    {
        await _signInCoordinator.VerifyThatSignInPageIsDisplayedAsync();
    }

    [Then("the start page should match the visual snapshot")]
    public async Task ThenTheStartPageShouldMatchTheVisualSnapshot()
    {
        string screenshotPath = await _startPageCoordinator.CaptureStartPageVisualAsync();

        await VerifyFile(screenshotPath)
        .UseDirectory(ScenarioConstant.SnapShots)
        .UseFileName(ScenarioConstant.StartPage); ;
    }

    [Then("a new page should open with title {string} with url containing {string}")]
    public async Task ThenANewPageShouldOpenWithTitleWithUrlContaining(string title, string url)
    {
        await _startPageCoordinator.VerifyFooterLinkNavigationAsync(title, url);
    }



}
