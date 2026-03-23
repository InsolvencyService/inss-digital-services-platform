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
    
    public ValueTask<NodeId?> ExecuteAsync(ExecuteContext context)
    {
        return ValueTask.FromResult<NodeId?>(context.CurrentNode.NextNodes[0]);
    }
}