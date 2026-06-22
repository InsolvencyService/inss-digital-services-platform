using GovUk.Forms.Application.Services.Search;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.DataFlow.Executing;

public sealed class SearchFlowNodeExecutor : IFlowNodeExecutor
{
    private readonly ISearchService _searchService;

    public SearchFlowNodeExecutor(ISearchService searchService)
    {
        _searchService = searchService;
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

        SearchResult[] results = await _searchService.SearchAsync(search.SearchText);

        SearchModel pageSearch = context.Section.Pages.GetFirstOf<SearchModel>();
        pageSearch.SearchText = search.SearchText;
        pageSearch.ResultColumns = search.ResultColumns;
        pageSearch.Results = results;

        return context.Nodes[0].Id;



        //foreach (PageModel page in context.Section.Pages)
        //{
        //    //SearchModel pageSearch = page.As<SearchModel>();

        //    if (page is SearchModel pageSearch) { 

        //    pageSearch.SearchText = search.SearchText;
        //    pageSearch.ResultColumns = search.ResultColumns;
        //    pageSearch.Results = results;
        //    }
        //}

        // var searched = context.Section.As<SearchResult>();
        //search.Results = results;

        //search.ResultColumns ??= [];
        //search.CopyTo(context.CurrentPage);

        //if(results.Length >0)
        //{
        //    return context.Nodes[1].Id;
        //}
        //search.ResultColumns ??= [];
        //return ValueTask.FromResult<NodeId?>(context.Nodes[0].Id).Result;
        //return context.CurrentNode.Id;
    }
}