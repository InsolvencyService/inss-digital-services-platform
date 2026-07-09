using GovUk.Forms.Domain.Primitives;
using GovUk.Forms.Domain.Search;

namespace GovUk.Forms.Application.DataFlow.Executing;

public sealed class SearchResultFlowNodeExecutor : IFlowNodeExecutor
{
    public async ValueTask<NodeId?> ExecuteAsync(FlowNodeContext context)
    {
        SearchResultModel searchResult = context.CurrentPage.As<SearchResultModel>();
        context.AddQueryParam("keyword", searchResult.SearchText);
        return await ValueTask.FromResult<NodeId?>(context.CurrentNode.NextNodes[0]);
    }
}