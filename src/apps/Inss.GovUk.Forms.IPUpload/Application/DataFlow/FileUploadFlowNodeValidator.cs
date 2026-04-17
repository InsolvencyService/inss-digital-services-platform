using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using GovUk.Forms.Application.DataFlow.Validating;
using GovUk.Forms.Application.Extensions;
using GovUk.Forms.IPUpload.Domain;
using Inss.GovUk.Forms.IPUpload.Domain;
using Inss.GovUk.Forms.IPUpload.Extensions;
using Microsoft.Extensions.Logging;

namespace Inss.GovUk.Forms.IPUpload.Application.DataFlow;

public sealed class FileUploadFlowNodeValidator : IFlowNodeValidator
{
    private readonly ILogger<FileUploadFlowNodeValidator> _logger;
    private readonly XmlFileCheckList _checkActions = [CheckFileExtension, CheckFileSize, CheckFileIsValidXml];
    private const string XmlExtension = ".xml";
    private const int MaxFileSizeInMb = 10;

    public FileUploadFlowNodeValidator(ILogger<FileUploadFlowNodeValidator> logger)
    {
        _logger = logger;
    }
    
    public async ValueTask<ValidationResult[]> ValidateAsync(ValidateContext context)
    {
        ValidationResult[] baseValidationResults = await DefaultFlowNodeValidator.Default.ValidateAsync(context);
        List<ValidationResult> validationResults = baseValidationResults.ToList();
        
        XmlFileUploadModel fileUpload = context.Page.As<XmlFileUploadModel>();

        foreach (var checkAction in _checkActions)
        {
            checkAction(this, validationResults, fileUpload);
            
            if (validationResults.Count > 0)
            {
                break;
            }
        }
        
        return await ValueTask.FromResult(validationResults.ToArray());
    }

    private static void CheckFileIsValidXml(
        FileUploadFlowNodeValidator self, 
        List<ValidationResult> validationResults, 
        XmlFileUploadModel fileUpload)
    {
        try
        {
            XDocument document = fileUpload.GetXml();
            RP14A _ = document.CreateModel<RP14A>();
        }
        catch (Exception error)
        {
            self._logger.XmlLoadError(error.Message);
            validationResults.AddResult("The file provided is invalid XML or has invalid field data", [nameof(fileUpload.Contents)]);
        }
    }
    
    private static void CheckFileExtension(
        FileUploadFlowNodeValidator self, 
        List<ValidationResult> validationResults, 
        XmlFileUploadModel fileUpload)
    {
        if (!fileUpload.Filename.EndsWith(XmlExtension, StringComparison.OrdinalIgnoreCase))
        {
            validationResults.AddResult("The file must end with an XML extension", [nameof(fileUpload.Contents)]);
        }
    }

    private static void CheckFileSize(
        FileUploadFlowNodeValidator self, 
        List<ValidationResult> validationResults, 
        XmlFileUploadModel fileUpload)
    {
        if (fileUpload.SizeInMb > MaxFileSizeInMb)
        {
            validationResults.AddResult($"The maximum file size is {MaxFileSizeInMb}Mb", [nameof(fileUpload.Contents)]);
        }
    }

    private sealed class XmlFileCheckList : List<Action<FileUploadFlowNodeValidator, List<ValidationResult>, XmlFileUploadModel>>;
}