using GovUk.Forms.HostApp.UI.Tests.Helpers;

namespace GovUk.Forms.HostApp.UI.Tests.Extensions;

public static class VideoExtensions
{
    extension(IVideo video)
    {
        public async Task SaveToArtifactsAsync(
            TestArtifacts artifacts,
            IReqnrollOutputHelper outputHelper,
            string videoName)
        {
            ArgumentNullException.ThrowIfNull(video);
            ArgumentNullException.ThrowIfNull(artifacts);
            ArgumentNullException.ThrowIfNull(outputHelper);
            ArgumentException.ThrowIfNullOrWhiteSpace(videoName);

            try
            {
                string path = artifacts.GetVideoPath(videoName);

                await video.SaveAsAsync(path);

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
