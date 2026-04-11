using GovUk.Forms.HostApp.UI.Tests.Models.Settings;

namespace GovUk.Forms.HostApp.UI.Tests.Config.Driver;

public sealed class PlaywrightDriver : IPlaywrightDriver
{
    private IPlaywright? _playwright;
    private IPage? _page;
    private IBrowser? _browser;
    private IBrowserContext? _context;

    private readonly List<IPage> _pages = [];
    private readonly object _pagesLock = new();

    public IBrowser Browser =>
        _browser ?? throw new InvalidOperationException("PlaywrightDriver is not initialised.");

    public IBrowserContext Context =>
        _context ?? throw new InvalidOperationException("PlaywrightDriver is not initialised.");

    public IPage Page =>
        _page ?? throw new InvalidOperationException("There is no active page.");

    public IReadOnlyList<IPage> Pages
    {
        get
        {
            lock (_pagesLock)
            {
                return _pages.ToList().AsReadOnly();
            }
        }
    }

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
                    SlowMo = browserSettings.SlowMo,
                });

            _context = await _browser.NewContextAsync(contextOptions ?? new BrowserNewContextOptions());

            AttachContextHandlers(_context);

            IPage page = await _context.NewPageAsync();
            RegisterPage(page);
            AttachConsoleLogging(page);
        }
        catch
        {
            await CloseAsync();
            throw;
        }
    }

    public void SetActivePage(IPage page)
    {
        ArgumentNullException.ThrowIfNull(page);

        lock (_pagesLock)
        {
            if (!_pages.Contains(page))
            {
                _pages.Add(page);
            }

            _page = page;
        }
    }

    public async Task CloseAsync()
    {
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

            lock (_pagesLock)
            {
                _pages.Clear();
                _page = null;
            }

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

        string browserName = settings.BrowserName?.Trim().ToLowerInvariant()
            ?? throw new InvalidOperationException("Browser name is not configured.");

        return browserName switch
        {
            "chromium" => await playwright.Chromium.LaunchAsync(options),
            "firefox" => await playwright.Firefox.LaunchAsync(options),
            "webkit" => await playwright.Webkit.LaunchAsync(options),
            _ => throw new NotSupportedException($"Unsupported browser: {browserName}")
        };
    }

    private void AttachContextHandlers(IBrowserContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        context.Page += (_, page) =>
        {
            RegisterPage(page);
            AttachConsoleLogging(page);
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

    private void RegisterPage(IPage page)
    {
        ArgumentNullException.ThrowIfNull(page);

        lock (_pagesLock)
        {
            if (!_pages.Contains(page))
            {
                _pages.Add(page);
            }

            _page = page;
        }

        page.Close += (_, _) =>
        {
            lock (_pagesLock)
            {
                _pages.Remove(page);

                if (ReferenceEquals(_page, page))
                {
                    _page = _pages.Count > 0
                        ? _pages[^1]
                        : null;
                }
            }
        };
    }
}


