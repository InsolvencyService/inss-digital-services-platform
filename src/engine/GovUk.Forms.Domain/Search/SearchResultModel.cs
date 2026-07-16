using System.ComponentModel.DataAnnotations;
using System.Net;
using GovUk.Forms.Domain.Exceptions;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Domain.Search;

public class SearchResultModel : PageModel
{
    [Required(ErrorMessage = "You must enter a search text")]
    public string SearchText { get; set; }

    public string ConfigKey { get; init; }

    public int CurrentPageNumber { get; set; }

    public int TotalPages { get; set; }

    public int TotalResults { get; set; }

    public bool HasNextPage { get; init; }
    
    public ContentPath ResultDetailPath { get; set; }
    
    public SearchResult[] Results { get; set; } = [];

    public SearchDefinition Definition { get; set; } = new();

    public ContentPath SearchPath => CurrentPageNumber > 1 
        ? $"{Path}?keyword={SearchText}&currentPageNumber={CurrentPageNumber}" : $"{Path}?keyword={SearchText}";
    
    public int StartPageNumber => (CurrentPageNumber - 1) * Definition.PageSize + 1;
    
    public int EndPageNumber => (StartPageNumber - 1) + Definition.PageSize > TotalResults 
        ? TotalResults : (StartPageNumber - 1) + Definition.PageSize;
    
    public override void CopyTo(PageModel target)
    {
        SearchResultModel searchResult = target.As<SearchResultModel>();
        searchResult.SearchText = SearchText;
        searchResult.Definition = Definition;
        searchResult.Results = Results;
    }
    
    public override void ClearValues()
    {
        base.ClearValues();
        SearchText = string.Empty;
        Results = [];
        TotalPages = 0;
        CurrentPageNumber = 0;
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
            throw new ModelException("Unable to find an identifier for result detail links.");
        }

        string key = identifierDefinition.Names[0];
        string value = result.Fields[key];
        return $"<a href='{ResultDetailPath}/?key={WebUtility.UrlEncode(key)}" +
               $"&value={WebUtility.UrlEncode(value)}' class='govuk-link'>{displayValue}</a>";
    }
}