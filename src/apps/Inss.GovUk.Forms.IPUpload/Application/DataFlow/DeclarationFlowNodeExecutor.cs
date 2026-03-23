using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Domain.Primitives;
using Inss.GovUk.Forms.IPUpload.Domain;

namespace Inss.GovUk.Forms.IPUpload.Application.DataFlow;

public sealed class DeclarationFlowNodeExecutor : IFlowNodeExecutor
{
    public ValueTask<NodeId?> ExecuteAsync(ExecuteContext context)
    {
        // Find the target page and enforce acceptance of the declaration
        IPUploadDeclarationModel declaration = context.Section.Pages.GetPage(context.UpdatedPage.Path).As<IPUploadDeclarationModel>();
        declaration.Accepted = true;
        return ValueTask.FromResult<NodeId?>(context.CurrentNode.NextNodes[0]);
    }
}