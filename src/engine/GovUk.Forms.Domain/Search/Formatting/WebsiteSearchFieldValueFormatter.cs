using System.Text.RegularExpressions;

// ReSharper disable UnusedType.Global - used in config

namespace GovUk.Forms.Domain.Search.Formatting;

public sealed partial class WebsiteSearchFieldValueFormatter : SearchFieldValueFormatter
{
    private const string Pattern = @"^(?:https:\/\/)?(?:www\.)?[a-zA-Z0-9-]+(?:\.[a-zA-Z]{2,})+(?::\d+)?(?:\/[^\s]*)?$";
    
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
        
        return WebsiteLinkRegex().IsMatch(value)
            ? $"<a target='_blank' href='{value}' class='govuk-link'>{value}</a>"
            : string.Empty;
    }

    [GeneratedRegex(Pattern, RegexOptions.IgnoreCase)]
    private static partial Regex WebsiteLinkRegex();
}