using GovUk.Forms.Application.Services.Search;
using GovUk.Forms.Domain;
using GovUk.Forms.Infrastructure.Options;
using GovUk.Forms.Infrastructure.Services;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;

namespace GovUk.Forms.Infrastructure.Services;

public sealed class SearchPersonService : ISearchService
{
    private readonly HttpClient _httpClient;
    private readonly SearchPersonOptions _options;

    public SearchPersonService(HttpClient httpClient, SearchPersonOptions options)
    {
        _httpClient = httpClient;
        _options = options;
    }

    public async Task<SearchResponse> SearchAsync(
        string searchText, 
        int pageSize,
        int currentPageNumber)
    {
        if (string.IsNullOrEmpty(searchText))
        {
            // Return emoty class....
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

   
