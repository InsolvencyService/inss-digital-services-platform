using GovUk.Forms.Application.Services.Search;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Microsoft.Extensions.Logging;

namespace GovUk.Forms.Application.DataFlow.Loading;

public sealed class SearchFlowNodeLoader : IFlowNodeLoader
{
    private readonly ISearchConfigProvider _configSettings;
    private readonly ILogger<SearchFlowNodeLoader> _logger;
    private readonly ISearchService _searchService;


    public SearchFlowNodeLoader(
        ILogger<SearchFlowNodeLoader> logger,
        ISearchConfigProvider configSettings,
        ISearchService searchService)
    {
        _configSettings = configSettings;
        _logger = logger;
        _searchService = searchService;
    }

    public async ValueTask<NodeId?> LoadAsync(FlowNodeContext context)
    {
        SearchModel search = context.CurrentPage.As<SearchModel>();
        search.CurrentResult = null;

        // Get configuration settings...
        SearchModel searchConfig = _configSettings.LoadConfig("FindPersonConfig.json");

        // Handle result detail by setting it on the search model..
        search.ResultColumns = searchConfig.ResultColumns;
        search.PageSize = searchConfig.PageSize;
        search.DisplayAsTable = searchConfig.DisplayAsTable;
     
        // Check if columns in config...
        CheckAndLogConfiguratonFiles(search);

        string? searchText = context.GetQueryParam<string>("searchText");

        int currentPageNumber = context.GetQueryParam<int>("currentPageNumber");
        if (currentPageNumber < 1)
        {
            currentPageNumber = 1;
        }
        search.CurrentPageNumber = currentPageNumber;


        if (!string.IsNullOrWhiteSpace(searchText))
        {
            SearchResponse response = await
                _searchService.SearchAsync(
                    searchText, 
                    search.PageSize, 
                    search.CurrentPageNumber);

            search.SearchText = searchText;
            search.Results = response.Results;
            search.TotalResults = response.TotalResults;
            search.TotalPages = (int)Math.Ceiling((double)search.TotalResults / search.PageSize);

        }

        // if (context.State is not null)
        // {
        //
        // }

        // The context has a state with will be the Id for the result so you can find it and set the CurrentResult
        //return new ValueTask<NodeId?>((NodeId?)null);
        return context.Nodes[0].Id;
    }


    private void CheckAndLogConfiguratonFiles(SearchModel search)
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