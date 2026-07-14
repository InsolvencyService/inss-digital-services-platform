using GovUk.Forms.HostApp.UI.Test.Config.Driver;

namespace GovUk.Forms.HostApp.UI.Test.Pages.Navigation;

public class PageNotFoundPage : BasePage, IPageNotFoundPage
{
    private readonly IPlaywrightDriver _playwrightDriver;

    public PageNotFoundPage(IPlaywrightDriver playwrightDriver)
    {
        _playwrightDriver = playwrightDriver;
    }

    private new IPage Page => _playwrightDriver.Page;

    private ILocator PageHeading => Page.GetByRole(AriaRole.Heading, new() { Name = "Page not found" });
    private ILocator MainContent => Page.Locator("#main-content");

    protected override async Task PageContentLoadedAsync()
    {
        await Expect(PageHeading).ToBeVisibleAsync();

        await Expect(MainContent).ToMatchAriaSnapshotAsync("""
            - heading "Page not found" [level=1]
            - paragraph: If you typed the web address, check it is correct.
            - paragraph: If you pasted the web address, check you copied the entire address.
            """);
    }
}
