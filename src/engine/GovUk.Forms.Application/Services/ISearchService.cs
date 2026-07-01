
using GovUk.Forms.Domain.Search;

namespace GovUk.Forms.Application.Services;

public interface ISearchService
{
    Task<SearchResponse> SearchAsync(SearchRequest request);
}