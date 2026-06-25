using GovUk.Forms.Application.Services.Search;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.DataFlow.Executing;

public sealed class SearchFlowNodeExecutor : IFlowNodeExecutor
{
    private readonly ISearchService _searchService;
    private readonly ISearchConfigProvider _configSettings;

    public SearchFlowNodeExecutor(ISearchService searchService, ISearchConfigProvider configSettings)
    {
        _searchService = searchService;
        _configSettings = configSettings;
    }
    
    public async ValueTask<NodeId?> ExecuteAsync(FlowNodeContext context)
    {
        SearchModel search = context.CurrentPage.As<SearchModel>();

        if (string.IsNullOrWhiteSpace(search.SearchText))
        {
            // TODO: Inject service to use search text and get results. If we have results and are executing then we want to continue
            //search.Results = [new SearchResult { Id = "1" }, new SearchResult { Id = "2" }];

            // If results exist then goto next node (index 1)
            //return ValueTask.FromResult<NodeId?>(context.Nodes[0].Id).Result;
            return context.Nodes[0].Id;
        }

        // Add the initial current page number, unable to retrieve it from the loader...
        SearchModel config = _configSettings.LoadConfig("FindPersonConfig.json"); //SearchModel searchConfig = _configSettings.LoadConfig("FindPersonConfig.json");
        search.PageSize = config.PageSize;

        if (search.CurrentPageNumber < 1)
        {
            search.CurrentPageNumber = 1;
        }

        if (search.CurrentPageNumber < 1)
        {
            search.PageSize = config.PageSize;
        }

        //SearchResult[] results = await _searchService.SearchAsync(search.SearchText, search.PageSize, search.CurrentPageNumber);
        SearchResponse response = await _searchService.SearchAsync(search.SearchText, search.PageSize, search.CurrentPageNumber);
        
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