using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Application.Services;
using GovUk.Forms.Domain.Primitives;

namespace Inss.GovUk.Forms.IPUpload.Application.DataFlow;

public sealed class PostSubmitFlowNodeLoader : IFlowNodeLoader
{
    private readonly IUserFormService _userFormService;
    
    public PostSubmitFlowNodeLoader(IUserFormService userFormService)
    {
        _userFormService = userFormService;
    }
    
    public async ValueTask<NodeId?> LoadAsync(FlowNodeContext context)
    {
        context.Section.VisitedNodes = [];
        await _userFormService.RemoveAsync(context.Form);
        return null;
    }
}