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


    public async Task<byte[]> CaptureVisualAsync(IPage page)
    {
        ArgumentNullException.ThrowIfNull(page);

        return await page.ScreenshotAsync(new()
        {
            FullPage = true,
            Animations = ScreenshotAnimations.Disabled
        });
    }


    public async Task<IResponse> PageGoForwardAsync(IPage page, PageGoForwardOptions? options = null)
    {
        IResponse? response = await page.GoForwardAsync(options);

        return response ?? throw new InvalidOperationException("No response received when navigating forward.");
    }

    public async Task<IResponse> PageGoBackAsync(IPage page, PageGoBackOptions? options = null)
    {
        IResponse? response = await page.GoBackAsync(options);
        return response ?? throw new InvalidOperationException("No response received when navigating back.");
    }
}
