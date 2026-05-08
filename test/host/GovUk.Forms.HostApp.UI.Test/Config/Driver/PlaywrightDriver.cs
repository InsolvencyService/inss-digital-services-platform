using GovUk.Forms.HostApp.UI.Test.Models.Settings;

namespace GovUk.Forms.HostApp.UI.Test.Config.Driver;

public sealed class PlaywrightDriver : IPlaywrightDriver
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IBrowserContext? _context;
    private IPage? _page;

    public IBrowser Browser =>
        _browser ?? throw new InvalidOperationException("PlaywrightDriver is not initialised.");

    public IBrowserContext Context =>
        _context ?? throw new InvalidOperationException("PlaywrightDriver is not initialised.");

    public IPage Page =>
        _page ?? throw new InvalidOperationException("PlaywrightDriver is not initialised.");

    public async Task InitialiseAsync(BrowserNewContextOptions? contextOptions = null)
    {
        if (_browser is not null && _context is not null && _page is not null)
        {
            return;
        }

        try
        {
            TestSettings config = TestConfigReader.Settings
                ?? throw new InvalidOperationException("Test settings could not be loaded.");

            BrowserSettings browserSettings = config.BrowserSettings
                ?? throw new InvalidOperationException("Browser settings are missing.");

            _playwright ??= await Playwright.CreateAsync();

            _browser = await CreateBrowserAsync(
                _playwright,
                browserSettings,
                new BrowserTypeLaunchOptions
                {
                    Headless = browserSettings.Headless,
                    SlowMo = browserSettings.SlowMo
                });

            _context = await _browser.NewContextAsync(
                contextOptions ?? new BrowserNewContextOptions { IgnoreHTTPSErrors = true });

            _page = await _context.NewPageAsync();
            AttachConsoleLogging(_page);
        }
        catch
        {
            await CloseAsync();
            throw;
        }
    }

    public async Task CloseAsync()
    {
        try
        {
            if (_page is not null && !_page.IsClosed)
            {
                await _page.CloseAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Driver Page Close Error] {ex}");
        }

        try
        {
            if (_context is not null)
            {
                await _context.CloseAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Driver Context Close Error] {ex}");
        }

        try
        {
            if (_browser is not null)
            {
                await _browser.CloseAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Driver Browser Close Error] {ex}");
        }
        finally
        {
            _playwright?.Dispose();

            _page = null;
            _context = null;
            _browser = null;
            _playwright = null;
        }
    }

    private static async Task<IBrowser> CreateBrowserAsync(
        IPlaywright playwright,
        BrowserSettings settings,
        BrowserTypeLaunchOptions options)
    {
        ArgumentNullException.ThrowIfNull(playwright);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(options);

        string browserName = settings.BrowserName.Trim().ToLowerInvariant()
            ?? throw new InvalidOperationException("Browser name is not configured.");

        return browserName switch
        {
            "chromium" => await playwright.Chromium.LaunchAsync(options),
            "firefox" => await playwright.Firefox.LaunchAsync(options),
            "webkit" => await playwright.Webkit.LaunchAsync(options),
            _ => throw new NotSupportedException($"Unsupported browser: {browserName}")
        };
    }

    private static void AttachConsoleLogging(IPage page)
    {
        ArgumentNullException.ThrowIfNull(page);

        page.Console += (_, msg) =>
        {
            Console.WriteLine($"[Browser Console] {msg.Type}: {msg.Text}");
        };
    }
}


