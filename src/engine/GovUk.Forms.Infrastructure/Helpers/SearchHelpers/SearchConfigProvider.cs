using GovUk.Forms.Application.Services.Search;
using GovUk.Forms.Domain;
using System.Text.Json;

namespace GovUk.Forms.Infrastructure.Helpers.SearchHelpers;

public sealed class SearchConfigProvider : ISearchConfigProvider
{
    public SearchModel LoadConfig(string filename)
    {
        // Load the search configuration from a JSON file or other source
        string path = Path.Combine(
            AppContext.BaseDirectory,
            "App",
            "Search",
            filename);

        string json = File.ReadAllText(path);

        SearchModel? searchConfig = JsonSerializer.Deserialize<SearchModel>(
            json,
            _jsonOptions);

        return searchConfig ?? new SearchModel();
    }

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
}
