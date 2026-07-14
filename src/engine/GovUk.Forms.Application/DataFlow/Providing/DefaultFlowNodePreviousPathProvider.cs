using GovUk.Forms.Application.Providers;

namespace GovUk.Forms.Application.DataFlow.Providing;

public class DefaultFlowNodePreviousPathProvider : IFlowNodePreviousPathProvider
{
    private readonly IPagePropertiesProvider _pagePropertiesProvider;
    private const string EmptyPath = "/";

    public DefaultFlowNodePreviousPathProvider(IPagePropertiesProvider pagePropertiesProvider)
    {
        _pagePropertiesProvider = pagePropertiesProvider;
    }
    
    public virtual ValueTask UpdateAsync(FlowNodeContext context)
    {
        // This is not correct as a default but we need to revisit the whole back button navigation
        _pagePropertiesProvider.PreviousPagePath = EmptyPath;
        return ValueTask.CompletedTask;
    }
}