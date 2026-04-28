using GovUk.Forms.HostApp.UI.Test.Helpers;

namespace GovUk.Forms.HostApp.UI.Test.Extensions;

public static class VideoExtensions
{
    public static async Task<string?> SaveToArtifactsAsync(
        this IVideo video,
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
                return path;
            }

            return null;
        }
        catch (Exception ex)
        {
            outputHelper.WriteLine($"[Video Error] {ex.Message}");
            return null;
        }
    }

    public static async Task<string?> SaveRecordedVideoToArtifactsAsync(
    this IVideo video,
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
            string sourcePath = await video.PathAsync();

            if (!File.Exists(sourcePath))
            {
                return null;
            }

            string targetPath = artifacts.GetVideoPath(videoName);

            if (!string.Equals(sourcePath, targetPath, StringComparison.OrdinalIgnoreCase))
            {
                File.Move(sourcePath, targetPath, overwrite: true);
            }

            outputHelper.WriteLine($"Video saved: {targetPath}");
            outputHelper.AddAttachment(targetPath);

            return targetPath;
        }
        catch (Exception ex)
        {
            outputHelper.WriteLine($"[Video Error] {ex.Message}");
            return null;
        }
    }
}
