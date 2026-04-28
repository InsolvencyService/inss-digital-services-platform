using GovUk.Forms.HostApp.UI.Test.Helpers;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators;

public abstract class BaseCoordinator
{
    protected TestArtifacts TestArtifacts { get; }

    protected BaseCoordinator(TestArtifacts testArtifacts)
    {
        TestArtifacts = testArtifacts ?? throw new ArgumentNullException(nameof(testArtifacts));
    }

    protected async Task<string> CapturePageVisualAsync(
     Func<Task<byte[]>> captureAction,
     string name)
    {
        ArgumentNullException.ThrowIfNull(captureAction);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        byte[] screenshot = await captureAction();

        string path = TestArtifacts.GetScreenshotPath(name);

        await File.WriteAllBytesAsync(path, screenshot);

        return path;
    }

    protected static async Task<T> NavigateAsync<T>(Func<Task<T>> navigationAction)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(navigationAction);

        T page = await navigationAction();

        return page ?? throw new InvalidOperationException("Navigation did not return a valid page.");
    }


}