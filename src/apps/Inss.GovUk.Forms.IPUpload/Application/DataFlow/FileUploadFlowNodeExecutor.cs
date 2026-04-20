using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Domain.Primitives;
using Inss.GovUk.Forms.IPUpload.Domain;
using Inss.GovUk.Forms.IPUpload.Domain.Extensions;
using Inss.GovUk.Forms.IPUpload.Domain.Validation;

namespace Inss.GovUk.Forms.IPUpload.Application.DataFlow;

public sealed class FileUploadFlowNodeExecutor : IFlowNodeExecutor
{
    private const int FileUploadErrorIndex = 0;
    private const int SummaryIndex = 1;
    
    public ValueTask<NodeId?> ExecuteAsync(ExecuteContext context)
    {
        XmlFileUploadModel fileUpload = context.UpdatedPage.As<XmlFileUploadModel>();
        IPUploadXmlErrorsModel fileUploadErrors = context.Section.Pages.GetFirstOf<IPUploadXmlErrorsModel>();
        fileUploadErrors.ClearErrors();
        fileUploadErrors.Filename = fileUpload.Filename;
        
        if (context.FinalExecuteStep)
        {
            object redundancyPayment = fileUpload.GetRedundancyPaymentObject();
            List<ExtendedValidationResult> validationResults = [];
            
            if (!redundancyPayment.TryValidateRecursive(validationResults))
            {
                foreach (ExtendedValidationResult result in validationResults)
                {
                    PropertyAnnotationAttribute propertyAnnotation = result.PropertyAnnotation;
                    fileUploadErrors.AddError(propertyAnnotation.Category, propertyAnnotation.PropertyName, result.ErrorMessage!);
                }
            }
        }
        
        return ValueTask.FromResult<NodeId?>(fileUploadErrors.HasErrors 
            ? context.CurrentNode.NextNodes[FileUploadErrorIndex] 
            : context.CurrentNode.NextNodes[SummaryIndex]);
    }
}