using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Domain.Primitives;
using Inss.GovUk.Forms.IPUpload.Domain;

namespace Inss.GovUk.Forms.IPUpload.Application.DataFlow;

public sealed class FileUploadErrorFlowNodeLoader : IFlowNodeLoader
{
    private const int FileUploadErrorDetailIndex = 1;
    
    public ValueTask<NodeId?> LoadAsync(LoadContext context)
    {
        // Link the error details to the correct node Id as this is a spur page that is accessed without a call to action
        IPUploadXmlErrorDetailsModel fileUploadErrorDetails = context.Section.Pages.GetFirstOf<IPUploadXmlErrorDetailsModel>();
        fileUploadErrorDetails.LinkedToNode = context.CurrentNode.NextNodes[FileUploadErrorDetailIndex];
        return ValueTask.FromResult<NodeId?>(null);
    }
}