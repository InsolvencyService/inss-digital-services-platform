using GovUk.Forms.Application.Providers;
using GovUk.Forms.Application.Services;
using GovUk.Forms.Domain.Primitives;
using GovUk.Forms.Domain.Search;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GovUk.Forms.Application.DataFlow.Executing;

public sealed class SearchFlowNodeExecutor : IFlowNodeExecutor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SearchFlowNodeExecutor> _logger;

    public SearchFlowNodeExecutor(IServiceProvider serviceProvider, ILogger<SearchFlowNodeExecutor> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    
    public async ValueTask<NodeId?> ExecuteAsync(FlowNodeContext context)
    {
        SearchModel search = context.CurrentPage.As<SearchModel>();

        if (string.IsNullOrWhiteSpace(search.SearchText))
        {
            _logger.LogWarning("Search Text is missing.");
            return context.Nodes[0].Id;
        }

        ISearchConfigProvider searchConfigProvider = _serviceProvider.GetRequiredKeyedService<ISearchConfigProvider>(search.ConfigKey);
        
        SearchModel config = searchConfigProvider.LoadConfig(); 

        if (config.PageSize <= 0)
        {
            // Invalid page size in the config....
            _logger.LogError("Page size is required.");
            return context.Nodes[0].Id;
        }
        
        search.PageSize = config.PageSize;

        if (search.CurrentPageNumber < 1)
        {
            search.CurrentPageNumber = 1;
        }

        ISearchService searchService = _serviceProvider.GetRequiredKeyedService<ISearchService>(search.ConfigKey);
        SearchRequest request = new() 
        { 
            SearchText = search.SearchText, 
            PageSize = search.PageSize, 
            CurrentPageNumber = search.CurrentPageNumber
        };
        SearchResponse response = await searchService.SearchAsync(request);
        
        SearchModel pageSearch = context.Section.Pages.GetFirstOf<SearchModel>();
        pageSearch.SearchText = search.SearchText;
        pageSearch.ResultColumns = search.ResultColumns;
        pageSearch.CurrentPageNumber = search.CurrentPageNumber;
        pageSearch.PageSize = search.PageSize;
        pageSearch.Results = response.Results;
        pageSearch.TotalResults = response.TotalResults;
        pageSearch.TotalPages = (int)Math.Ceiling((double)pageSearch.TotalResults / search.PageSize);

        return context.Nodes[0].Id;
    }
}