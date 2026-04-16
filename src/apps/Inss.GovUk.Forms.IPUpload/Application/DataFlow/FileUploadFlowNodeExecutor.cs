using System.ComponentModel.DataAnnotations;
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

        if (context.FinalExecuteStep)
        {
            XDocument document = fileUpload.GetXml();
            RP14A redundancyPayment = _redundancyPaymentProvider.Create(document);
            List<ExtendedValidationResult> validationResults = [];
            
            if (!redundancyPayment.TryValidateRecursive(validationResults))
            {
                IPUploadXmlErrorsModel fileUploadErrors = context.Section.Pages.GetFirstOf<IPUploadXmlErrorsModel>();
                fileUploadErrors.ClearValues();
                fileUploadErrors.Filename = fileUpload.Filename;

                foreach (ExtendedValidationResult result in validationResults)
                {
                    PropertyAnnotationAttribute propertyAnnotation = result.PropertyAnnotation;
                    
                    fileUploadErrors.AddError(propertyAnnotation.Category, propertyAnnotation.PropertyName, result.ErrorMessage!);
                }
                
                return ValueTask.FromResult<NodeId?>(fileUploadErrors.HasErrors 
                    ? context.CurrentNode.NextNodes[FileUploadErrorIndex] 
                    : context.CurrentNode.NextNodes[SummaryIndex]);
            }
        }
        
        return ValueTask.FromResult<NodeId?>(context.CurrentNode.NextNodes[FileUploadErrorIndex]);

        /*
        // TODO: Hack - we need to uncouple the execute and processing as the execute gets called twice. Once before updates and once after
        // Below needs to move to a process before decider is called
        if (!string.IsNullOrWhiteSpace(fileUpload.Filename))
        {
            IPUploadXmlErrorsModel fileUploadErrors = context.Section.Pages.GetFirstOf<IPUploadXmlErrorsModel>();
            fileUploadErrors.ClearValues();
            fileUploadErrors.Filename = fileUpload.Filename;

            if (fileUpload.Filename.Equals("rp14a-with-error.xml", StringComparison.OrdinalIgnoreCase))
            {
                fileUploadErrors.AddError("Case", "Case reference", "The case reference provided does not match any of our records");

                fileUploadErrors.AddError("Employee", "Employee title", "missing employee titles");
                fileUploadErrors.AddError("Employee", "Employee title", "missing employee titles");
                fileUploadErrors.AddError("Employee", "National insurance number", "Invalid national insurance numbers");
                fileUploadErrors.AddError("Employee", "National insurance number", "Invalid national insurance numbers");
                fileUploadErrors.AddError("Employee", "National insurance number", "Invalid national insurance numbers");

                fileUploadErrors.AddError("Employment dates", "Employer start date", "invalid employer start dates");
                fileUploadErrors.AddError("Employment dates", "Employer start date", "invalid employer start dates");
            }

            return ValueTask.FromResult<NodeId?>(fileUploadErrors.HasErrors 
                ? context.CurrentNode.NextNodes[FileUploadErrorIndex] 
                : context.CurrentNode.NextNodes[SummaryIndex]);
        }

        return ValueTask.FromResult<NodeId?>(context.CurrentNode.NextNodes[FileUploadErrorIndex]);*/
    }
}