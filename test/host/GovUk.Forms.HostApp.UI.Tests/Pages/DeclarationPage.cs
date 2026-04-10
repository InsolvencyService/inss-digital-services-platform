using GovUk.Forms.HostApp.UI.Tests.Config.Driver;
using GovUk.Forms.HostApp.UI.Tests.Pages.Common;
using GovUk.Forms.HostApp.UI.Tests.Pages.Locators;
using GovUk.Forms.HostApp.UI.Tests.Support;

namespace GovUk.Forms.HostApp.UI.Tests.Pages;

public class DeclarationPage : BasePage, IDeclarationPage
{
    private readonly IPlaywrightDriver _playwrightDriver;
    private readonly ICommonPage _commonPage;

    public DeclarationPage(IPlaywrightDriver playwrightDriver, ICommonPage commonPage)
    {
        _playwrightDriver = playwrightDriver;
        _commonPage = commonPage;
    }

    private new IPage Page => _playwrightDriver.Page;

    private ILocator DeclarationTitle =>
        Page.GetByRole(AriaRole.Heading, new() { Name = DeclarationLocators.Labels.DeclarationTitle });

    private ILocator Section187Link =>
        Page.GetByRole(AriaRole.Link, new() { Name = DeclarationLocators.Labels.Section187Link });

    protected override async Task PageContentLoadedAsync()
    {
        await Page.WaitForLoadStateAsync(LoadState.Load);
        await Expect(DeclarationTitle).ToBeVisibleAsync();
        await Expect(DeclarationTitle).ToHaveTextAsync(DeclarationLocators.Labels.DeclarationTitle);
        await Expect(Section187Link).ToBeVisibleAsync();
        await Expect(Section187Link).ToBeEnabledAsync();
    }

    public async Task<IPage> ClickOnSection187LinkAsync()
    {
        await PageContentLoadedAsync();

        IPage page = await _commonPage.OpenNewTabAndVerifyAsync(
            Page.Context,
            () => Section187Link.ClickAsync(),
            expectedUrlPart: PartialPageUris.Section187Page);
        _playwrightDriver.SetActivePage(page);
        return page;
    }

    public void SwitchBackTo(IPage page)
    {
        _playwrightDriver.SetActivePage(page);
    }
}
