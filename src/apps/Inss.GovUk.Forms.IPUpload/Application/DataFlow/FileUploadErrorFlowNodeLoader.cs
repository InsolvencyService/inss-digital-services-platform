using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Domain.Primitives;
using Inss.GovUk.Forms.IPUpload.Domain;

namespace Inss.GovUk.Forms.IPUpload.Application.DataFlow;

public sealed class FileUploadErrorFlowNodeLoader : IFlowNodeLoader
{
    public ValueTask<NodeId?> LoadAsync(LoadContext context)
    {
        XmlFileUploadModel fileUpload = context.Section.Pages.GetFirstOf<XmlFileUploadModel>();
        IPUploadXmlErrorsModel fileUploadErrors = context.Section.Pages.GetFirstOf<IPUploadXmlErrorsModel>();
        fileUploadErrors.CurrentErrorDetail = null;
        fileUploadErrors.PreviousPagePath = fileUpload.Path;
        fileUploadErrors.FullWidthLayout = false;
        
        if (context.State is not null)
        {
            fileUploadErrors.CurrentErrorDetail = fileUploadErrors.GetError(context.State);
            
            if (fileUploadErrors.CurrentErrorDetail is not null)
            {
                fileUploadErrors.PreviousPagePath = fileUploadErrors.Path;
                fileUploadErrors.FullWidthLayout = true;
            }
        }
        
        return ValueTask.FromResult<NodeId?>(null);
    }
}