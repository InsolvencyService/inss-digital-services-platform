using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Inss.Platform.Application.Clients;
using Inss.Platform.Application.Services;
using Inss.Platform.Domain.Components.Searching.Support;
using Inss.Platform.Infrastructure.Extensions;
using Microsoft.Extensions.Logging;

namespace Inss.Platform.Infrastructure.Clients;

public sealed class AzureSearchClient : ISearchClient
{
    private readonly ILogger<SearchService> _logger;
    private readonly SearchClient _searchClient;
    private const int LowerSuccessStatusCode = 200;
    private const int UpperSuccessStatusCode = 299;
    private const int NotFoundStatusCode = 404;

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

        if (statusCode is < LowerSuccessStatusCode or > UpperSuccessStatusCode)
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
    
    public async Task<SearchDetailResponse?> SearchDetailAsync(SearchDetailRequest request)
    {
        Response<SearchDocument>? response = await _searchClient.GetDocumentAsync<SearchDocument>(request.KeyValue);
        
        int statusCode = response?.GetRawResponse().Status ?? NotFoundStatusCode;

        if (statusCode is < LowerSuccessStatusCode or > UpperSuccessStatusCode)
        {
            _logger.AzureSearchDetailFailed(statusCode, request.KeyValue);
            return null;
        }

        Dictionary<string, string> fields = response!.Value
            .ToDictionary(
                field => field.Key,
                field => field.Value?.ToString() ?? string.Empty);

        return new SearchDetailResponse { Result = new SearchResult { Fields = fields } };
    }
}