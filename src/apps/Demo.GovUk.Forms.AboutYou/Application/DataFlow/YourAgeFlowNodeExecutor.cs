using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;

namespace Demo.GovUk.Forms.AboutYou.Application.DataFlow;

public sealed class YourAgeFlowNodeExecutor : IFlowNodeExecutor
{
    private const int EqualToOrAbove18NodeIndex = 0;
    private const int Below18NodeIndex = 1;
    
    public ValueTask<NodeId?> ExecuteAsync(ExecuteContext context)
    {
        AgeModel age = context.UpdatedPage.As<AgeModel>();
        return ValueTask.FromResult<NodeId?>(age.Value >= 18 
            ? context.CurrentNode.NextNodes[EqualToOrAbove18NodeIndex] 
            : context.CurrentNode.NextNodes[Below18NodeIndex]);
    }
}