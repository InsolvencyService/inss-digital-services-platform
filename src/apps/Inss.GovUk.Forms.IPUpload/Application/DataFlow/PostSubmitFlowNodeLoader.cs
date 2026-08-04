using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Application.Services;
using GovUk.Forms.Domain.Primitives;
using Inss.GovUk.Forms.IPUpload.Application.Clients;

namespace Inss.GovUk.Forms.IPUpload.Application.DataFlow;

public sealed class PostSubmitFlowNodeLoader : IFlowNodeLoader
{
    private readonly IUserFormService _userFormService;
    private readonly IUploadContentBlobClient _uploadContentBlobClient;

    public PostSubmitFlowNodeLoader(IUserFormService userFormService, IUploadContentBlobClient uploadContentBlobClient)
    {
        _userFormService = userFormService;
        _uploadContentBlobClient = uploadContentBlobClient;
    }
    
    public async ValueTask<NodeId?> LoadAsync(FlowNodeContext context)
    {
        await _uploadContentBlobClient.RemoveAsync(context.Form.Id);
        await _userFormService.RemoveAsync(context.Form);
        return null;
    }
}