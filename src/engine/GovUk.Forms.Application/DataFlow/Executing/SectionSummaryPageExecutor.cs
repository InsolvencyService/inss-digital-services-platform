using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.DataFlow.Executing;

public sealed class SectionSummaryFlowNodeExecutor : IFlowNodeExecutor
{
    public ValueTask<NodeId?> ExecuteAsync(FlowNodeContext context)
    {
        context.Section.SetCompleted();
        return ValueTask.FromResult<NodeId?>(null);
    }
}

public sealed class SectionSummaryPageExecutor : IPageExecutor
{
    public ValueTask ExecuteAsync(ExecutePageContext context)
    {
        context.Section.SetCompleted();
        return ValueTask.CompletedTask;
    }
}