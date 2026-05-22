namespace GovUk.Forms.Application.DataFlow.Providing;

public interface IFlowNodePreviousPathProvider
{
    ValueTask UpdateAsync(FlowNodeContext context);
}