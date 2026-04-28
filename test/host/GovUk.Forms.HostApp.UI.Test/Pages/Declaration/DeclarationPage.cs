using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Pages.Common;
using GovUk.Forms.HostApp.UI.Test.Support;

namespace GovUk.Forms.HostApp.UI.Test.Pages.Declaration;

public class DeclarationPage : BasePage, IDeclarationPage
{
    private readonly IPlaywrightDriver _playwrightDriver;
    private readonly ICommonPage _commonPage;

    public DeclarationPage(IPlaywrightDriver playwrightDriver, ICommonPage commonPage)
    {
        _playwrightDriver = playwrightDriver;
        _commonPage = commonPage;
    }

    protected new IPage Page => _playwrightDriver.Page;

    private ILocator DeclarationTitle => Page.GetByRole(AriaRole.Link, new() { Name = DeclarationLocators.Labels.DeclarationTitle });
    private ILocator Section187Link => Page.GetByRole(AriaRole.Link, new() { Name = DeclarationLocators.Labels.Section187Link });
    private ILocator BackButton => Page.GetByRole(AriaRole.Link, new() { Name = SharedLoactors.BackButton, Exact = true });
    private ILocator AgreeAndContinueButton => Page.GetByRole(AriaRole.Button, new() { Name = DeclarationLocators.Labels.AgreeAndContinueButton });
    protected override async Task PageContentLoadedAsync()
    {
        await Page.WaitForLoadStateAsync(LoadState.Load);
        await Expect(DeclarationTitle).ToBeVisibleAsync();
        await Expect(DeclarationTitle).ToHaveTextAsync(DeclarationLocators.Labels.DeclarationTitle);
        await Expect(Section187Link).ToBeVisibleAsync();
        await Expect(Section187Link).ToBeEnabledAsync();
        await Expect(AgreeAndContinueButton).ToBeVisibleAsync();
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

    public async Task ClickOnBackButtonAsync()
    {
        await PageContentLoadedAsync();
        await BackButton.ClickAsync();
    }

    public async Task ClickOnAgreeAndContinueButtonAsync()
    {
        await PageContentLoadedAsync();
        await AgreeAndContinueButton.ClickAsync();
    }
}
