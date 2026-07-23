using System.ComponentModel.DataAnnotations;

namespace GovUk.Forms.Domain.Search.Formatting;

public sealed class EmailSearchFieldValueFormatter : SearchFieldValueFormatter
{
    public override string Format(string?[] values)
    {
        if (values.Length == 0)
        {
            return string.Empty;
        }

        string? value = values[0];
        
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }
        
        EmailAddressAttribute attr = new();
        return attr.IsValid(value) ? $"<a href='mailto:{value}' class='govuk-link'>{value}</a>" : string.Empty;
    }
}