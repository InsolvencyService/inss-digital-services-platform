using System.Text.Json;
using GovUk.Forms.Application.Providers;
using GovUk.Forms.Domain.Search;
using GovUk.Forms.Infrastructure.Extensions;
using Microsoft.Extensions.Logging;

namespace GovUk.Forms.Infrastructure.Providers;

public sealed class SearchConfigProvider : ISearchConfigProvider
{
    private readonly string _configFile;
    private readonly ILogger<SearchConfigProvider> _logger;
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };
    
    public SearchConfigProvider(string configFile, ILogger<SearchConfigProvider> logger)
    {
        _configFile = configFile;
        _logger = logger;
    }

    public SearchModel LoadConfig()
    {
        string json = File.ReadAllText(_configFile);

        SearchModel? searchConfig = JsonSerializer.Deserialize<SearchModel>(json, _jsonOptions);

        if (searchConfig is null)
        {
            _logger.SearchConfigMissing(_configFile);
            return new SearchModel(); // TODO: Throw and also check file?
        }

        return searchConfig;
    }
}
