using GovUk.Forms.Domain.Primitives;
using GovUk.Forms.Domain.Search;

namespace GovUk.Forms.Application.DataFlow.Executing;

public sealed class SearchTermFlowNodeExecutor : IFlowNodeExecutor
{
    public ValueTask<NodeId?> ExecuteAsync(FlowNodeContext context)
    {
        SearchTermModel searchTerm = context.CurrentPage.As<SearchTermModel>();

        context.QueryParams["keyword"] = searchTerm.SearchText;

        return ValueTask.FromResult<NodeId?>(context.CurrentNode.NextNodes[0]);
    }
}