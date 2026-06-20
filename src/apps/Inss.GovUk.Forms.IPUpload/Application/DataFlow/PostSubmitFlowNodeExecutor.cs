using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Domain.Primitives;

namespace Inss.GovUk.Forms.IPUpload.Application.DataFlow;

public sealed class PostSubmitFlowNodeExecutor : IFlowNodeExecutor
{
    private const int DeclarationIndex = 0;
    
    public ValueTask<NodeId?> ExecuteAsync(FlowNodeContext context)
    {
        context.Section.VisitedNodes = [];
        return ValueTask.FromResult<NodeId?>(context.CurrentNode.NextNodes[DeclarationIndex]);
    }
}