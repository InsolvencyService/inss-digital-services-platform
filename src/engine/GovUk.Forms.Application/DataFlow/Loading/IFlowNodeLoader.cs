using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.DataFlow.Loading;

public interface IFlowNodeLoader
{
    ValueTask<NodeId?> LoadAsync(LoadContext context);
}