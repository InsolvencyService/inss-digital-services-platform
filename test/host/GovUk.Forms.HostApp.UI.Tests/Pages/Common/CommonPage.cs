using System.Text.RegularExpressions;

namespace GovUk.Forms.HostApp.UI.Test.Pages.Common;

public class CommonPage : ICommonPage
{

    public async Task<IPage> OpenNewTabAndVerifyAsync(
    IBrowserContext browserContext,
    Func<Task> triggerAction,
    string? expectedUrlPart = null)
    {
        ArgumentNullException.ThrowIfNull(browserContext);
        ArgumentNullException.ThrowIfNull(triggerAction);

        IPage newPage = await browserContext.RunAndWaitForPageAsync(triggerAction);

        await newPage.WaitForLoadStateAsync(LoadState.Load);

        if (!string.IsNullOrWhiteSpace(expectedUrlPart))
        {
            await Expect(newPage).ToHaveURLAsync(new Regex(expectedUrlPart));
        }

        return newPage;
    }
}
