using GovUk.Forms.Application.Services.Search;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Text.Json;

namespace GovUk.Forms.Application.DataFlow.Loading;

public sealed class SearchFlowNodeLoader : IFlowNodeLoader
{
    private readonly ISearchConfigProvider _configSettings;
    private readonly ILogger<SearchFlowNodeLoader> _logger;


    public SearchFlowNodeLoader(ILogger<SearchFlowNodeLoader> logger, ISearchConfigProvider configSettings)
    {
        _configSettings = configSettings;
        _logger = logger;
    }

    public ValueTask<NodeId?> LoadAsync(FlowNodeContext context)
    {
        SearchModel search = context.CurrentPage.As<SearchModel>();
        search.CurrentResult = null;

        // Get configuration settings
        SearchModel searchConfig = _configSettings.LoadConfig("FindPersonConfig.json");

        // Handle result detail by setting it on the search model..
        search.ResultColumns = searchConfig.ResultColumns;
        search.PageSize = searchConfig.PageSize;
        search.DisplayAsTable = searchConfig.DisplayAsTable;
        search.HasNextPage = search.Results.Length == search.PageSize;

        CheckAndLogConfiguratonFiles(search);


        // if (context.State is not null)
        // {
        //
        // }

        // The context has a state with will be the Id for the result so you can find it and set the CurrentResult
        return new ValueTask<NodeId?>((NodeId?)null);
    }


    private void CheckAndLogConfiguratonFiles(SearchModel search)
    {
        // Check if column are within the azure search - if not log warning.
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