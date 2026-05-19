using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Pages.Common;
using GovUk.Forms.HostApp.UI.Test.Pages.Login;
using GovUk.Forms.HostApp.UI.Test.Support;
using System.Text.RegularExpressions;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators;

public class StartPageCoordinator(
    IStartPage startPage,
    ISignInPage signInPage,
    ICommonPage commonPage,
    IPlaywrightDriver playwrightDriver,
    ScenarioContext scenarioContext,
    TestArtifacts testArtifacts,
    DirectorConductReportingServicePage conductReportingServicePage) : BaseCoordinator(testArtifacts)
{

    public async Task VerifyStartPageIsDisplayedAsync()
    {
        await startPage.WaitForPageToLoadAsync();
    }

    public async Task NavigateToLoginPageAsync()
    {
        await startPage.ClickOnStartNowAsync();
        await signInPage.WaitForPageToLoadAsync();
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

    public async Task<string> CaptureStartPageVisualAsync()
    {
        return await CapturePageVisualAsync(
            () => commonPage.CaptureVisualAsync(playwrightDriver.Page),
            ScenarioConstant.StartPage);
    }

    public async Task ClickOnFooterLinkAsync(string linkText)
    {
        await VerifyStartPageIsDisplayedAsync();
        await startPage.ClickOnFooterLinkAsync(linkText);
    }

    public async Task VerifyFooterLinkNavigationAsync(string expectedTitle, string expectedUrlPart)
    {
        IPage currentPage = scenarioContext.Get<IPage>();
        await Expect(currentPage).ToHaveURLAsync(new Regex(expectedUrlPart));
        await Expect(currentPage).ToHaveTitleAsync(expectedTitle);

        IResponse response = await commonPage.PageGoBackAsync(currentPage, new PageGoBackOptions { WaitUntil = WaitUntilState.DOMContentLoaded });
        Assert.That(response.Ok, Is.True, "Navigation back to the Start Page was not successful.");

    }

}
