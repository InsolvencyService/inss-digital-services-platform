namespace Inss.Platform.Domain.Components.Searching;

public sealed class SearchResponse
{
    public SearchResult[] Results { get; init; } = [];
    public int TotalResults { get; init; }
}