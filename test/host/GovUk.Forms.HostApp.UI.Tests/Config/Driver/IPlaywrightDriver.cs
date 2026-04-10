namespace GovUk.Forms.HostApp.UI.Tests.Config.Driver;

public interface IPlaywrightDriver
{
    IPage Page { get; }
    IBrowser Browser { get; }
    IBrowserContext Context { get; }
    Task InitialiseAsync();
    Task<string> TakeScreenshotAsync(string name);
    void SetActivePage(IPage page);
    IReadOnlyList<IPage> Pages { get; }
}