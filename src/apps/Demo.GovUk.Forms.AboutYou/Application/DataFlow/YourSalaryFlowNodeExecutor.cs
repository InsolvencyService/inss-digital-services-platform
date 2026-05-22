using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;

namespace Demo.GovUk.Forms.AboutYou.Application.DataFlow;

public sealed class YourSalaryFlowNodeExecutor : IFlowNodeExecutor
{
    private const int EqualToOrAbove10000NodeIndex = 0;
    private const int Below10000NodeIndex = 1;
    
    public ValueTask<NodeId?> ExecuteAsync(FlowNodeContext context)
    {
        SalaryModel salary = context.CurrentPage.As<SalaryModel>();
        return ValueTask.FromResult<NodeId?>(salary.Value >= 10_000 
            ? context.CurrentNode.NextNodes[EqualToOrAbove10000NodeIndex] 
            : context.CurrentNode.NextNodes[Below10000NodeIndex]);
    }
}