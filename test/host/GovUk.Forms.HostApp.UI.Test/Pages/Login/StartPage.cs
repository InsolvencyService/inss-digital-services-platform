
using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Pages.Common;
using GovUk.Forms.HostApp.UI.Test.Support;

namespace GovUk.Forms.HostApp.UI.Test.Pages.Login;

public class StartPage : BasePage, IStartPage
{
    private readonly IPlaywrightDriver _playwrightDriver;
    private readonly ICommonPage _commonPage;

    public StartPage(IPlaywrightDriver playwrightDriver, ICommonPage commonPage)
    {
        _playwrightDriver = playwrightDriver;
        _commonPage = commonPage;
    }

    private new IPage Page => _playwrightDriver.Page;
    private ILocator Heading => Page.GetByRole(AriaRole.Heading, new() { Name = StartPageLocators.Labels.Heading });
    private ILocator BeforeYouStartText => Page.GetByRole(AriaRole.Heading, new() { Name = StartPageLocators.Labels.BeforeYouStartText });
    private ILocator StartNowButton => Page.GetByRole(AriaRole.Button, new() { Name = StartPageLocators.Labels.StartNowButton });
    private ILocator FeedbackLink => Page.GetByRole(AriaRole.Link, new() { Name = StartPageLocators.Labels.FeedbackLink });
    private ILocator UploadRedundancyPaymentFormsLink => Page.GetByRole(AriaRole.Link, new() { Name = StartPageLocators.Labels.UploadRedundancyPaymentFormsLink });
    private ILocator OnceLoggedInText => Page.GetByText(StartPageLocators.Labels.OnceLoggedInText, new() { Exact = true });
    private ILocator GOVUKLink => Page.GetByRole(AriaRole.Img, new() { Name = SignInLocators.Labels.GOVUKLink });

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

    public async Task VerifyStartPageIsDisplayedAsync()
    {
        await PageContentLoadedAsync();
    }
}
