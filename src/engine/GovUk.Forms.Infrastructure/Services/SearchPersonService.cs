using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using GovUk.Forms.Application.Services.Search;
using GovUk.Forms.Domain;
using Microsoft.Extensions.Logging;

namespace GovUk.Forms.Infrastructure.Services;

public sealed class SearchPersonService : ISearchService
{
    private readonly ILogger<SearchPersonService> _logger;
    private readonly SearchClient _searchClient;

    public SearchPersonService(SearchClient searchClient, ILogger<SearchPersonService> logger)
    {
        _logger = logger;
        _searchClient = searchClient;
    }

    public async Task<SearchResponse> SearchAsync(
    string searchText,
    int pageSize,
    int currentPageNumber)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            _logger.LogWarning("Azure Search request sent with an empty search text.");
            return new SearchResponse();
        }

        if (pageSize <= 0)
        {
            _logger.LogError(
                "Azure Search request made with invalid page size : {PageSize}.", 
                pageSize);
            return new SearchResponse();
        }

        if (currentPageNumber <= 0)
        {
            _logger.LogWarning(
                "Azure Search request sent with invalid current page number: {CurrentPageNumber}", 
                currentPageNumber);
            // Reset to 1 if invalid..
           currentPageNumber = 1;
        }

        int skip = (currentPageNumber - 1) * pageSize;
 
        SearchOptions searchOptions = new()
        {
            Size = pageSize,
            Skip = skip,
            IncludeTotalCount = true
        };

        Response<SearchResults<SearchDocument>> response =
            await _searchClient.SearchAsync<SearchDocument>(searchText, searchOptions);

        int statusCode = response.GetRawResponse().Status;

        if (statusCode is < 200 or > 299)
        {
            _logger.LogError(
                "Azure Search request has failed.  Status Code:  {StatusCode}, SearchText: {SearchText}", 
                statusCode, 
                searchText);

            return new SearchResponse();
        }

        List<SearchResult> results = [];

        await foreach (SearchResult<SearchDocument> result in response.Value.GetResultsAsync())
        {
            Dictionary<string, string> fields = result.Document
                .ToDictionary(
                    field => field.Key,
                    field => field.Value?.ToString() ?? string.Empty);

            results.Add(new SearchResult
            {
                Fields = fields
            });
        }

        return new SearchResponse
        {
            Results = [.. results],
            TotalResults = (int)(response.Value.TotalCount ?? 0)
        };
    }
}
