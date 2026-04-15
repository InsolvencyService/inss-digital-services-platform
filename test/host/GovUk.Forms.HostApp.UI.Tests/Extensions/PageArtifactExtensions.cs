using GovUk.Forms.HostApp.UI.Test.Helpers;

namespace GovUk.Forms.HostApp.UI.Test.Extensions;

public static class PageArtifactExtensions
{
    extension(IPage page)
    {
        public async Task SaveFinalScreenshotAsync(
            TestArtifacts artifacts,
            IReqnrollOutputHelper outputHelper,
            string testName)
        {
            ArgumentNullException.ThrowIfNull(page);
            ArgumentNullException.ThrowIfNull(artifacts);
            ArgumentNullException.ThrowIfNull(outputHelper);
            ArgumentException.ThrowIfNullOrWhiteSpace(testName);

            try
            {
                string path = artifacts.GetScreenshotPath($"{testName}_Final_{DateTime.UtcNow:HH-mm-ss}");

                await page.TakeScreenshotAsync(outputHelper, path);
            }
            catch (Exception ex)
            {
                outputHelper.WriteLine($"[Screenshot Error] {ex.Message}");
            }
        }

        public async Task SaveVideoToArtifactsAsync(
            TestArtifacts artifacts,
            IReqnrollOutputHelper outputHelper,
            string videoName)
        {
            ArgumentNullException.ThrowIfNull(page);
            ArgumentNullException.ThrowIfNull(artifacts);
            ArgumentNullException.ThrowIfNull(outputHelper);
            ArgumentException.ThrowIfNullOrWhiteSpace(videoName);

            if (page.Video is null)
            {
                outputHelper.WriteLine("[Video] No video available on page.");
                return;
            }

            try
            {
                string path = artifacts.GetVideoPath(videoName);

                await page.Video.SaveAsAsync(path);

                outputHelper.WriteLine($"Video saved: {path}");

                if (File.Exists(path))
                {
                    outputHelper.AddAttachment(path);
                }
            }
            catch (Exception ex)
            {
                outputHelper.WriteLine($"[Video Error] {ex.Message}");
            }
        }
    }
}
