using System.Diagnostics.CodeAnalysis;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.DataFlow.Loading;

[ExcludeFromCodeCoverage]
public sealed class NoopFlowNodeLoader : IFlowNodeLoader
{
    public static readonly IFlowNodeLoader Default = new NoopFlowNodeLoader();
    
    private NoopFlowNodeLoader()
    {
    }
    
    public ValueTask<NodeId?> LoadAsync(FlowNodeContext context)
    {
        return ValueTask.FromResult<NodeId?>(null);
    }
}

[ExcludeFromCodeCoverage]
public sealed class NoopPageLoader : IPageLoader
{
    public static readonly IPageLoader Default = new NoopPageLoader();
    
    private NoopPageLoader()
    {
    }
    
    public ValueTask LoadAsync(LoadPageContext context)
    {
        return ValueTask.CompletedTask;
    }
}