namespace GovUk.Forms.Domain.Search;

public sealed class SearchResponse
{
    public SearchResult[] Results { get; init; } = [];
    public int TotalResults { get; init; }
}