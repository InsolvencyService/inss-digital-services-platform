namespace GovUk.Forms.HostApp.UI.Tests.Config.Driver;

public interface IPlaywrightDriver
{
    IBrowser Browser { get; }
    IBrowserContext Context { get; }
    IPage Page { get; }
    IReadOnlyList<IPage> Pages { get; }
    Task InitialiseAsync(BrowserNewContextOptions? contextOptions = null);
    Task CloseAsync();
    void SetActivePage(IPage page);
}