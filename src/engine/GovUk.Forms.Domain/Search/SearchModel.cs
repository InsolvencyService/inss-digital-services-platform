using System.ComponentModel.DataAnnotations;

namespace GovUk.Forms.Domain.Search;

public class SearchModel : PageModel
{
    [Required(ErrorMessage = "You must enter a search text")]
    public string SearchText { get; set; }

    public bool DisplayAsTable { get; set; }
    
    public string ConfigKey { get; set; }

    public int PageSize { get; set; }

    public int CurrentPageNumber { get; set; }

    public int TotalPages { get; set; }

    public int TotalResults { get; set; }

    public bool HasNextPage { get; set; }

    public SearchResultColumn[] ResultColumns { get; set; } = [];

    public SearchResult[] Results { get; set; } = [];
    
    public SearchResult? CurrentResult { get; set; }
    
    public override string[] GetSummaryInfo()
    {
        // TODO: Determine what this should be
        return [SearchText];
    }
    
    public override void CopyTo(PageModel target)
    {
        SearchModel search = target.As<SearchModel>();
        search.SearchText = SearchText;
        search.ResultColumns = ResultColumns;
        search.Results = Results;
        search.CurrentResult = CurrentResult;
    }
    
    public override void ClearValues()
    {
        base.ClearValues();
        SearchText = string.Empty;
    }

    public string GetResultsInfo()
    {
        int start = (CurrentPageNumber - 1) * PageSize + 1;
        int end = (start - 1) + PageSize > TotalResults ? TotalResults : (start - 1) + PageSize;
        return $"Showing {start} to {end} of {TotalResults} results for ";
    }
}

public sealed class SearchResultColumn
{
    public required string Name { get; init; }
    
    public string? Css { get; init; } // TODO: Enum?

    public  int Order { get; init; }

    public string? Header { get; init; }
}

public sealed class SearchResult
{
    public Dictionary<string, string> Fields { get; init; } = [];
}

public sealed class SearchRequest
{
    // string searchText, int pageSize, int currentPageNumber
    public string SearchText { get; init; }
    
    public int PageSize { get; init; }
    
    public int CurrentPageNumber { get; init; }
    
    public int Skip => (CurrentPageNumber - 1) * PageSize;

}
public sealed class SearchResponse
{
    public SearchResult[] Results { get; set; } = [];
    public int TotalResults { get; init; }
}