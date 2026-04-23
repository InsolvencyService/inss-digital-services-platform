namespace GovUk.Forms.HostApp.UI.Test.Coordinators;

public class BaseCoordinator
{
    public static async Task<T> NavigateAsync<T>(Func<Task<T>> navigationAction)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(navigationAction);

        T page = await navigationAction();

        return page ?? throw new InvalidOperationException("Navigation did not return a valid page.");
    }

    public static async Task<string> CapturePageVisualAsync(
       Func<string, Task<string>> captureAction,
       string name)
    {
        return await captureAction(name);
    }
}