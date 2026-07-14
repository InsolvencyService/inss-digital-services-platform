using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Providing;
using GovUk.Forms.Application.Providers;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Search;

namespace Demo.GovUk.Forms.ContactUs.Application.DataFlow;

public class FindPeopleFlowNodePreviousPathProvider : IFlowNodePreviousPathProvider
{
    private readonly IPagePropertiesProvider _pagePropertiesProvider;
    
    public FindPeopleFlowNodePreviousPathProvider(IPagePropertiesProvider pagePropertiesProvider)
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
            SummaryModel => section.Pages.GetFirstOf<SearchResultModel>().Path,
            _ => context.Form.Path
        };
        
        return ValueTask.CompletedTask;
    }
}