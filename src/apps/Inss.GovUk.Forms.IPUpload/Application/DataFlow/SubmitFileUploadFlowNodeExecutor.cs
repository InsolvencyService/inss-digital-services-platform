using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Domain.Primitives;
using Inss.GovUk.Forms.IPUpload.Application.Services;
using Inss.GovUk.Forms.IPUpload.Domain;

namespace Inss.GovUk.Forms.IPUpload.Application.DataFlow;

public sealed class SubmitFileUploadFlowNodeExecutor : IFlowNodeExecutor
{
    private readonly ISubmitUploadedXmlService _submitUploadedXmlService;
    private const int PostSubmitIndex = 0;
    
    public SubmitFileUploadFlowNodeExecutor(ISubmitUploadedXmlService submitUploadedXmlService)
    {
        _submitUploadedXmlService = submitUploadedXmlService;
    }
    
    public async ValueTask<NodeId?> ExecuteAsync(FlowNodeContext context)
    {
        context.Section.SetCompleted();
        string referenceNumber = await _submitUploadedXmlService.SubmitAsync(context.Section, context.Form.Id);

        PostSubmitModel postSubmit = context.Section.Pages.GetFirstOf<PostSubmitModel>();
        postSubmit.ReferenceNumber = referenceNumber;
        
        return context.CurrentNode.NextNodes[PostSubmitIndex];
    }
}