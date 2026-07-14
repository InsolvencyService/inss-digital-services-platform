
using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Pages.Common;
using GovUk.Forms.HostApp.UI.Test.Support;

namespace GovUk.Forms.HostApp.UI.Test.Pages.Login;

public class StartPage : BasePage, IStartPage
{
    private readonly IPlaywrightDriver _playwrightDriver;
    private readonly ICommonPage _commonPage;
    private readonly ScenarioContext _scenarioContext;

    public StartPage(IPlaywrightDriver playwrightDriver,
        ICommonPage commonPage,
        ScenarioContext scenarioContext)
    {
        _playwrightDriver = playwrightDriver;
        _commonPage = commonPage;
        _scenarioContext = scenarioContext;
    }

    protected new IPage Page => _playwrightDriver.Page;
    private ILocator Heading => Page.GetByRole(AriaRole.Heading, new() { Name = StartPageLocators.Labels.Heading });
    private ILocator BeforeYouStartText => Page.GetByRole(AriaRole.Heading, new() { Name = StartPageLocators.Labels.BeforeYouStartText });
    private ILocator StartNowButton => Page.GetByRole(AriaRole.Button, new() { Name = StartPageLocators.Labels.StartNowButton });
    private ILocator FeedbackLink => Page.GetByRole(AriaRole.Link, new() { Name = StartPageLocators.Labels.FeedbackLink });
    private ILocator GetLink(string linkText) => Page.GetByRole(AriaRole.Link, new() { Name = linkText });
    private ILocator UploadRedundancyPaymentFormsLink => Page.GetByRole(AriaRole.Link, new() { Name = StartPageLocators.Labels.UploadRedundancyPaymentFormsLink });
    private ILocator OnceLoggedInText => Page.GetByText(StartPageLocators.Labels.OnceLoggedInText, new() { Exact = true });
    private ILocator GOVUKLink => Page.GetByRole(AriaRole.Img, new() { Name = SignInLocators.Labels.GOVUKLink });
    private ILocator MainContent => Page.Locator(StartPageLocators.Selectors.MainContent);

    public async Task ClickOnStartNowAsync()
    {
        await PageContentLoadedAsync();
        await StartNowButton.ClickAsync();
    }

    public async Task<IPage> ClickOnFeedbackAsync()
    {
        await PageContentLoadedAsync();

        IPage page = await _commonPage.OpenNewTabAndVerifyAsync(
            Page.Context,
            () => FeedbackLink.ClickAsync(),
            expectedUrlPart: PartialPageUris.DirectorConductReportingServicePage);
        return page;
    }

    public async Task<string> GetHeadingTextAsync()
    {
        await PageContentLoadedAsync();
        return await Heading.InnerTextAsync();
    }

    protected override async Task PageContentLoadedAsync()
    {
        await Expect(GOVUKLink).ToBeVisibleAsync();
        await Expect(Heading).ToBeVisibleAsync();
        await Expect(StartNowButton).ToBeVisibleAsync();
        await Expect(BeforeYouStartText).ToBeVisibleAsync();
        await Expect(FeedbackLink).ToBeVisibleAsync();
        await Expect(UploadRedundancyPaymentFormsLink).ToBeVisibleAsync();
        await Expect(OnceLoggedInText).ToBeVisibleAsync();
    }
    public async Task ClickOnFooterLinkAsync(string linkText)
    {
        _scenarioContext.Set(Page);
        await GetLink(linkText).ClickAsync();
    }

    public async Task VerifyStartPageAriaSnapshotAsync()
    {
        await WaitForPageToLoadAsync();

        await Expect(MainContent)
            .ToMatchAriaSnapshotAsync("""
             - heading "Upload redundancy payment forms (RP14/A)" [level=1]
             - paragraph: Access to this service is restricted to Insolvency Practitioners, authorised Agents acting on their behalf, Service Managers and Staff.
             - heading "Before you start" [level=2]
             - paragraph: Make sure your email address is registered with the Insolvency Service before using this platform.
             - button "Start now"
             """);
    }


}
