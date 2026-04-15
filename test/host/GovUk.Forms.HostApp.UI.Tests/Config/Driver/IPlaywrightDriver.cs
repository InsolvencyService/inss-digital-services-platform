namespace GovUk.Forms.HostApp.UI.Test.Config.Driver;

public interface IPlaywrightDriver
{
    IBrowser Browser { get; }
    IBrowserContext Context { get; }
    IPage Page { get; }

    Task InitialiseAsync(BrowserNewContextOptions? contextOptions = null);
    Task CloseAsync();
}