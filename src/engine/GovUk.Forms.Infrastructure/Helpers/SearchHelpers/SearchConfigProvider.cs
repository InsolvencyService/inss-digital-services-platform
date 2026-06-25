using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Application.Services.Search;
using GovUk.Forms.Domain;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace GovUk.Forms.Infrastructure.Helpers.SearchHelpers;

public sealed class SearchConfigProvider : ISearchConfigProvider
{
    private readonly ILogger<SearchConfigProvider> _logger;
    public SearchConfigProvider(ILogger<SearchConfigProvider> logger)
    {
        _logger = logger; ;
    }

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

            if (searchConfig == null)
            {
                _logger.LogError("Unable to load configuration file '{FileName}'.", filename);
                return new SearchModel();
            }

            return searchConfig;
    }

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
}
