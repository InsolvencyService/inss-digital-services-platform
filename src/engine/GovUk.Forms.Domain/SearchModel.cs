using System.ComponentModel.DataAnnotations;

namespace GovUk.Forms.Domain;

public class SearchModel : PageModel
{
    [Required(ErrorMessage = "You must enter a search text")]
    public string SearchText { get; set; }

    public bool DisplayAsTable { get; init; }

    public SearchResultColumn[] ResultColumns { get; set; } = [];

    public SearchResult[] Results { get; set; }
    
    public SearchResult? CurrentResult { get; set; }
    
    public void AddResultColumn(string name, string? css)
    {
        List<SearchResultColumn> columns = [..ResultColumns, new() { Name = name, Css = css }];
        ResultColumns = columns.ToArray();
    }
    
    public override string[] GetSummaryInfo()
    {
        // TODO: Determine what this should be
        return [SearchText];
    }
    
    public override void CopyTo(PageModel target)
    {
        SearchModel search = target.As<SearchModel>();
        search.SearchText = SearchText;
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
}

public sealed class SearchResult
{
    public string Id { get; init; }
}