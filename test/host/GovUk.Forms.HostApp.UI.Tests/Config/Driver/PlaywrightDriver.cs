using GovUk.Forms.HostApp.UI.Tests.Models.Settings;

namespace GovUk.Forms.HostApp.UI.Tests.Config.Driver;

public class PlaywrightDriver : IPlaywrightDriver
{
    private IPlaywright? _playwright;
    private IPage _page;
    private IBrowser? _browser;
    private IBrowserContext? _context;


    public IBrowser Browser =>
        _browser ?? throw new InvalidOperationException("PlaywrightDriver is not initialised.");
    public IBrowserContext Context =>
        _context ?? throw new InvalidOperationException("PlaywrightDriver is not initialised.");
    public IPage Page =>
        _page ?? throw new InvalidOperationException("PlaywrightDriver is not initialised.");


    public async Task InitialiseAsync()
    {
        TestSettings config = TestConfigReader.Settings;

        _playwright = await Playwright.CreateAsync();

        _browser = await CreateBrowserAsync(
            _playwright,
            config.BrowserSettings,
           new BrowserTypeLaunchOptions
           {
               Headless = config.BrowserSettings.Headless,
               SlowMo = config.BrowserSettings.SlowMo,
           });

        _context = await _browser.NewContextAsync();
        await StartTracingSafeAsync();
        _page = await Context.NewPageAsync();
        AttachConsoleLogging();
    }

    private static async Task<IBrowser> CreateBrowserAsync(
        IPlaywright playwright,
        BrowserSettings settings,
        BrowserTypeLaunchOptions options)
    {
        string browserName = settings?.BrowserName?.ToLowerInvariant()!;

        return await playwright[browserName].LaunchAsync(options);

    }

    private async Task StartTracingSafeAsync()
    {
        if (_context == null)
        {
            return;
        }

        await _context.Tracing.StartAsync(new()
        {
            Title = TestContext.CurrentContext.Test.Name,
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });
    }

    private void AttachConsoleLogging()
    {
        _page.Console += (_, msg) =>
        {
            Console.WriteLine($"[Browser Console] {msg.Type}: {msg.Text}");
        };
    }
}


