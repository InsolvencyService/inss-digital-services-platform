using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Domain.Primitives;
using Inss.GovUk.Forms.IPUpload.Application.Exceptions;
using Inss.GovUk.Forms.IPUpload.Domain;

namespace Inss.GovUk.Forms.IPUpload.Application.DataFlow;

public sealed class FileUploadErrorDetailsFlowNodeLoader : IFlowNodeLoader
{
    public ValueTask<NodeId?> LoadAsync(LoadContext context)
    {
        if (context.State is null)
        {
            throw new IPUploadException("Unable to load the IP upload error details as the state is unset.");
        }

        IPUploadXmlErrorsModel fileUploadErrors = context.Section.Pages.GetFirstOf<IPUploadXmlErrorsModel>();
        IPUploadXmlErrorDetailsModel fileUploadErrorDetails = context.Section.Pages.GetFirstOf<IPUploadXmlErrorDetailsModel>();
        fileUploadErrorDetails.CurrentErrorDetail = fileUploadErrors.GetError(context.State)!;
        fileUploadErrorDetails.PreviousPagePath = fileUploadErrors.Path;
        fileUploadErrorDetails.FullWidthLayout = true;
        
        return ValueTask.FromResult<NodeId?>(null);
    }
}