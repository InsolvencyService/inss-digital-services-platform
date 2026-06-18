using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Providing;
using GovUk.Forms.Application.Providers;

namespace Inss.GovUk.Forms.IPUpload.Application.DataFlow;

public sealed class DeclarationFlowNodePreviousPathProvider : DefaultFlowNodePreviousPathProvider
{
    private readonly IPagePropertiesProvider _pagePropertiesProvider;

    public DeclarationFlowNodePreviousPathProvider(IPagePropertiesProvider pagePropertiesProvider) : base(pagePropertiesProvider)
    {
        _pagePropertiesProvider = pagePropertiesProvider;
    }
    
    public override ValueTask UpdateAsync(FlowNodeContext context)
    {
        _pagePropertiesProvider.PreviousPagePath = "/";
        return ValueTask.CompletedTask;
    }
}