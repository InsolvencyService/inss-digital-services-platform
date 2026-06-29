using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Domain.Primitives;
using Inss.GovUk.Forms.IPUpload.Application.Exceptions;
using Inss.GovUk.Forms.IPUpload.Domain;

namespace Inss.GovUk.Forms.IPUpload.Application.DataFlow;

public sealed class CaseReferenceFlowNodeExecutor : IFlowNodeExecutor
{
    private const int EmployerNodeIndex = 0;
    
    public ValueTask<NodeId?> ExecuteAsync(FlowNodeContext context)
    {
        CheckCaseReferenceModel checkCaseReference = context.CurrentPage.As<CheckCaseReferenceModel>();
        string caseReference = checkCaseReference.CaseReference.Value;
        
        CheckCaseReferenceModel previousCheckCaseReference = context.PageBeforeChanges?.As<CheckCaseReferenceModel>() 
                                                          ?? throw new IPUploadException("Unable to get the page before edits.");
        string previousCaseReference = previousCheckCaseReference.CaseReference.Value;

        // If the page is being edited from the summary and the case has changed from what was saved then we need to force the user 
        // through the upload flow again
        if (previousCaseReference != caseReference && context.Section.ReturnUrl is not null)
        {
            // We don't want the user to return to the summary
            context.Section.ReturnUrl = null;
        }

        return ValueTask.FromResult<NodeId?>(context.CurrentNode.NextNodes[EmployerNodeIndex]);
    }
}