namespace GovUk.Forms.HostApp.UI.Tests.Pages;

public class StartPage : BasePage, IStartPage
{
    private readonly IPage _page;

    public StartPage(IPage page)
    {
        _page = page;
    }

    private ILocator StartButton => _page.Locator("#start-button");
    private ILocator PageTitle => _page.GetByRole(AriaRole.Heading, new() { Name = "" });

    public async Task ClickOnStartButtonAsync()
    {
        await StartButton.ClickAsync();
    }

    protected override async Task PageContentLoadedAsync()
    {
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        await Expect(PageTitle).ToBeVisibleAsync();
    }
}
