using Microsoft.Playwright.NUnit;

namespace GovUk.Forms.HostApp.UI.Test;

public class DemoTest : PageTest
{

    [Test]
    public async Task ShouldNavigateToHomePage()
    {
        IBrowser browser = await BrowserType.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false
        });

        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await page.GotoAsync("https://localhost:5056/");
        var title = await page.TitleAsync();

        var locator = await page.PickLocatorAsync();

        Console.WriteLine($"Picked element: {locator}");
        Assert.That(title, Is.EqualTo(""));

        await page.CancelPickLocatorAsync();
    }
}
