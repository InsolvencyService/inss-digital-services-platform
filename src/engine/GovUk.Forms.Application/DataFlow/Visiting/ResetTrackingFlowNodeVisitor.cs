namespace GovUk.Forms.Application.DataFlow.Visiting;

public sealed class ResetTrackingFlowNodeVisitor : IFlowNodeVisitor
{
    public ValueTask VisitAsync(FlowNodeContext context)
    {
        context.Section.VisitedNodes = [];
        return ValueTask.CompletedTask;
    }
}