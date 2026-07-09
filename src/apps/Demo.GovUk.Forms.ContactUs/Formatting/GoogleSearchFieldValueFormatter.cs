using GovUk.Forms.Domain.Search.Formatting;
// ReSharper disable UnusedType.Global - used in config

namespace Demo.GovUk.Forms.ContactUs.Formatting;

public sealed class GoogleSearchFieldValueFormatter : FieldValueFormatter
{
    public override string Format(string? value)
    {
        return $"<a href='https://www.google.com/search?q={value ?? string.Empty}'>{value}</a>";
    }
}