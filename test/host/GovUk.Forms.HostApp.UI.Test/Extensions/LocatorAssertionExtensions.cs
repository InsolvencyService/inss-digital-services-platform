namespace GovUk.Forms.HostApp.UI.Test.Extensions;

public static class LocatorAssertionExtensions
{
    public static async Task ShouldBeVisibleAsync(this ILocator locator)
    {
        await Expect(locator).ToBeVisibleAsync();
    }
}
