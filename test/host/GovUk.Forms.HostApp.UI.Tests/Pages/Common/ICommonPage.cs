namespace GovUk.Forms.HostApp.UI.Tests.Pages.Common;

public interface ICommonPage
{
    Task<IPage> OpenNewTabAndVerifyAsync(IBrowserContext browserContext, Func<Task> triggerAction, string? expectedUrlPart = null);
}
