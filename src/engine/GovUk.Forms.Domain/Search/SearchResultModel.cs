using System.ComponentModel.DataAnnotations;
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
}