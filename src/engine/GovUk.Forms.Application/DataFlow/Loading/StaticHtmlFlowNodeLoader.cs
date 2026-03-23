using GovUk.Forms.Application.Providers;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Application.DataFlow.Loading;

public sealed class StaticHtmlFlowNodeLoader : IFlowNodeLoader
{
    private readonly IStaticContentProvider _staticContentProvider;

    public StaticHtmlFlowNodeLoader(IStaticContentProvider staticContentProvider)
    {
        _staticContentProvider = staticContentProvider;
    }
    
    public async ValueTask<NodeId?> LoadAsync(LoadContext context)
    {
        StaticHtmlModel staticHtml = context.Page.As<StaticHtmlModel>();

        if (string.IsNullOrWhiteSpace(staticHtml.Html))
        {
            staticHtml.Html = await _staticContentProvider.GetAsync(staticHtml.Key);
        }

        return null;
    }
}