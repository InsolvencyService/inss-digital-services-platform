using GovUk.Forms.Application.Services;
using Inss.Platform.Application.Extensions;
using Inss.Platform.Application.Providers;
using Inss.Platform.Domain.Components;
using Inss.Platform.Domain.Components.Searching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Inss.Platform.Application.Loaders;

public sealed class SearchResultComponentLoader : IComponentLoader
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SearchResultComponentLoader> _logger;

    public SearchResultComponentLoader(IServiceProvider serviceProvider, ILogger<SearchResultComponentLoader> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    
    public async ValueTask LoadAsync(ComponentModel component, QueryParamList queryParams)
    {
        SearchResultComponentModel searchResult = component.As<SearchResultComponentModel>();
        
        // Load and check if columns in config
        ISearchConfigProvider searchConfigProvider = _serviceProvider.GetRequiredKeyedService<ISearchConfigProvider>(searchResult.ConfigKey);
        searchResult.Definition = searchConfigProvider.LoadConfig();
        CheckAndLogConfigurationFiles(searchResult);
        
        // Update the result detail path so the row links for each result can be built 
        //SearchResultDetailModel searchResultDetail = context.Section.Pages.GetFirstOf<SearchResultDetailModel>();
        //searchResult.ResultDetailPath = searchResultDetail.Path;
        
        // Get the requested search query params
        string? searchText = queryParams.GetQueryParam<string>("keyword");
        int currentPageNumber = queryParams.GetQueryParam<int>("currentPageNumber");
        
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

            searchResult.Value = searchText;
            searchResult.Results = response.Results;
            searchResult.TotalResults = response.TotalResults;
            searchResult.TotalPages = (int)Math.Ceiling((double)searchResult.TotalResults / searchResult.Definition.PageSize);
        }
    }
    
    private void CheckAndLogConfigurationFiles(SearchResultComponentModel searchResult)
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
            string wildcardWord = word.EndsWith('*') 
                ? word 
                : string.Concat(word, "*");

            wildcardWords.Add(wildcardWord);
        }

        return string.Join(" ", wildcardWords);
    }
}