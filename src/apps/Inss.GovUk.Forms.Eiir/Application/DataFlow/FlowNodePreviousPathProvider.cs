using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Providing;
using GovUk.Forms.Application.Providers;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Search;

namespace Inss.GovUk.Forms.Eiir.Application.DataFlow;

public class FlowNodePreviousPathProvider : IFlowNodePreviousPathProvider
{
    private readonly IPagePropertiesProvider _pagePropertiesProvider;
    
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
            SearchTermModel => context.Form.Path,
            SearchResultModel => section.Pages.GetFirstOf<SearchTermModel>().Path,
            SearchResultDetailModel => section.Pages.GetFirstOf<SearchResultModel>().SearchPath,
            _ => context.Form.Path
        };
        
        return ValueTask.CompletedTask;
    }
}