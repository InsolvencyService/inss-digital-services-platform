using GovUk.Forms.Application.Clients;
using GovUk.Forms.Application.Extensions;
using GovUk.Forms.Domain.Search;
using Microsoft.Extensions.Logging;

namespace GovUk.Forms.Application.Services;

public sealed class SearchService : ISearchService
{
    private readonly ILogger<SearchService> _logger;
    private readonly ISearchClient _searchClient;

    public SearchService(ISearchClient searchClient, ILogger<SearchService> logger)
    {
        _logger = logger;
        _searchClient = searchClient;
    }

    public async Task<SearchResponse> SearchAsync(SearchRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.SearchText))
        {
            _logger.MissingSearchText();
            return new SearchResponse();
        }

        if (request.PageSize <= 0)
        {
            _logger.InvalidSearchPageSize(request.PageSize);
            return new SearchResponse();
        }

        if (request.CurrentPageNumber <= 0)
        {
            _logger.InvalidCurrentPageNumber(request.CurrentPageNumber);
            return new SearchResponse();
        }

        return await _searchClient.SearchAsync(request);
    }
}
