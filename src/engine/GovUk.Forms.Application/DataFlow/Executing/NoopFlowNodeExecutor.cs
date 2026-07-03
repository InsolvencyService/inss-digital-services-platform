using System.Diagnostics.CodeAnalysis;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.DataFlow.Executing;

[ExcludeFromCodeCoverage]
public sealed class NoopFlowNodeExecutor : IFlowNodeExecutor
{
    public static readonly IFlowNodeExecutor Default = new NoopFlowNodeExecutor();
    
    private NoopFlowNodeExecutor()
    {
    }
    
    public ValueTask<NodeId?> ExecuteAsync(FlowNodeContext context)
    {
        return ValueTask.FromResult<NodeId?>(context.CurrentNode.NextNodes[0]);
    }
}

[ExcludeFromCodeCoverage]
public sealed class NoopPageExecutor : IPageExecutor
{
    public static readonly IPageExecutor Default = new NoopPageExecutor();
    
    private NoopPageExecutor()
    {
    }
    
    public ValueTask ExecuteAsync(ExecutePageContext context)
    {
        context.ChildNodeIndex = 0;
        return ValueTask.CompletedTask;
    }
}