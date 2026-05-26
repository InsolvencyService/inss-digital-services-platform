namespace GovUk.Forms.Application.DataFlow.Visiting;

public interface IFlowNodeVisitor
{
    ValueTask VisitAsync(FlowNodeContext context);
}