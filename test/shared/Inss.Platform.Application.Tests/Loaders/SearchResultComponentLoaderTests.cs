using System.Text.Json;
using System.Text.Json.Serialization;
using Inss.Platform.Application.Loaders;
using Inss.Platform.Application.Providers;
using Inss.Platform.Application.Services;
using Inss.Platform.Domain;
using Inss.Platform.Domain.Components;
using Inss.Platform.Domain.Components.Searching;
using Inss.Platform.Domain.Components.Searching.Support;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Inss.Platform.Application.Tests.Loaders;

public class SearchResultComponentLoaderTests
{
    private readonly SearchResultComponentLoader _searchResultComponentLoader;
    private readonly ISearchConfigProvider _searchConfigProvider;
    private readonly ISearchService _searchService;
    private readonly AppModel _app;
    private readonly PageModel _searchResultPage;
    private readonly SearchResultComponentModel _searchResultComponent;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true, 
        Converters = { new JsonStringEnumConverter() }
    };
    
    public SearchResultComponentLoaderTests()
    {
        ServiceCollection services = [];
        _app = CreateApp();
        _searchResultPage = _app.Pages.GetPage("/search-results");
        _searchResultComponent = _searchResultPage.Components.GetFirstOf<SearchResultComponentModel>();
        _searchConfigProvider = Substitute.For<ISearchConfigProvider>();
        _searchConfigProvider.LoadConfig().Returns(LoadSearchDefinition());
        services.AddKeyedSingleton<ISearchConfigProvider>(_searchResultComponent.ConfigKey, (_, _) => _searchConfigProvider);
        _searchService = Substitute.For<ISearchService>();
        _searchService.SearchAsync(Arg.Is<SearchRequest>(r => r.SearchText == "Nevada*")).Returns(new SearchResponse { Results = [] });
        services.AddKeyedSingleton<ISearchService>(_searchResultComponent.ConfigKey, (_, _) => _searchService);
        ILogger<SearchResultComponentLoader> logger = Substitute.For<ILogger<SearchResultComponentLoader>>();
        _searchResultComponentLoader = new SearchResultComponentLoader(services.BuildServiceProvider(), logger);
    }

    [Fact]
    public async Task HasExistingSearchInfoAndNoSearchKeyword_LoadAsync_ClearsFields()
    {
        _searchResultComponent.Value = "Springfield";
        _searchResultComponent.Results = [new SearchResult()];
        QueryParamList queryParams = CreateQueryParamList();
        LoaderContext context = new(_app, _searchResultPage, _searchResultComponent, queryParams);

        await _searchResultComponentLoader.LoadAsync(context);
        
        Assert.Equal("Nevada", _searchResultComponent.Value);
        Assert.Empty(_searchResultComponent.Results);
    }

    private static AppModel CreateApp()
    {
        return new AppModel
        {
            Session = "Test",
            Email = "homer@simpsons.com",
            Pages =
            [
                new PageModel
                {
                    Title = "Search",
                    Path = "/search",
                    Components =
                    [
                        new SearchTermComponentModel
                        {
                            Id = "/search/components/0",
                            Heading = "Find Ips",
                            Label = "Search",
                            Description = "Find IPs",
                            AssociatedPagePath = "/search"
                        }
                    ]
                },
                new PageModel
                {
                    Title = "Search results",
                    Path = "/search-results",
                    Components =
                    [
                        new SearchResultComponentModel
                        {
                            Id = "/search-results/components/0",
                            Label = "Search results",
                            ConfigKey = "ConfigKey",
                            AssociatedPagePath = "/search-results"
                        }
                    ]
                },
                new PageModel
                {
                    Title = "Search result detail",
                    Path = "/search-result-detail",
                    Components =
                    [
                        new SearchResultDetailComponentModel
                        {
                            Id = "/search-result-detail/components/0",
                            ConfigKey = "ConfigKey",
                            AssociatedPagePath = "/search-result-detail"
                        }
                    ]
                }
            ]
        };
    }
    
    private static SearchDefinition LoadSearchDefinition()
    {
        string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "FindIP.json");
        string json = File.ReadAllText(configFilePath);
        return JsonSerializer.Deserialize<SearchDefinition>(json, _jsonOptions)!;
    }

    private static QueryParamList CreateQueryParamList()
    {
        QueryParamList queryParams = [];
        queryParams.AddQueryParam("keyword", "Nevada");
        return queryParams;
    }
}