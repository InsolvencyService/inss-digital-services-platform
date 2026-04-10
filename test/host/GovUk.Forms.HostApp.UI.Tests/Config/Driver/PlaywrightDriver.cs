using GovUk.Forms.HostApp.UI.Tests.Helpers;
using GovUk.Forms.HostApp.UI.Tests.Models.Settings;

namespace GovUk.Forms.HostApp.UI.Tests.Config.Driver;

public class PlaywrightDriver : IPlaywrightDriver
{
    private IPlaywright? _playwright;
    private IPage _page;
    private IBrowser? _browser;
    private IBrowserContext? _context;
    private readonly TestArtifacts _artifacts;

    private readonly List<IPage> _pages = [];

    public PlaywrightDriver(TestArtifacts artifacts)
    {
        _artifacts = artifacts ?? throw new ArgumentNullException(nameof(artifacts));
    }
    public IBrowser Browser =>
        _browser ?? throw new InvalidOperationException("PlaywrightDriver is not initialised.");
    public IBrowserContext Context =>
        _context ?? throw new InvalidOperationException("PlaywrightDriver is not initialised.");
    public IPage Page =>
        _page ?? throw new InvalidOperationException("PlaywrightDriver is not initialised.");

    public IReadOnlyList<IPage> Pages => _pages.AsReadOnly();
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

        _context = await _browser.NewContextAsync(new BrowserNewContextOptions
        {
            RecordVideoDir = _artifacts.VideoDirectory,
            RecordVideoSize = new RecordVideoSize
            {
                Width = 1280,
                Height = 720
            }
        });
        await StartTracingSafeAsync();
        _page = await Context.NewPageAsync();
        AttachConsoleLogging();
        RegisterPage(_page);
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

    public void SetActivePage(IPage page)
    {
        ArgumentNullException.ThrowIfNull(page);

        _page = page;
        RegisterPage(page);
    }

    private void RegisterPage(IPage page)
    {
        if (_pages.Contains(page))
        {
            return;
        }

        _pages.Add(page);

        page.Close += (_, _) =>
        {
            _pages.Remove(page);

            if (_page == page)
            {
                _page = _pages.Last();
            }
        };
    }

    public async Task<string> TakeScreenshotAsync(string name)
    {
        if (_page == null)
        {
            throw new InvalidOperationException("Page is not initialised.");
        }

        string path = _artifacts.GetScreenshotPath(name);

        await _page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = path,
            FullPage = true
        });

        TestContext.AddTestAttachment(path, $"Screenshot: {name}");
        return path;
    }
}


