using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Application.Services;
using GovUk.Forms.Domain.Primitives;

namespace Inss.GovUk.Forms.IPUpload.Application.DataFlow;

public sealed class PostSubmitFlowNodeExecutor : IFlowNodeExecutor
{
    private readonly IUserFormService _userFormService;
    private const int DeclarationIndex = 0;
    
    public PostSubmitFlowNodeExecutor(IUserFormService userFormService)
    {
        _userFormService = userFormService;
    }
    public async ValueTask<NodeId?> ExecuteAsync(ExecuteContext context)
    {
        if (context.FinalExecuteStep)
        {
            await _userFormService.RemoveAsync(context.Form);
        }
        
        return context.CurrentNode.NextNodes[DeclarationIndex];
    }
}