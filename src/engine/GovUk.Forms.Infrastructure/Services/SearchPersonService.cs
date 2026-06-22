using GovUk.Forms.Application.Services.Search;
using GovUk.Forms.Domain;
using GovUk.Forms.Infrastructure.Options;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;

namespace GovUk.Forms.Infrastructure.Services;

public sealed class SearchPersonService : ISearchService
{
    private readonly HttpClient _httpClient;
    private readonly SearchPersonOptions _options;

    public SearchPersonService(HttpClient httpClient, SearchPersonOptions options)// IOptions<SearchPersonOptions> options)
    {
        _httpClient = httpClient;
        _options = options;
    }

    public async Task<SearchResult[]> SearchAsync(string searchText)
    {
        // TODO: Implement search logic
        if (string.IsNullOrEmpty(searchText))
        {
            return [];
        }

        string url = $"{_options.Endpoint}/indexes/{_options.IndexName}/docs/search?api-version={_options.ApiVersion}";
        using HttpRequestMessage request = new(HttpMethod.Post, url);
        request.Headers.Add("api-key", _options.ApiKey);
        request.Content = JsonContent.Create(new
        {
            search = searchText,
            top = 100
        });

        HttpResponseMessage response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        JsonDocument json = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());

        List<SearchResult> results = [];

        foreach (JsonElement item in json.RootElement
            .GetProperty("value")
            .EnumerateArray())
        {
            SearchResult result = new()
            {
                Fields = item.EnumerateObject()
            .ToDictionary(
                property => property.Name,
                property => property.Value.ToString())
            };

            results.Add(result);
        }

        return [.. results];
    }
}

   
