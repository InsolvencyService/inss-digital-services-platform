using GovUk.Forms.Application.DataFlow;
using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Application.Providers;
using GovUk.Forms.Domain.Primitives;
using Inss.GovUk.Forms.IPUpload.Application.Exceptions;
using Inss.GovUk.Forms.IPUpload.Domain;

namespace Inss.GovUk.Forms.IPUpload.Application.DataFlow;

public sealed class FileUploadErrorDetailsFlowNodeLoader : IFlowNodeLoader
{
    private readonly IPagePropertiesProvider _pagePropertiesProvider;

    public FileUploadErrorDetailsFlowNodeLoader(IPagePropertiesProvider pagePropertiesProvider)
    {
        _pagePropertiesProvider = pagePropertiesProvider;
    }
    
    public ValueTask<NodeId?> LoadAsync(FlowNodeContext context)
    {
        string? errorId = context.GetQueryParam<string?>("errorId");
        
        if (errorId is null)
        {
            throw new IPUploadException("Unable to load the IP upload error details as the error Id is unset.");
        }

        IPUploadXmlErrorsModel fileUploadErrors = context.Section.Pages.GetFirstOf<IPUploadXmlErrorsModel>();
        IPUploadXmlErrorDetailsModel fileUploadErrorDetails = context.Section.Pages.GetFirstOf<IPUploadXmlErrorDetailsModel>();
        fileUploadErrorDetails.CurrentErrorDetail = fileUploadErrors.GetPropertyErrors(errorId);
        _pagePropertiesProvider.FullPageLayout = true;
        fileUploadErrorDetails.ReturnUrl = fileUploadErrors.Path;
        
        return ValueTask.FromResult<NodeId?>(null);
    }
}