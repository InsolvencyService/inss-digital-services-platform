namespace GovUk.Forms.HostApp.UI.Test.Pages.Common;

public interface ICommonPage
{
    Task<IPage> OpenNewTabAndVerifyAsync(IBrowserContext browserContext, Func<Task> triggerAction, string? expectedUrlPart = null);
    Task<string> CaptureVisualAsync(IPage page, string screenshotName);
}
