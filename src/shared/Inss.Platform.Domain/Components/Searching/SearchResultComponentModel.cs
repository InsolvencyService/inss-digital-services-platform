using System.Net;
using Inss.Platform.Domain.Components.Searching.Support;
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
    
    public required string Label { get; init; }
    
    public string? Hint { get; init; }
    
    public string? Value { get; set; }
    
    public SearchResult[] Results { get; set; } = [];

    public SearchDefinition Definition { get; set; } = new();
    
    public PagePath ResultDetailPath { get; init; }
    
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