using GovUk.Forms.Application.DataFlow.Loading;
using GovUk.Forms.Domain.Primitives;
using Inss.GovUk.Forms.IPUpload.Domain;

namespace Inss.GovUk.Forms.IPUpload.Application.DataFlow;

public sealed class FileUploadFlowNodeLoader : IFlowNodeLoader
{
    public ValueTask<NodeId?> LoadAsync(LoadContext context)
    {
        IPUploadDeclarationModel declaration = context.Section.Pages.GetFirstOf<IPUploadDeclarationModel>();

        if (!declaration.Accepted)
        {
            throw new InvalidOperationException("You must accept the declaration.");
        }

        IPUploadXmlErrorsModel fileUploadErrors = context.Section.Pages.GetFirstOf<IPUploadXmlErrorsModel>();
        fileUploadErrors.ClearValues();
        
        return ValueTask.FromResult<NodeId?>(null);
    }
}