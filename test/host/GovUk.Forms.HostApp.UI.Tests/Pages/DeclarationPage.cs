using GovUk.Forms.HostApp.UI.Tests.Config.Driver;
using GovUk.Forms.HostApp.UI.Tests.Pages.Common;
using GovUk.Forms.HostApp.UI.Tests.Pages.Locators;
using GovUk.Forms.HostApp.UI.Tests.Support;

namespace GovUk.Forms.HostApp.UI.Tests.Pages;

public class DeclarationPage : BasePage, IDeclarationPage
{
    private readonly IPage _page;
    private readonly ICommonPage _commonPage;

    public DeclarationPage(IPlaywrightDriver playwrightDriver, ICommonPage commonPage)
    {
        _page = playwrightDriver.Page;
        _commonPage = commonPage;
    }


    private ILocator DeclarationTitle =>
        _page.GetByRole(AriaRole.Heading, new() { Name = DeclarationLocators.Labels.DeclarationTitle });
    private ILocator Section187Link =>
        _page.GetByRole(AriaRole.Link, new() { Name = DeclarationLocators.Labels.Section187Link });

    protected override async Task PageContentLoadedAsync()
    {
        await _page.WaitForLoadStateAsync(LoadState.Load);
        await Expect(DeclarationTitle).ToBeVisibleAsync();
        await Expect(DeclarationTitle).ToHaveTextAsync(DeclarationLocators.Labels.DeclarationTitle);
        await Expect(Section187Link).ToBeVisibleAsync();
        await Expect(Section187Link).ToBeEnabledAsync();
    }


    public async Task<IPage> ClickOnSection187LinkAsync()
    {
        await PageContentLoadedAsync();

        return await _commonPage.OpenNewTabAndVerifyAsync(
              _page.Context,
              () => Section187Link.ClickAsync(),
              expectedUrlPart: PartialPageUris.Section187Page);
    }
}
