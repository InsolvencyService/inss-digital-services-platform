using Inss.Platform.Domain.Components.Searching;

namespace GovUk.Forms.Application.Services;

public interface ISearchService
{
    Task<SearchResponse> SearchAsync(SearchRequest request);
    Task<SearchDetailResponse?> SearchDetailAsync(SearchDetailRequest request);
}