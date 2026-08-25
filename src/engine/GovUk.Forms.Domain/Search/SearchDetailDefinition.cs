// ReSharper disable UnusedAutoPropertyAccessor.Global - Json serialization

using GovUk.Forms.Domain.Search.Formatting;

namespace GovUk.Forms.Domain.Search;

public sealed class SearchDetailDefinition
{
    public required string[] Names { get; init; }
    
    public string? Header { get; init; }

    public int? Order { get; init; }
    
    public required string Category { get; init; }
    
    public string? FormatterType { get; init; }

    public string? Description { get; init; } = string.Empty;

    public string GetLabel()
    {
        return !string.IsNullOrWhiteSpace(Header) ? Header : string.Join(' ', Names).Trim();
    }
    
    public string GetValue(string[] values)
    {
        SearchFieldValueFormatter formatter = SearchFieldValueFormatter.CreateFormatter(FormatterType);
        return formatter.Format(values);
    }
}