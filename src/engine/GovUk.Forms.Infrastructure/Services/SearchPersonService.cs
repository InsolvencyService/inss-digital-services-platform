using GovUk.Forms.Application.Services.Search;
using GovUk.Forms.Domain;
using GovUk.Forms.Infrastructure.Helpers.SearchHelpers;
using GovUk.Forms.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace GovUk.Forms.Infrastructure.Services;

public sealed class SearchPersonService : ISearchService
{
    private readonly HttpClient _httpClient;
    private readonly SearchPersonOptions _options;
    private readonly ILogger<SearchPersonService> _logger;

    public SearchPersonService(HttpClient httpClient, SearchPersonOptions options, Logger<SearchPersonService> logger)
    {
        _httpClient = httpClient;
        _options = options;
        _logger = logger;
    }

    public async Task<SearchResponse> SearchAsync(
        string searchText, 
        int pageSize,
        int currentPageNumber)
    {
        if (string.IsNullOrEmpty(searchText))
        {
            return new SearchResponse
            {
                Results = [],
                TotalResults = 0
            };
        }

        int skip = (currentPageNumber - 1) * pageSize;

        string url = $"{_options.Endpoint}/indexes/{_options.IndexName}/docs/search?api-version={_options.ApiVersion}";
        using HttpRequestMessage request = new(HttpMethod.Post, url);
        request.Headers.Add("api-key", _options.ApiKey);
        request.Content = JsonContent.Create(new
        {
            search = searchText,
            top = pageSize,
            count = true,
            skip

        });

        HttpResponseMessage response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Azure Search request has failed.  Status Code: {StatusCode}", response.StatusCode);
            return new SearchResponse();
        }

        JsonDocument json = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());

        int totalResults = 0;

        if (json.RootElement.TryGetProperty("@odata.count", out JsonElement countElement))
        {
            totalResults = countElement.GetInt32();
        }

        List<SearchResult> results = [];

        foreach (JsonElement item in json.RootElement
            .GetProperty("value")
            .EnumerateArray())
        {
            SearchResult result = new()
            {
                Fields = item.EnumerateObject().ToDictionary(
                property => property.Name,
                property => property.Value.ToString())
            };

            results.Add(result);
        }

        return new SearchResponse
        {
            Results = [.. results],
            TotalResults = totalResults
        };
    }
}

   
