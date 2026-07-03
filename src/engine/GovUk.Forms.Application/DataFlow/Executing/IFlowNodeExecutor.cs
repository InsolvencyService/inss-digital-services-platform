using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.DataFlow.Executing;

public interface IFlowNodeExecutor
{
    ValueTask<NodeId?> ExecuteAsync(FlowNodeContext context);
}

public interface IPageExecutor
{
    ValueTask ExecuteAsync(ExecutePageContext context);
}