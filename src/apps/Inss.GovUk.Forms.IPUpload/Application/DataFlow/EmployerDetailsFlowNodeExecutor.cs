using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Domain.Primitives;
using Inss.GovUk.Forms.IPUpload.Domain;

namespace Inss.GovUk.Forms.IPUpload.Application.DataFlow;

public sealed class EmployerDetailsFlowNodeExecutor : IFlowNodeExecutor
{
    private const int FirstWorkingPageNodeIdIndex = 0;
    private const int NextPageNodeIdIndex = 1;

    public ValueTask<NodeId?> ExecuteAsync(FlowNodeContext context)
    {
        EmployerDetailsModel yesornoitem = context.CurrentPage.As<EmployerDetailsModel>();
        
        if (!yesornoitem.YesorNoItem)
        {
            return ValueTask.FromResult<NodeId?>(context.CurrentNode.NextNodes[FirstWorkingPageNodeIdIndex]);
        }

        return ValueTask.FromResult<NodeId?>(context.CurrentNode.NextNodes[NextPageNodeIdIndex]);
    }
}