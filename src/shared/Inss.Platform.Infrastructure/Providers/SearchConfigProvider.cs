using System.Text.Json;
using System.Text.Json.Serialization;
using Inss.Platform.Application.Providers;
using Inss.Platform.Domain.Components.Searching;
using Inss.Platform.Infrastructure.Extensions;
using Microsoft.Extensions.Logging;

namespace Inss.Platform.Infrastructure.Providers;

public class SearchConfigProvider : ISearchConfigProvider
{
    private readonly string _configFile;
    private readonly ILogger<SearchConfigProvider> _logger;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true, 
        Converters = { new JsonStringEnumConverter() }
    };
    
    public SearchConfigProvider(string configFile, ILogger<SearchConfigProvider> logger)
    {
        _configFile = configFile;
        _logger = logger;
    }

    public SearchDefinition LoadConfig()
    {
        string configFilePath = Path.IsPathRooted(_configFile)
            ? _configFile
            : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _configFile);
        string json = File.ReadAllText(configFilePath);

        SearchDefinition? definition = JsonSerializer.Deserialize<SearchDefinition>(json, _jsonOptions);

        if (definition is null)
        {
            _logger.SearchConfigMissing(_configFile);
            return new SearchDefinition();
        }

        return definition;
    }
}