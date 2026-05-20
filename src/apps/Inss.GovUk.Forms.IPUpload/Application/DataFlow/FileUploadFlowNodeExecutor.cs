using FluentValidation.Results;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Domain.Primitives;
using Inss.Common.IPUpload;
using Inss.GovUk.Forms.IPUpload.Application.Services;
using Inss.GovUk.Forms.IPUpload.Domain;
using Inss.GovUk.Forms.IPUpload.Domain.Validation;
using Inss.GovUk.Forms.IPUpload.Domain.Validation.Employee;
using Inss.GovUk.Forms.IPUpload.Domain.Validation.Employer;
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
        
        object redundancyPayment = FileHelper.GetRedundancyPaymentObject(fileUpload.Contents);
        await ValidateAsync(fileUploadErrors, redundancyPayment);

        return fileUploadErrors.HasErrors 
            ? context.CurrentNode.NextNodes[FileUploadErrorIndex] 
            : context.CurrentNode.NextNodes[SummaryIndex];
    }
    
    private async Task ValidateAsync(IPUploadXmlErrorsModel fileUploadErrors, object redundancyPayment)
    {
        if (redundancyPayment is Inss.Common.IPUpload.Employee.Spreadsheet.RP14A spreadsheetRP14A)
        {
            await ValidateSpreadsheetUploadAsync(fileUploadErrors, spreadsheetRP14A);
        }
        else if (redundancyPayment is Inss.Common.IPUpload.Employee.Api.RP14A apiRP14A)
        {
            await ValidateApiUploadAsync(fileUploadErrors, apiRP14A);
        }
        else if (redundancyPayment is Inss.Common.IPUpload.Employer.Spreadsheet.RP14 spreadsheetRP14)
        {
            await ValidateSpreadsheetUploadAsync(fileUploadErrors, spreadsheetRP14);
        }
        else if (redundancyPayment is Inss.Common.IPUpload.Employer.Api.RP14 apiRP14)
        {
            await ValidateApiUploadAsync(fileUploadErrors, apiRP14);
        }
    }

    private async Task ValidateSpreadsheetUploadAsync(
        IPUploadXmlErrorsModel fileUploadErrors, 
        Inss.Common.IPUpload.Employee.Spreadsheet.RP14A redundancyPayment)
    {
        ICaseReferenceService caseReferenceService = _serviceProvider.GetRequiredService<ICaseReferenceService>();

        foreach (Inss.Common.IPUpload.Employee.Spreadsheet.RP14AEmployee employee in redundancyPayment.Employee)
        {
            if (!(await caseReferenceService.CheckExistsAsync(employee.Header.CaseReference)))
            {
                fileUploadErrors.AddOrMergeError(new EmployeeErrorInfo(
                    "Case",
                    "Case reference",
                    "[COUNT] case reference have not been matched in our system",
                    null,
                    employee.EmployeeName.Forenames,
                    employee.EmployeeName.Surname,
                    DateOnly.FromDateTime(employee.DateOfBirth),
                    employee.NINO,
                    employee.Header.CaseReference));
            }
            
            RP14ASpreadsheetEmployeeValidator validator = new();
            ValidationResult? validationResult = await validator.ValidateAsync(employee);

            if (validationResult?.IsValid == false)
            {
                foreach (var validationError in validationResult.Errors)
                {
                    fileUploadErrors.AddOrMergeError(new EmployeeErrorInfo(
                        ValidationInfo.GetCategory(validationError.PropertyName),
                        ValidationInfo.GetProperty(validationError.PropertyName),
                        ValidationInfo.GetError(validationError.ErrorMessage),
                        ValidationInfo.GetHint(validationError.ErrorMessage),
                        employee.EmployeeName.Forenames,
                        employee.EmployeeName.Surname,
                        DateOnly.FromDateTime(employee.DateOfBirth),
                        employee.NINO,
                        validationError.AttemptedValue?.ToString()));
                }
            }
        }
    }
    
    private async Task ValidateSpreadsheetUploadAsync(
        IPUploadXmlErrorsModel fileUploadErrors,
        Inss.Common.IPUpload.Employer.Spreadsheet.RP14 redundancyPayment)
    {
        ICaseReferenceService caseReferenceService = _serviceProvider.GetRequiredService<ICaseReferenceService>();

        if (!string.IsNullOrWhiteSpace(redundancyPayment.Header.CaseReference))
        {
            if (!(await caseReferenceService.CheckExistsAsync(redundancyPayment.Header.CaseReference)))
            {
                fileUploadErrors.AddOrMergeError(new EmployerErrorInfo(
                    "Case",
                    "Case reference",
                    "[COUNT] case reference have not been matched in our system",
                    null,
                    ["Case reference value"],
                    [redundancyPayment.Header.CaseReference]));
            }
        }

        RP14SpreadsheetValidator rp14SpreadsheetValidator = new();
        ValidationResult? validationResult = await rp14SpreadsheetValidator.ValidateAsync(redundancyPayment);
        
        if (validationResult?.IsValid == false)
        {
            foreach (var validationError in validationResult.Errors)
            {
                fileUploadErrors.AddOrMergeError(new EmployerErrorInfo(
                    ValidationInfo.GetCategory(validationError.PropertyName),
                    ValidationInfo.GetProperty(validationError.PropertyName),
                    ValidationInfo.GetError(validationError.ErrorMessage),
                    ValidationInfo.GetHint(validationError.ErrorMessage),
                    [$"{ValidationInfo.GetProperty(validationError.PropertyName)} value"],
                    [validationError.AttemptedValue?.ToString()]
                ));
            }
        }
    }
    
    private async Task ValidateApiUploadAsync(
        IPUploadXmlErrorsModel fileUploadErrors, 
        Inss.Common.IPUpload.Employee.Api.RP14A redundancyPayment)
    {
        RP14AApiValidator apiValidator = new();
        ValidationResult? validationResult = await apiValidator.ValidateAsync(redundancyPayment);
        
        if (validationResult?.IsValid == false)
        {
            Inss.Common.IPUpload.Employee.Api.RP14AEmployee employee = redundancyPayment.Employee.First();
            
            foreach (var validationError in validationResult.Errors)
            {
                fileUploadErrors.AddOrMergeError(new EmployeeErrorInfo(
                    ValidationInfo.GetCategory(validationError.PropertyName),
                    ValidationInfo.GetProperty(validationError.PropertyName),
                    ValidationInfo.GetError(validationError.ErrorMessage),
                    ValidationInfo.GetHint(validationError.ErrorMessage),
                    employee.EmployeeName.Forenames,
                    employee.EmployeeName.Surname,
                    DateOnly.FromDateTime(employee.DateOfBirth),
                    employee.NINO,
                    validationError.AttemptedValue?.ToString()));
            }
        }
        
        ICaseReferenceService caseReferenceService = _serviceProvider.GetRequiredService<ICaseReferenceService>();
        
        if (!(await caseReferenceService.CheckExistsAsync(redundancyPayment.Header.CaseReference)))
        {
            Inss.Common.IPUpload.Employee.Api.RP14AEmployee employee = redundancyPayment.Employee.First();
            
            fileUploadErrors.AddOrMergeError(new EmployeeErrorInfo(
                "Case",
                "Case reference",
                "[COUNT] case reference have not been matched in our system",
                null,
                employee.EmployeeName.Forenames,
                employee.EmployeeName.Surname,
                DateOnly.FromDateTime(employee.DateOfBirth),
                employee.NINO,
                redundancyPayment.Header.CaseReference));
        }
        
        foreach (Inss.Common.IPUpload.Employee.Api.RP14AEmployee employee in redundancyPayment.Employee)
        {
            RP14AApiEmployeeValidator validator3 = new();
            ValidationResult? employeeValidationResult = await validator3.ValidateAsync(employee);
            
            if (employeeValidationResult?.IsValid == false)
            {
                foreach (var validationError in employeeValidationResult.Errors)
                {
                    fileUploadErrors.AddOrMergeError(new EmployeeErrorInfo(
                        ValidationInfo.GetCategory(validationError.PropertyName),
                        ValidationInfo.GetProperty(validationError.PropertyName),
                        ValidationInfo.GetError(validationError.ErrorMessage),
                        ValidationInfo.GetHint(validationError.ErrorMessage),
                        employee.EmployeeName.Forenames,
                        employee.EmployeeName.Surname,
                        DateOnly.FromDateTime(employee.DateOfBirth),
                        employee.NINO,
                        validationError.AttemptedValue?.ToString()));
                }
            }
        }
    }
    
    private async Task ValidateApiUploadAsync(
        IPUploadXmlErrorsModel fileUploadErrors, 
        Inss.Common.IPUpload.Employer.Api.RP14 redundancyPayment)
    {
        ICaseReferenceService caseReferenceService = _serviceProvider.GetRequiredService<ICaseReferenceService>();

        if (!string.IsNullOrWhiteSpace(redundancyPayment.Header.CaseReference))
        {
            if (!(await caseReferenceService.CheckExistsAsync(redundancyPayment.Header.CaseReference)))
            {
                fileUploadErrors.AddOrMergeError(new EmployerErrorInfo(
                    "Case",
                    "Case reference",
                    "[COUNT] case reference have not been matched in our system",
                    null,
                    ["Case reference value"],
                    [redundancyPayment.Header.CaseReference]));
            }
        }

        RP14ApiValidator rp14ApiValidator = new();
        ValidationResult? validationResult = await rp14ApiValidator.ValidateAsync(redundancyPayment);
        
        if (validationResult?.IsValid == false)
        {
            foreach (var validationError in validationResult.Errors)
            {
                fileUploadErrors.AddOrMergeError(new EmployerErrorInfo(
                    ValidationInfo.GetCategory(validationError.PropertyName),
                    ValidationInfo.GetProperty(validationError.PropertyName),
                    ValidationInfo.GetError(validationError.ErrorMessage),
                    ValidationInfo.GetHint(validationError.ErrorMessage),
                    [$"{ValidationInfo.GetProperty(validationError.PropertyName)} value"],
                    [validationError.AttemptedValue?.ToString()]
                ));
            }
        }
    }
}