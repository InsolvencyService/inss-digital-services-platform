// ReSharper disable UnusedAutoPropertyAccessor.Global - Json serialization

using System.Linq;
using GovUk.Forms.Domain.Search.Formatting;

namespace GovUk.Forms.Domain.Search;

public sealed class SearchResultDefinition
{
    public required string[] Names { get; init; }
    
    public string? Header { get; init; }
    
    public string? Css { get; init; }

    public int? Order { get; init; }

    public string? FormatterType { get; init; }
    
    public SearchResultType ColumnType { get; init; } = SearchResultType.Display;

    public bool IsDisplayable => (ColumnType & SearchResultType.Display) == SearchResultType.Display;
    
    public bool IsIdentifier => (ColumnType & SearchResultType.Key) == SearchResultType.Key;
    
    public string GetValueForNames(Dictionary<string, string> fields)
    {
        List<string> values = [];

        foreach (string name in Names.Where(fields.ContainsKey))
        {
            if (fields.TryGetValue(name, out string? fieldValue))
            {
                values.Add(fieldValue);
            }
        }

        SearchFieldValueFormatter formatter = SearchFieldValueFormatter.CreateFormatter(FormatterType);
        return formatter.Format(values.ToArray());
    }
}