namespace GovUk.Forms.HostApp.UI.Test.Extensions;

public static class EnumerableExtensions
{
    public static bool IsAnyOf<T>(this T enumerable, params T[] values)
    {
        ArgumentNullException.ThrowIfNull(values);

        return values.Contains(enumerable);
    }
}
