namespace GovUk.Forms.HostApp.UI.Test.Pages.Common;

public interface ICommonPage
{
    Task<IPage> OpenNewTabAndVerifyAsync(IBrowserContext browserContext, Func<Task> triggerAction, string? expectedUrlPart = null);
    Task<string> CaptureVisualAsync(IPage page, string screenshotName);
    Task<IResponse> PageGoForwardAsync(IPage page, PageGoForwardOptions? options = null);
    Task<IResponse> PageGoBackAsync(IPage page, PageGoBackOptions? options = null);
}
