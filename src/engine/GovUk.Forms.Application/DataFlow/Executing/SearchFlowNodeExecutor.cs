using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.DataFlow.Executing;

public sealed class SearchFlowNodeExecutor : IFlowNodeExecutor
{
    public ValueTask<NodeId?> ExecuteAsync(FlowNodeContext context)
    {
        SearchModel search = context.CurrentPage.As<SearchModel>();

        // TODO: Inject service to use search text and get results. If we have results and are executing then we want to continue
        
        return ValueTask.FromResult<NodeId?>(context.Nodes[0].Id);
    }
}