using Demo.GovUk.Forms.AboutYou.Domain;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Domain.Primitives;

namespace Demo.GovUk.Forms.AboutYou.Application.DataFlow;

public sealed class OwnHomeFlowNodeExecutor : IFlowNodeExecutor
{
    private const int OwnHomeNodeIndex = 0;
    private const int NotOwnHomeNodeIndex = 1;
    
    public ValueTask<NodeId?> ExecuteAsync(ExecuteContext context)
    {
        OwnHomeModel ownHome = context.UpdatedPage.As<OwnHomeModel>();
        return ValueTask.FromResult<NodeId?>(ownHome.OwnsHome 
            ? context.CurrentNode.NextNodes[OwnHomeNodeIndex] 
            : context.CurrentNode.NextNodes[NotOwnHomeNodeIndex]);
    }
}