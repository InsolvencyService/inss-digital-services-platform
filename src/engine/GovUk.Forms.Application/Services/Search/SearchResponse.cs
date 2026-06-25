using GovUk.Forms.Domain;

namespace GovUk.Forms.Application.Services.Search;

public sealed class SearchResponse
{
    public SearchResult[] Results { get; set; } = [];
    public int TotalResults { get; init; }
}
