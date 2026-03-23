using System.Diagnostics.CodeAnalysis;

namespace GovUk.Forms.Components.AttributeHelpers;

[ExcludeFromCodeCoverage]
public static class InputAttributes
{
    public static readonly Dictionary<string, string?> AutoFocus = new()
    {
        { "autofocus", "autofocus" }
    };
}