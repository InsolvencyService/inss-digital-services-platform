using Microsoft.Playwright;
using Xunit;

// ReSharper disable ReplaceWithPrimaryConstructorParameter

namespace GovUk.Forms.HostApp.PageTest;

[Collection(WebApplicationFactoryFixture.Name)]
public abstract class PageTestBase(TestWebApplicationFactory factory) : Microsoft.Playwright.Xunit.PageTest
{
    private readonly TestWebApplicationFactory _factory = factory;
    
    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions
        {
            ColorScheme = ColorScheme.Light,
            ViewportSize = new ViewportSize { Width = 1920, Height = 1080 },
            BaseURL = _factory.ServerAddress
        };
    }
    
    protected async Task GotToPage(string url)
    {
        IResponse? response = await Page.GotoAsync(url);
        
        Assert.NotNull(response);
        Assert.True(response.Ok, $"Navigating to requested page {url} failed.");
    }

    protected async Task ExpectLinkToExist(string title)
    {
        ILocator link = Page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = title });
        await Expect(link).ToBeVisibleAsync();
    }
    
    protected async Task ExpectQuestion(string text)
    {
        ILocator question = Page.GetByText(text);
        await Expect(question).ToBeVisibleAsync();
    }
    
    protected async Task ExpectHeading(string text)
    {
        ILocator question = Page.GetByText(text);
        await Expect(question).ToBeVisibleAsync();
    }
    
    protected async Task ExpectHint(string text)
    {
        ILocator hint = Page.GetByText(text);
        await Expect(hint).ToBeVisibleAsync();
    }

    protected async Task ExpectErrorHeading(string text)
    {
        ILocator question = Page.GetByText(text);
        await Expect(question).ToBeVisibleAsync();
    }
    
    protected async Task ClickLink(string title)
    {
        ILocator link = Page.GetByRole(AriaRole.Link, new PageGetByRoleOptions { Name = title });
        await Expect(link).ToBeVisibleAsync();
        await link.ClickAsync();
    }
    
    protected async Task ClickButton(string text)
    {
        ILocator button = Page.GetByRole(AriaRole.Button, new PageGetByRoleOptions { Name = text });
        await button.ClickAsync();
    }
}
