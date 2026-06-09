using System.ComponentModel.DataAnnotations;

namespace GovUk.Forms.Domain;

public class SearchModel : PageModel
{
    [Required(ErrorMessage = "You must enter a search text")]
    public string SearchText { get; set; }

    public bool DisplayAsTable { get; init; }
    
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