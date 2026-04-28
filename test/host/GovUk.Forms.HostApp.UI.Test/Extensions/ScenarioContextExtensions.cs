using GovUk.Forms.HostApp.UI.Test.Helpers;

namespace GovUk.Forms.HostApp.UI.Test.Extensions;

public static class ScenarioContextExtensions
{
    private const string ScreencastKey = "Screencast";

    public static void SetScreencast(this ScenarioContext scenarioContext, ScreencastHelper screencast)
    {
        ArgumentNullException.ThrowIfNull(scenarioContext);
        ArgumentNullException.ThrowIfNull(screencast);

        scenarioContext[ScreencastKey] = screencast;
    }

    public static ScreencastHelper? GetScreencast(this ScenarioContext scenarioContext)
    {
        ArgumentNullException.ThrowIfNull(scenarioContext);

        return scenarioContext.TryGetValue(ScreencastKey, out object? value)
            ? value as ScreencastHelper
            : null;
    }
}
