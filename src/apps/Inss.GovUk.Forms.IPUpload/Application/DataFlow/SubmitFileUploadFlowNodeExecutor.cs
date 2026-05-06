using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Domain.Primitives;
using Inss.GovUk.Forms.IPUpload.Application.Clients;

namespace Inss.GovUk.Forms.IPUpload.Application.DataFlow;

public sealed class SubmitFileUploadFlowNodeExecutor : IFlowNodeExecutor
{
    private readonly ISubmitIPUploadSectionClient _submitSectionClient;
    private const int PostSubmitIndex = 0;
    
    public SubmitFileUploadFlowNodeExecutor(ISubmitIPUploadSectionClient submitSectionClient)
    {
        _submitSectionClient = submitSectionClient;
    }
    
    public async ValueTask<NodeId?> ExecuteAsync(ExecuteContext context)
    {
        if (context.FinalExecuteStep)
        {
            context.Section.SetCompleted();
            await _submitSectionClient.SubmitAsync(context.Section, context.Form.Id);
        }
        
        return context.CurrentNode.NextNodes[PostSubmitIndex];
    }
}