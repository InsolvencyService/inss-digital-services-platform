using GovUk.Forms.Domain.Search;

namespace GovUk.Forms.Application.Clients;

public interface ISearchClient
{
    Task<SearchResponse> SearchAsync(SearchRequest request);
    Task<SearchDetailResponse?> SearchDetailAsync(SearchDetailRequest request);
}