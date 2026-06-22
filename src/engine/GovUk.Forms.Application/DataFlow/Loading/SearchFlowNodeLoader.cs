using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;
using System.Text.Json;

namespace GovUk.Forms.Application.DataFlow.Loading;

public sealed class SearchFlowNodeLoader : IFlowNodeLoader
{
    //private readonly IConfiguration _configuration;

    //public SearchFlowNodeLoader(ISearchConfigProvider configuration)
    //{
    //    _configuration = configuration;
    //}

    public ValueTask<NodeId?> LoadAsync(FlowNodeContext context)
    {
        SearchModel search = context.CurrentPage.As<SearchModel>();
        search.CurrentResult = null;

        // TODO: Handle result detail by setting it on the search model..
        SearchModel searchConfig = LoadSearchConfig();
        search.ResultColumns = searchConfig.ResultColumns;
        
        search.PageSize = searchConfig.PageSize;
        search.DisplayAsTable = searchConfig.DisplayAsTable;
        

        // Unable to set properties due to the init status on the property...
        //SearchModel = new()
        //{
        //    DisplayAsTable = searchConfig.DisplayAsTable,
        //    ResultColumns = searchConfig.ResultColumns,
        //    PageSize = searchConfig.PageSize
        //};


        if (context.State is not null)
        {
            // Find result and set to search.CurrentResult
           // search.DisplayAsTable = searchCon
        }

        // The context has a state with will be the Id for the result so you can find it and set the CurrentResult
        return new ValueTask<NodeId?>((NodeId?)null);
    }



    //TODO : Maybe move this helper to a service class that can be injected into the loader.
    private static SearchModel LoadSearchConfig()
    {
        // Load the search configuration from a JSON file or other source
        string path = Path.Combine(
            AppContext.BaseDirectory,
            "App",
            "Search",
            "FindPersonConfig.json"); 

        string json = File.ReadAllText(path);

        SearchModel? searchConfig = JsonSerializer.Deserialize<SearchModel>(
            json,
            _jsonOptions);

            return searchConfig ?? new SearchModel();
    }

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
}