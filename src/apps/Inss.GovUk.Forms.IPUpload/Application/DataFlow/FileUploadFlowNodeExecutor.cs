using System.Xml.Linq;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Domain.Extensions;
using GovUk.Forms.Domain.Primitives;
using GovUk.Forms.IPUpload.Domain;
using Inss.GovUk.Forms.IPUpload.Application.Providers;
using Inss.GovUk.Forms.IPUpload.Domain;

namespace Inss.GovUk.Forms.IPUpload.Application.DataFlow;

public sealed class FileUploadFlowNodeExecutor : IFlowNodeExecutor
{
    private const int FileUploadErrorIndex = 0;
    private const int SummaryIndex = 1;
    
    private readonly IRedundancyPaymentProvider _redundancyPaymentProvider;
    
    public FileUploadFlowNodeExecutor(IRedundancyPaymentProvider redundancyPaymentProvider)
    {
        _redundancyPaymentProvider = redundancyPaymentProvider;
    }

    public ValueTask<NodeId?> ExecuteAsync(ExecuteContext context)
    {
        XmlFileUploadModel fileUpload = context.UpdatedPage.As<XmlFileUploadModel>();
        IPUploadXmlErrorsModel fileUploadErrors = context.Section.Pages.GetFirstOf<IPUploadXmlErrorsModel>();
        fileUploadErrors.ClearErrors();
        fileUploadErrors.Filename = fileUpload.Filename;
        
        if (context.FinalExecuteStep)
        {
            XDocument document = fileUpload.GetXml();
            RP14A redundancyPayment = _redundancyPaymentProvider.Create(document);
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