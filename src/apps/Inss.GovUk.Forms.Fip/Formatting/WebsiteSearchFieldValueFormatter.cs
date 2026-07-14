using System.Text.RegularExpressions;
using GovUk.Forms.Domain.Search.Formatting;

// ReSharper disable UnusedType.Global - used in config

namespace Inss.GovUk.Forms.Fip.Formatting;

public sealed partial class WebsiteSearchFieldValueFormatter : FieldValueFormatter
{
    private const string Pattern = @"^https:\/\/(?:www\.)?[a-zA-Z0-9-]+(?:\.[a-zA-Z]{2,})+(?::\d+)?(?:\/[^\s]*)?$";
    
    public override string Format(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }
        
        return WebsiteLinkRegex().IsMatch(value)
            ? $"<a target='_blank' href='{value}'>{value}</a>"
            : string.Empty;
    }

    [GeneratedRegex(Pattern, RegexOptions.IgnoreCase, "en-GB")]
    private static partial Regex WebsiteLinkRegex();
}