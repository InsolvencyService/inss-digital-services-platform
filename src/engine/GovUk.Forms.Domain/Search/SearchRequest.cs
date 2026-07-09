namespace GovUk.Forms.Domain.Search;

public sealed class SearchRequest
{
    public string SearchText { get; init; }
    
    public int PageSize { get; init; }
    
    public int CurrentPageNumber { get; init; }
    
    public int Skip => (CurrentPageNumber - 1) * PageSize;

}