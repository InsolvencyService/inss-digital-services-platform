using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Pages.Common;
using GovUk.Forms.HostApp.UI.Test.Pages.Locators;
using GovUk.Forms.HostApp.UI.Test.Support;

namespace GovUk.Forms.HostApp.UI.Test.Pages;

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
        return page;
    }
}
