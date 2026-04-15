namespace GovUk.Forms.HostApp.UI.Test.Extensions;

public static class PageExtensions
{
    public static async Task TakeScreenshotAsync(this IPage page, IReqnrollOutputHelper outputHelper, string screenshotPath)
    {

        try
        {
            await page.ScreenshotAsync(new()
            {
                Path = screenshotPath,
                FullPage = true,
            });
            outputHelper.WriteLine($"Screenshot saved to: {screenshotPath}");
            outputHelper.AddAttachment(screenshotPath);
        }
        catch (IOException ex)
        {
            outputHelper.WriteLine($"Failed to save screenshot: {ex.Message}");
        }

    }
}
