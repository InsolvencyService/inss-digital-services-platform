using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Providing;
using GovUk.Forms.Application.Providers;

namespace Inss.GovUk.Forms.IPUpload.Application.DataFlow;

public sealed class PostSubmitFlowNodePreviousPathProvider : IFlowNodePreviousPathProvider
{
    private readonly IPagePropertiesProvider _pagePropertiesProvider;

    public PostSubmitFlowNodePreviousPathProvider(IPagePropertiesProvider pagePropertiesProvider)
    {
        _pagePropertiesProvider = pagePropertiesProvider;
    }
    
    public ValueTask UpdateAsync(FlowNodeContext context)
    {
        _pagePropertiesProvider.PreviousPagePath = null;
        return ValueTask.CompletedTask;
    }
}