using GovUk.Forms.Application.Extensions;
using GovUk.Forms.Application.Providers;
using GovUk.Forms.Application.Services;
using GovUk.Forms.Domain.Primitives;
using GovUk.Forms.Domain.Search;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GovUk.Forms.Application.DataFlow.Loading;

public sealed class SearchResultFlowNodeLoader : IFlowNodeLoader
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SearchResultFlowNodeLoader> _logger;
    private const int SearchDetailIndex = 1;
    
    public SearchResultFlowNodeLoader(IServiceProvider serviceProvider, ILogger<SearchResultFlowNodeLoader> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async ValueTask<NodeId?> LoadAsync(FlowNodeContext context)
    {
        SearchResultModel searchResult = context.CurrentPage.As<SearchResultModel>();
        searchResult.ClearValues();
        
        // Load and check if columns in config
        ISearchConfigProvider searchConfigProvider = _serviceProvider.GetRequiredKeyedService<ISearchConfigProvider>(searchResult.ConfigKey);
        searchResult.Definition = searchConfigProvider.LoadConfig();
        CheckAndLogConfigurationFiles(searchResult);
        
        // Update the result detail path so the row links for each result can be built 
        SearchResultDetailModel searchResultDetail = context.Section.Pages.GetFirstOf<SearchResultDetailModel>();
        searchResult.ResultDetailPath = searchResultDetail.Path;
        
        // Link the search detail to the correct node Id as this is a spur page that is accessed without a call to action
        searchResultDetail.LinkedToNode = context.CurrentNode.NextNodes[SearchDetailIndex];
        
        // Get the requested search query params
        string? searchText = context.GetQueryParam<string>("keyword");
        int currentPageNumber = context.GetQueryParam<int>("currentPageNumber");
        
        if (currentPageNumber < 1)
        {
            currentPageNumber = 1;
        }
        
        searchResult.CurrentPageNumber = currentPageNumber;
        
        // If we have a search term then do a search else we will present no results to the user 
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            SearchRequest request = new() 
            {
                SearchText = AddingWildCard(searchText),
                PageSize = searchResult.Definition.PageSize, 
                CurrentPageNumber = searchResult.CurrentPageNumber
            };
            
            ISearchService searchService = _serviceProvider.GetRequiredKeyedService<ISearchService>(searchResult.ConfigKey);
            SearchResponse response = await searchService.SearchAsync(request);

            searchResult.SearchText = searchText;
            searchResult.Results = response.Results;
            searchResult.TotalResults = response.TotalResults;
            searchResult.TotalPages = (int)Math.Ceiling((double)searchResult.TotalResults / searchResult.Definition.PageSize);
        }
        
        return null;
    }

    private void CheckAndLogConfigurationFiles(SearchResultModel searchResult)
    {
        foreach (SearchResult result in searchResult.Results)
        {
            foreach (KeyValuePair<string, string> column in result.Fields)
            {
                SearchResultDefinition? definition = searchResult.Definition.Results.FirstOrDefault(dr => dr.Names.Contains(column.Key));

                if (definition is null)
                {
                    _logger.SearchConfigAndResultMismatch(column.Key);
                }
            }
        }
    }

    private static string AddingWildCard(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return searchText;
        }

        string[] words = searchText.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        List<string> wildcardWords = new();
        foreach (string word in words)
        {
            wildcardWords.Add($"{word}*");
        }

        return string.Join(" ", wildcardWords);
    }
}