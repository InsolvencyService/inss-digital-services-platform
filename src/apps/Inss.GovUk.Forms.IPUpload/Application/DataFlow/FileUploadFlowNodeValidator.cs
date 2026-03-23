using System.ComponentModel.DataAnnotations;
using GovUk.Forms.Application.DataFlow.Validating;
using GovUk.Forms.Application.Extensions;
using Inss.GovUk.Forms.IPUpload.Domain;

namespace Inss.GovUk.Forms.IPUpload.Application.DataFlow;

public sealed class FileUploadFlowNodeValidator : IFlowNodeValidator
{
    private const string XmlExtension = ".xml";
    private const int MaxFileSizeInMb = 10;
    
    public async ValueTask<ValidationResult[]> ValidateAsync(ValidateContext context)
    {
        ValidationResult[] baseValidationResults = await DefaultFlowNodeValidator.Default.ValidateAsync(context);
        List<ValidationResult> validationResults = baseValidationResults.ToList();
        
        XmlFileUploadModel fileUpload = context.Page.As<XmlFileUploadModel>();

        if (!fileUpload.Filename.EndsWith(XmlExtension, StringComparison.OrdinalIgnoreCase))
        {
            validationResults.AddResult("The file must end with an XML extension", [nameof(fileUpload.Contents)]);
            return await ValueTask.FromResult(validationResults.ToArray());
        }
        
        if (fileUpload.SizeInMb > MaxFileSizeInMb)
        {
            validationResults.AddResult($"The maximum file size is {MaxFileSizeInMb}Mb", [nameof(fileUpload.Contents)]);
        }
        
        return await ValueTask.FromResult(validationResults.ToArray());
    }
}