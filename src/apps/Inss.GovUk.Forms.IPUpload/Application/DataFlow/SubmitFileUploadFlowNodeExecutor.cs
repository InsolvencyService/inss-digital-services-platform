using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Domain.Primitives;
using Inss.GovUk.Forms.IPUpload.Application.Clients;

namespace Inss.GovUk.Forms.IPUpload.Application.DataFlow;

public sealed class SubmitFileUploadFlowNodeExecutor : IFlowNodeExecutor
{
    private readonly ISubmitIPUploadSectionClient _submitSectionClient;
    private const int CompletedIndex = 0;
    
    public SubmitFileUploadFlowNodeExecutor(ISubmitIPUploadSectionClient submitSectionClient)
    {
        _submitSectionClient = submitSectionClient;
    }
    public async ValueTask<NodeId?> ExecuteAsync(ExecuteContext context)
    {
        context.Section.SetCompleted();
        await _submitSectionClient.SubmitAsync(context.Section, context.Form.Id);
        return context.CurrentNode.NextNodes[CompletedIndex];
    }
}