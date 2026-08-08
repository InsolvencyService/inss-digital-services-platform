using Inss.Platform.Domain.Components.Searching.Support;

namespace Inss.Platform.Application.Clients;

public interface ISearchClient
{
    Task<SearchResponse> SearchAsync(SearchRequest request);
    Task<SearchDetailResponse?> SearchDetailAsync(SearchDetailRequest request);
}