using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Domain.Primitives;

namespace Inss.GovUk.Forms.IPUpload.Application.DataFlow;

public sealed class PostSubmitFlowNodeLoader : IFlowNodeLoader
{
    public ValueTask<NodeId?> LoadAsync(LoadContext context)
    {
        context.Page.PreviousPagePath = null;
        return ValueTask.FromResult<NodeId?>(null);
    }
}