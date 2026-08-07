using Inss.Platform.Domain.Components.Searching.Support;

namespace Inss.Platform.Application.Services;

public interface ISearchService
{
    Task<SearchResponse> SearchAsync(SearchRequest request);
    Task<SearchDetailResponse?> SearchDetailAsync(SearchDetailRequest request);
}