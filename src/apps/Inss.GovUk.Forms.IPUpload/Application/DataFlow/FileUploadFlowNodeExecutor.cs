using System.ComponentModel.DataAnnotations;
using System.Reflection;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Domain.Primitives;
using Inss.GovUk.Forms.IPUpload.Application.Exceptions;
using Inss.GovUk.Forms.IPUpload.Application.Services;
using Inss.GovUk.Forms.IPUpload.Domain;
using Inss.GovUk.Forms.IPUpload.Domain.Extensions;
using Inss.GovUk.Forms.IPUpload.Domain.Validation;

namespace Inss.GovUk.Forms.IPUpload.Application.DataFlow;

public sealed class FileUploadFlowNodeExecutor : IFlowNodeExecutor
{
    private readonly ICaseReferenceService _caseReferenceService;
    private const int FileUploadErrorIndex = 0;
    private const int SummaryIndex = 1;

    public FileUploadFlowNodeExecutor(ICaseReferenceService caseReferenceService)
    {
        _caseReferenceService = caseReferenceService;
    }
    
    public async ValueTask<NodeId?> ExecuteAsync(ExecuteContext context)
    {
        XmlFileUploadModel fileUpload = context.UpdatedPage.As<XmlFileUploadModel>();
        IPUploadXmlErrorsModel fileUploadErrors = context.Section.Pages.GetFirstOf<IPUploadXmlErrorsModel>();
        fileUploadErrors.ClearErrors();
        fileUploadErrors.Filename = fileUpload.Filename;
        
        if (context.FinalExecuteStep)
        {
            object redundancyPayment = fileUpload.GetRedundancyPaymentObject();
            List<ExtendedValidationResult> validationResults = await Todo(redundancyPayment);
            
            if (!redundancyPayment.TryValidateRecursive(validationResults))
            {
                foreach (ExtendedValidationResult result in validationResults)
                {
                    PropertyAnnotationAttribute propertyAnnotation = result.PropertyAnnotation;
                    fileUploadErrors.AddError(propertyAnnotation.Category, propertyAnnotation.PropertyName, result.ErrorMessage!);
                }
            }
        }

        return fileUploadErrors.HasErrors 
            ? context.CurrentNode.NextNodes[FileUploadErrorIndex] 
            : context.CurrentNode.NextNodes[SummaryIndex];
    }

    private async Task<List<ExtendedValidationResult>> Todo(object redundancyPayment)
    {
        return redundancyPayment switch
        {
            Domain.Spreadsheet.RP14A instance => await Test(instance),
            Domain.Api.RP14A instance => await  Test2(instance),
            _ => throw new IPUploadException("")
        };
    }

    private async Task<List<ExtendedValidationResult>> Test(Domain.Spreadsheet.RP14A redundancyPayment)
    {
        List<ExtendedValidationResult> results = [];

        foreach (Domain.Spreadsheet.RP14AEmployee employee in redundancyPayment.Employee)
        {
            bool exists = await _caseReferenceService.CheckExistsAsync(employee.Header.CaseReference);

            if (!exists)
            {
                AddUnknownCaseReferenceError(results, employee.Header);
            }
        }
        
        return results;
    }
    
    private async Task<List<ExtendedValidationResult>> Test2(Domain.Api.RP14A redundancyPayment)
    {
        List<ExtendedValidationResult> results = [];

        bool exists = await _caseReferenceService.CheckExistsAsync(redundancyPayment.Header.CaseReference);

        if (!exists)
        {
            AddUnknownCaseReferenceError(results, redundancyPayment.Header);
        }
        
        return results;
    }

    private static void AddUnknownCaseReferenceError(List<ExtendedValidationResult> results, object header)
    {
        PropertyInfo propInfo = header.GetType().GetProperty("CaseReference")!;

        ValidationResult validationResult = new(CaseReferenceAnnotation.NotFoundErrorMessageFormat, [propInfo.Name]);
        var extendedValidationResult = new ExtendedValidationResult(validationResult, propInfo)
        {
            PropertyAnnotation = new PropertyAnnotationAttribute(CaseReferenceAnnotation.Category, CaseReferenceAnnotation.PropertyName)
        };
        results.Add(extendedValidationResult);
    }
}