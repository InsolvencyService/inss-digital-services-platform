using GovUk.Forms.HostApp.UI.Tests.Helpers;

namespace GovUk.Forms.HostApp.UI.Tests.Extensions;

public static class TestArtifactsExtensions
{
    extension(TestArtifacts artifacts)
    {
        public string CreateFailureScreenshotPath()
            => artifacts.GetScreenshotPath("failure");

        public string CreateStepScreenshotPath(string stepName)
            => artifacts.GetScreenshotPath(stepName);

        public string CreateFailureVideoPath()
            => artifacts.GetVideoPath("failure");

        public void EnsureExists()
        {
            Directory.CreateDirectory(artifacts.Folder);
            Directory.CreateDirectory(artifacts.VideoDirectory);
            Directory.CreateDirectory(artifacts.ScreenshotsDirectory);
        }
    }

}
