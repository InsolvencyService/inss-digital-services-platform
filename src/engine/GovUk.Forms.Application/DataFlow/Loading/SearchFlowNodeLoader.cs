using GovUk.Forms.Application.Providers;
using GovUk.Forms.Application.Services;
using GovUk.Forms.Domain.Primitives;
using GovUk.Forms.Domain.Search;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GovUk.Forms.Application.DataFlow.Loading;

public sealed class SearchFlowNodeLoader : IFlowNodeLoader
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SearchFlowNodeLoader> _logger;
    
    public SearchFlowNodeLoader(IServiceProvider serviceProvider, ILogger<SearchFlowNodeLoader> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async ValueTask<NodeId?> LoadAsync(FlowNodeContext context)
    {
        SearchModel search = context.CurrentPage.As<SearchModel>();
        search.CurrentResult = null;

        ISearchConfigProvider searchConfigProvider = _serviceProvider.GetRequiredKeyedService<ISearchConfigProvider>(search.ConfigKey);
        SearchModel config = searchConfigProvider.LoadConfig(); 

        // Handle result detail by setting it on the search model
        search.ResultColumns = config.ResultColumns;
        search.PageSize = config.PageSize;
        search.DisplayAsTable = config.DisplayAsTable;
     
        // Check if columns in config
        CheckAndLogConfigurationFiles(search);

        string? searchText = context.GetQueryParam<string>("searchText");

        int currentPageNumber = context.GetQueryParam<int>("currentPageNumber");
        
        if (currentPageNumber < 1)
        {
            currentPageNumber = 1;
        }
        
        search.CurrentPageNumber = currentPageNumber;
        
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            SearchRequest request = new() 
            { 
                SearchText = searchText,
                PageSize = search.PageSize,
                CurrentPageNumber = search.CurrentPageNumber
            };
            
            ISearchService searchService = _serviceProvider.GetRequiredKeyedService<ISearchService>(search.ConfigKey);
            SearchResponse response = await searchService.SearchAsync(request);

            search.SearchText = searchText;
            search.Results = response.Results;
            search.TotalResults = response.TotalResults;
            search.TotalPages = (int)Math.Ceiling((double)search.TotalResults / search.PageSize);
        }
        
        // The context has a state with will be the Id for the result so you can find it and set the CurrentResult
        return context.Nodes[0].Id;
    }


    private void CheckAndLogConfigurationFiles(SearchModel search)
    {
        foreach (SearchResult result in search.Results)
        {
            foreach (SearchResultColumn column in search.ResultColumns)
            {
                if (!result.Fields.ContainsKey(column.Name))
                {
                    _logger.LogWarning("Unable to find column  Azure search field '{FieldName}'.", column.Name);
                }
            }
        }
    }
}