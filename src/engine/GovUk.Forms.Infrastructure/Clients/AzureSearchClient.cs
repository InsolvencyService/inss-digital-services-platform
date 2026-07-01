using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using GovUk.Forms.Application.Clients;
using GovUk.Forms.Application.Services;
using GovUk.Forms.Domain.Search;
using GovUk.Forms.Infrastructure.Extensions;
using Microsoft.Extensions.Logging;

namespace GovUk.Forms.Infrastructure.Clients;

public sealed class AzureSearchClient : ISearchClient
{
    private readonly ILogger<SearchService> _logger;
    private readonly SearchClient _searchClient;

    public AzureSearchClient(SearchClient searchClient, ILogger<SearchService> logger)
    {
        _logger = logger;
        _searchClient = searchClient;
    }

    public async Task<SearchResponse> SearchAsync(SearchRequest request)
    {
        SearchOptions searchOptions = new() { Size = request.PageSize, Skip = request.Skip, IncludeTotalCount = true };

        Response<SearchResults<SearchDocument>> response = 
            await _searchClient.SearchAsync<SearchDocument>(request.SearchText, searchOptions);

        int statusCode = response.GetRawResponse().Status;

        if (statusCode is < 200 or > 299)
        {
            _logger.AzureSearchFailed(statusCode, request.SearchText);

            return new SearchResponse();
        }

        List<SearchResult> results = [];

        await foreach (SearchResult<SearchDocument> result in response.Value.GetResultsAsync())
        {
            Dictionary<string, string> fields = result.Document
                .ToDictionary(
                    field => field.Key,
                    field => field.Value?.ToString() ?? string.Empty);

            results.Add(new SearchResult { Fields = fields });
        }

        return new SearchResponse { Results = [.. results], TotalResults = (int)(response.Value.TotalCount ?? 0) };
    }
}