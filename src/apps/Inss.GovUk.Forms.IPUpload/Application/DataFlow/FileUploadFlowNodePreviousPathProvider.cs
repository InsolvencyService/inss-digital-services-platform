using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Providing;
using GovUk.Forms.Application.Providers;
using GovUk.Forms.Domain;
using Inss.GovUk.Forms.IPUpload.Domain;

namespace Inss.GovUk.Forms.IPUpload.Application.DataFlow;

public sealed class FileUploadFlowNodePreviousPathProvider : DefaultFlowNodePreviousPathProvider
{
    private readonly IPagePropertiesProvider _pagePropertiesProvider;

    public FileUploadFlowNodePreviousPathProvider(IPagePropertiesProvider pagePropertiesProvider) : base(pagePropertiesProvider)
    {
        _pagePropertiesProvider = pagePropertiesProvider;
    }
    
    public override async ValueTask UpdateAsync(FlowNodeContext context)
    {
        await base.UpdateAsync(context);
        
        // If the base set the previous page, and we were referred from the summary page then we won't override the previous page
        if (_pagePropertiesProvider.PreviousPagePath is not null)
        {
            PageModel? previousPage = context.Section.Pages.FindPage(_pagePropertiesProvider.PreviousPagePath);

            if (previousPage is SummaryModel)
            {
                return;
            }
        }
        
        IPUploadDeclarationModel declaration = context.Section.Pages.GetFirstOf<IPUploadDeclarationModel>();
        _pagePropertiesProvider.PreviousPagePath = declaration.Path;
    }
}