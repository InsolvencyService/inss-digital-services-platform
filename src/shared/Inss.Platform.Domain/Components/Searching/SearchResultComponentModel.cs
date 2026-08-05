using System.Net;
using Inss.Platform.Domain.Components.Searching.Formatting;
using Inss.Platform.Domain.Exceptions;
using Inss.Platform.Domain.Primitives;

namespace Inss.Platform.Domain.Components.Searching;

public class SearchResultComponentModel : ComponentModel, IValueComponent, IQueryParamComponent
{
    public override string ViewName => "_SearchResult";
    
    public required string ConfigKey { get; init; }

    public int CurrentPageNumber { get; set; }

    public int TotalPages { get; set; }

    public int TotalResults { get; set; }

    public bool HasNextPage { get; init; }
    
    public required Content Label { get; init; }
    
    public Content? Hint { get; init; }
    
    public string? Value { get; set; }
    
    public SearchResult[] Results { get; set; } = [];

    public SearchDefinition Definition { get; set; } = new();
    
    public PagePath ResultDetailPath { get; set; }
    
    //public PagePath SearchPath => CurrentPageNumber > 1 
    //    ? $"{Path}?keyword={Value}&currentPageNumber={CurrentPageNumber}" : $"{Path}?keyword={Value}";
    
    public int StartPageNumber => (CurrentPageNumber - 1) * Definition.PageSize + 1;
    
    public int EndPageNumber => (StartPageNumber - 1) + Definition.PageSize > TotalResults 
        ? TotalResults : (StartPageNumber - 1) + Definition.PageSize;
    
    public override void CopyTo(ComponentModel targetComponent)
    {
        SearchResultComponentModel searchTerm = targetComponent.As<SearchResultComponentModel>();
        searchTerm.Value = Value;
    }
    
    public SearchResultDefinition[] GetOrderedDisplayResults()
    {
        return Definition.Results.Where(f => f.IsDisplayable).OrderBy(x => x.Order).ToArray();
    }
    
    public bool IsFirstDisplayableColumn(SearchResultDefinition column)
    {
        return Definition.Results.OrderBy(r => r.Order).First(r => r.IsDisplayable) == column;
    }
    
    public string GetResultDetailLink(SearchResult result, string displayValue)
    {
        SearchResultDefinition? identifierDefinition = Definition.Results.SingleOrDefault(r => r.IsIdentifier);

        if (identifierDefinition is null || identifierDefinition.Names.Length == 0)
        {
            throw new ComponentException("Unable to find an identifier for result detail links.");
        }

        string key = identifierDefinition.Names[0];
        string value = result.Fields[key];
        return $"<a href='{ResultDetailPath}?key={WebUtility.UrlEncode(key)}" +
               $"&value={WebUtility.UrlEncode(value)}' class='govuk-link'>{displayValue}</a>";
    }
    
    public void Append(QueryParamList queryParams)
    {
        queryParams.AddQueryParam("keyword", Value);
    }
}

public sealed class SearchResult
{
    public Dictionary<string, string> Fields { get; init; } = [];
}

public sealed class SearchDefinition
{
    public int PageSize { get; init; }
    
    public bool DisplayAsTable { get; init; }

    public SearchResultDefinition[] Results { get; init; } = [];
    
    public SearchDetailDefinition[] Details { get; init; } = [];
    
    public SearchCategory[] Categories { get; init; } = [];
}

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

public sealed class SearchDetailDefinition
{
    public required string[] Names { get; init; }
    
    public string? Header { get; init; }

    public int? Order { get; init; }
    
    public required string Category { get; init; }
    
    public string? FormatterType { get; init; }

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

[Flags]
public enum SearchResultType
{
    Key = 1,
    Hidden = 2,
    Display = 4
}

public sealed class SearchCategory
{
    public string Label { get; init; }
    
    public string? Css { get; init; }
}