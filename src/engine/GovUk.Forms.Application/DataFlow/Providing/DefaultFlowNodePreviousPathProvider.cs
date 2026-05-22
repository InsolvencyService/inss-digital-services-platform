using GovUk.Forms.Application.Providers;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Primitives;

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
        // Special case: If referer is the summary page then set to summary path so we return there
        if (context.RefererPath is not null && context.RefererPath != EmptyPath)
        {
            PageModel? refererPage = context.Section.Pages.FindPage(context.RefererPath);

            if (refererPage is SummaryModel)
            {
                _pagePropertiesProvider.PreviousPagePath = refererPage.Path;
                return ValueTask.CompletedTask;
            }
        }

        // Get the node before the current node from the section
        if (context.Section.VisitedNodes.Length > 0)
        {
            NodeId lastNodeId = context.Section.VisitedNodes.Last();
            FlowNode lastNode = context.Nodes.First(n => n.Id == lastNodeId);
            _pagePropertiesProvider.PreviousPagePath = lastNode.PagePath;
            return ValueTask.CompletedTask;
        }
        
        // Special case: No previous page defined and the current page matches the section then we either:
        // a) use the form path for multiple sections or
        // b) use the app root
        if (_pagePropertiesProvider.PreviousPagePath is null && context.CurrentPage == context.Section.FirstPage)
        {
            _pagePropertiesProvider.PreviousPagePath = context.Form.Sections.Count == 1 ? EmptyPath : context.Form.Path;
        }  

        return ValueTask.CompletedTask;
    }
}