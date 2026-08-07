using GovUk.Forms.Application.Services;
using Inss.Platform.Application.Clients;
using Inss.Platform.Application.Extensions;
using Inss.Platform.Domain.Components.Searching.Support;
using Microsoft.Extensions.Logging;

namespace Inss.Platform.Application.Services;

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

        _logger.PerformSearch(request.SearchText, request.Skip, request.PageSize);
        return await _searchClient.SearchAsync(request);
    }

    public async Task<SearchDetailResponse?> SearchDetailAsync(SearchDetailRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.KeyField) ||
            string.IsNullOrWhiteSpace(request.KeyValue))
        {
            _logger.MissingSearchKeyAndOrValue();
            return null;
        }

        _logger.PerformDetailSearch(request.KeyField, request.KeyValue);
        return await _searchClient.SearchDetailAsync(request);
    }
}
