using GovUk.Forms.Application.Services.Search;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Microsoft.Extensions.Logging;

namespace GovUk.Forms.Application.DataFlow.Executing;

public sealed class SearchFlowNodeExecutor : IFlowNodeExecutor
{
    private readonly ISearchService _searchService;
    private readonly ISearchConfigProvider _configSettings;
    private readonly ILogger<SearchFlowNodeExecutor> _logger;

    public SearchFlowNodeExecutor(ISearchService searchService, ISearchConfigProvider configSettings, ILogger<SearchFlowNodeExecutor> logger)
    {
        _searchService = searchService;
        _configSettings = configSettings;
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

        // Add the initial current page number, unable to retrieve it from the loader...
        SearchModel config = _configSettings.LoadConfig("FindPersonConfig.json"); 

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

        if (search.CurrentPageNumber < 1)
        {
            search.PageSize = config.PageSize;
        }

        SearchResponse response = await _searchService.SearchAsync(
            search.SearchText, 
            search.PageSize, 
            search.CurrentPageNumber);
        
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