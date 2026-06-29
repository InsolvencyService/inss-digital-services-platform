using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Providing;
using GovUk.Forms.Application.Providers;
using GovUk.Forms.Domain;

namespace Inss.GovUk.Forms.Fip.Application.DataFlow;

public class FlowNodePreviousPathProvider : IFlowNodePreviousPathProvider
{
    private readonly IPagePropertiesProvider _pagePropertiesProvider;
    private const string EmptyPath = "/";
    
    public FlowNodePreviousPathProvider(IPagePropertiesProvider pagePropertiesProvider)
    {
        _pagePropertiesProvider = pagePropertiesProvider;
    }
    
    public virtual ValueTask UpdateAsync(FlowNodeContext context)
    {
        SectionModel section = context.Section;
        
        if (section.ReturnUrl is not null && context.CurrentPage is not SummaryModel)
        {
            _pagePropertiesProvider.PreviousPagePath = section.ReturnUrl;
            return ValueTask.CompletedTask;
        }

        _pagePropertiesProvider.PreviousPagePath = context.CurrentPage switch
        {
            DateModel => EmptyPath,
            SummaryModel => section.Pages.GetFirstOf<DateModel>().Path,
            _ => EmptyPath
        };
        
        return ValueTask.CompletedTask;
    }
}