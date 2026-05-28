using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Providing;
using GovUk.Forms.Application.Providers;
using Inss.GovUk.Forms.IPUpload.Domain;

namespace Inss.GovUk.Forms.IPUpload.Application.DataFlow;

public sealed class FileUploadErrorDetailsFlowNodePreviousPathProvider : IFlowNodePreviousPathProvider
{
    private readonly IPagePropertiesProvider _pagePropertiesProvider;

    public FileUploadErrorDetailsFlowNodePreviousPathProvider(IPagePropertiesProvider pagePropertiesProvider)
    {
        _pagePropertiesProvider = pagePropertiesProvider;
    }
    
    public ValueTask UpdateAsync(FlowNodeContext context)
    {
        IPUploadXmlErrorsModel fileUploadErrors = context.Section.Pages.GetFirstOf<IPUploadXmlErrorsModel>();
        _pagePropertiesProvider.PreviousPagePath = fileUploadErrors.Path;
        return ValueTask.CompletedTask;
    }
}