using GovUk.Forms.Domain.Primitives;
using GovUk.Forms.Domain.Search;

namespace GovUk.Forms.Application.DataFlow.Executing;

// TODO: Rename to SearchResultFlowNodeExecutor

public sealed class SearchFlowNodeExecutor : IFlowNodeExecutor
{
    public async ValueTask<NodeId?> ExecuteAsync(FlowNodeContext context)
    {
        SearchModel search = context.CurrentPage.As<SearchModel>();

        context.QueryParams["keyword"] = search.SearchText;

        return await ValueTask.FromResult<NodeId?>(context.CurrentNode.NextNodes[0]);
    }
}