using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Domain.Primitives;
using Inss.GovUk.Forms.IPUpload.Application.Services;
using Inss.GovUk.Forms.IPUpload.Domain;
using Inss.GovUk.Forms.IPUpload.Domain.Validation;
using Inss.GovUk.Forms.IPUpload.Domain.Validation.RP14A;
using Microsoft.Extensions.DependencyInjection;

namespace Inss.GovUk.Forms.IPUpload.Application.DataFlow;

public sealed class FileUploadFlowNodeExecutor : IFlowNodeExecutor
{
    private readonly IServiceProvider _serviceProvider;
    private const int FileUploadErrorIndex = 0;
    private const int SummaryIndex = 1;

    public FileUploadFlowNodeExecutor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
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
            await ValidateAsync(fileUploadErrors, redundancyPayment);
        }

        return fileUploadErrors.HasErrors 
            ? context.CurrentNode.NextNodes[FileUploadErrorIndex] 
            : context.CurrentNode.NextNodes[SummaryIndex];
    }
    
    private async Task ValidateAsync(IPUploadXmlErrorsModel fileUploadErrors, object redundancyPayment)
    {
        if (redundancyPayment is Domain.Employee.Spreadsheet.RP14A spreadsheetRP14A)
        {
            await ValidateSpreadsheetUploadAsync(fileUploadErrors, spreadsheetRP14A);
        }
        else if (redundancyPayment is Domain.Employee.Api.RP14A apiRP14A)
        {
            await ValidateApiUploadAsync(fileUploadErrors, apiRP14A);
        }
    }

    private async Task ValidateSpreadsheetUploadAsync(IPUploadXmlErrorsModel fileUploadErrors, Domain.Employee.Spreadsheet.RP14A redundancyPayment)
    {
        ICaseReferenceService caseReferenceService = _serviceProvider.GetRequiredService<ICaseReferenceService>();

        foreach (Domain.Employee.Spreadsheet.RP14AEmployee employee in redundancyPayment.Employee)
        {
            if (!(await caseReferenceService.CheckExistsAsync(employee.Header.CaseReference)))
            {
                fileUploadErrors.AddError(
                    employee.EmployeeName.Forenames,
                    employee.EmployeeName.Surname,
                    DateOnly.FromDateTime(employee.DateOfBirth),
                    employee.NINO,
                    employee.Header.CaseReference,
                    "Case",
                    "Case reference",
                    "[COUNT] case reference have not been matched in our system",
                    null);
            }
            
            RP14ASpreadsheetEmployeeValidator validator = new();
            FluentValidation.Results.ValidationResult? validationResult = await validator.ValidateAsync(employee);

            if (validationResult?.IsValid == false)
            {
                foreach (var validationError in validationResult.Errors)
                {
                    fileUploadErrors.AddError(
                        employee.EmployeeName.Forenames,
                        employee.EmployeeName.Surname,
                        DateOnly.FromDateTime(employee.DateOfBirth),
                        employee.NINO,
                        validationError.AttemptedValue?.ToString() ?? "Not entered",
                        ValidationInfo.GetCategory(validationError.PropertyName),
                        ValidationInfo.GetProperty(validationError.PropertyName),
                        ValidationInfo.GetError(validationError.ErrorMessage),
                        ValidationInfo.GetHint(validationError.ErrorMessage));
                }
            }
        }
    }
    
    private async Task ValidateApiUploadAsync(IPUploadXmlErrorsModel fileUploadErrors, Domain.Employee.Api.RP14A redundancyPayment)
    {
        RP14AApiValidator apiValidator = new();
        FluentValidation.Results.ValidationResult? validationResult = await apiValidator.ValidateAsync(redundancyPayment);
        
        if (validationResult?.IsValid == false)
        {
            Domain.Employee.Api.RP14AEmployee employee = redundancyPayment.Employee.First();
            
            foreach (var validationError in validationResult.Errors)
            {
                fileUploadErrors.AddError(
                    employee.EmployeeName.Forenames,
                    employee.EmployeeName.Surname,
                    DateOnly.FromDateTime(employee.DateOfBirth),
                    employee.NINO,
                    validationError.AttemptedValue?.ToString() ?? "Not entered",
                    ValidationInfo.GetCategory(validationError.PropertyName),
                    ValidationInfo.GetProperty(validationError.PropertyName),
                    ValidationInfo.GetError(validationError.ErrorMessage),
                    ValidationInfo.GetHint(validationError.ErrorMessage));
            }
        }
        
        ICaseReferenceService caseReferenceService = _serviceProvider.GetRequiredService<ICaseReferenceService>();
        
        if (!(await caseReferenceService.CheckExistsAsync(redundancyPayment.Header.CaseReference)))
        {
            Domain.Employee.Api.RP14AEmployee employee = redundancyPayment.Employee.First();
            
            fileUploadErrors.AddError(
                employee.EmployeeName.Forenames,
                employee.EmployeeName.Surname,
                DateOnly.FromDateTime(employee.DateOfBirth),
                employee.NINO,
                redundancyPayment.Header.CaseReference,
                "Case",
                "Case reference",
                "[COUNT] case reference have not been matched in our system",
                null);
        }
        
        foreach (Domain.Employee.Api.RP14AEmployee employee in redundancyPayment.Employee)
        {
            RP14AApiEmployeeValidator validator3 = new();
            FluentValidation.Results.ValidationResult? validationResult2 = await validator3.ValidateAsync(employee);
            
            if (validationResult2?.IsValid == false)
            {
                foreach (var validationError in validationResult2.Errors)
                {
                    fileUploadErrors.AddError(
                        employee.EmployeeName.Forenames,
                        employee.EmployeeName.Surname,
                        DateOnly.FromDateTime(employee.DateOfBirth),
                        employee.NINO,
                        validationError.AttemptedValue?.ToString() ?? "Not entered",
                        ValidationInfo.GetCategory(validationError.PropertyName),
                        ValidationInfo.GetProperty(validationError.PropertyName),
                        ValidationInfo.GetError(validationError.ErrorMessage),
                        ValidationInfo.GetHint(validationError.ErrorMessage));
                }
            }
        }
    }
}