using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Providing;
using GovUk.Forms.Application.Providers;
using GovUk.Forms.Domain;
using Inss.GovUk.Forms.IPUpload.Domain;

namespace Inss.GovUk.Forms.IPUpload.Application.DataFlow;

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
            IPUploadDeclarationModel => EmptyPath,
            CheckCaseReferenceModel => section.Pages.GetFirstOf<IPUploadDeclarationModel>().Path,
            EmployerDetailsModel => section.Pages.GetFirstOf<CheckCaseReferenceModel>().Path,
            XmlFileUploadModel => section.Pages.GetFirstOf<EmployerDetailsModel>().Path,
            IPUploadXmlErrorsModel => section.Pages.GetFirstOf<XmlFileUploadModel>().Path,
            IPUploadXmlErrorDetailsModel => section.Pages.GetFirstOf<IPUploadXmlErrorsModel>().Path,
            SummaryModel => section.Pages.GetFirstOf<XmlFileUploadModel>().Path,
            PostSubmitModel => null,
            _ => EmptyPath
        };
        
        return ValueTask.CompletedTask;
    }
}