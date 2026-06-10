using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.DataFlow.Executing;

public sealed class SearchFlowNodeExecutor : IFlowNodeExecutor
{
    public SearchFlowNodeExecutor()
    {
        
    }
    
    public ValueTask<NodeId?> ExecuteAsync(FlowNodeContext context)
    {
        SearchModel search = context.CurrentPage.As<SearchModel>();

        // TODO: Inject service to use search text and get results. If we have results and are executing then we want to continue
        
        // If results exist then goto next node (index 1)
        return ValueTask.FromResult<NodeId?>(context.Nodes[0].Id);
    }
}