namespace GovUk.Forms.Application.DataFlow.Visiting;

public sealed class DefaultFlowNodeVisitor : IFlowNodeVisitor
{
    public ValueTask VisitAsync(FlowNodeContext context)
    {
        context.Section.Track(context.CurrentNode.Id);
        return ValueTask.CompletedTask;
    }
}