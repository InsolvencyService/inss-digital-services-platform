using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Domain.Primitives;
using Inss.GovUk.Forms.IPUpload.Domain;

namespace Inss.GovUk.Forms.IPUpload.Application.DataFlow;

public sealed class EmployerDetailsFlowNodeExecutor : IFlowNodeExecutor
{
    private const int CaseRefNumNodeIdIndex = 0;
    private const int NextPageNodeIdIndex = 1;

    public ValueTask<NodeId?> ExecuteAsync(FlowNodeContext context)
    {
        EmployerDetailsModel employerDetails = context.CurrentPage.As<EmployerDetailsModel>();

        if (employerDetails.DetailsMatch)
        {
            return ValueTask.FromResult<NodeId?>(context.CurrentNode.NextNodes[NextPageNodeIdIndex]);
        }

        // If we are changing the case reference then we need to reset the data we have collected for the user as the upload will have
        // to be re-validated

        employerDetails.ReturnUrl = null;
        
        IPUploadDeclarationModel declaration = context.Section.Pages.GetFirstOf<IPUploadDeclarationModel>();
        context.Section.ResetVisitedNodesFrom(declaration.LinkedToNode);
        
        IPUploadXmlErrorsModel uploadErrors = context.Section.Pages.GetFirstOf<IPUploadXmlErrorsModel>();
        uploadErrors.ClearErrors();
        
        XmlFileUploadModel fileUpload = context.Section.Pages.GetFirstOf<XmlFileUploadModel>();
        fileUpload.ClearValues();
        
        return ValueTask.FromResult<NodeId?>(context.CurrentNode.NextNodes[CaseRefNumNodeIdIndex]);
    }
}