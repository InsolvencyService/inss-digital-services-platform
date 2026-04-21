using GovUk.Forms.HostApp.UI.Test.Pages.Common;
using GovUk.Forms.HostApp.UI.Test.Pages.Login;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators;

public class StartPageCoordinator(
    IStartPage startPage,
    ISignInPage signInPage,
    ScenarioContext scenarioContext,
    DirectorConductReportingServicePage conductReportingServicePage) : BaseCoordinator
{

    public async Task VerifyStartPageIsDisplayedAsync()
    {
        await startPage.VerifyStartPageIsDisplayedAsync();
    }

    public async Task NavigateToLoginPageAsync()
    {
        await startPage.ClickOnStartNowAsync();
        await signInPage.VerifySignInPageIsDisplayedAsync();
    }

    public async Task NavigateToFeedbackPageAsync()
    {
        IPage feedbackPage = await NavigateAsync(startPage.ClickOnFeedbackAsync);
        scenarioContext.Set(feedbackPage);
    }

    public async Task VerifyDirectorConductReportingServicePageIsDisplayedAsync()
    {
        IPage feedbackPage = scenarioContext.Get<IPage>();
        conductReportingServicePage.SetPage(feedbackPage);
        await conductReportingServicePage.VerifyThatDirectorConductReportingServicePageIsDisplayedAsync();
    }

    public async Task VerifyNewBrowserTabIsOpenedAsync()
    {
        IPage feedbackPage = scenarioContext.Get<IPage>();
        string originalPageUrl = await startPage.GetHeadingTextAsync();
        string newPageUrl = feedbackPage.Url;
        Assert.That(newPageUrl, Is.Not.EqualTo(originalPageUrl), "A new browser tab was not opened.");
    }

}
