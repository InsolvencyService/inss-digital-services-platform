using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace GovUk.Forms.Domain;

public class SearchModel : PageModel
{
    [Required(ErrorMessage = "You must enter a search text")]
    public string SearchText { get; set; }

    public bool DisplayAsTable { get; set; }

    public int PageSize { get; set; }

    public int CurrentPageNumber { get; set; }

    public bool HasNextPage { get; set; }

    public SearchResultColumn[] ResultColumns { get; set; } = [];

    public SearchResult[] Results { get; set; } = [];
    
    public SearchResult? CurrentResult { get; set; }
    
    public void AddResultColumn(string name, string? css)
    {
        List<SearchResultColumn> columns = [..ResultColumns, new() { Name = name, Css = css }];
        ResultColumns = columns.ToArray();
    }



    // public List<SearchColumnOptions> Columns { get; set; } = new List<SearchColumnOptions>();

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
    //public string Id { get; init; }
    public Dictionary<string, string> Fields { get; init; } = [];
}