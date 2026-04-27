using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using GovUk.Forms.Application.DataFlow.Executing;
using GovUk.Forms.Domain.Primitives;
using Inss.GovUk.Forms.IPUpload.Domain;
using Inss.GovUk.Forms.IPUpload.Domain.Validation;
using Inss.GovUk.Forms.IPUpload.Domain.Validation.Lookups;

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
    
    public ValueTask<NodeId?> ExecuteAsync(ExecuteContext context)
    {
        XmlFileUploadModel fileUpload = context.UpdatedPage.As<XmlFileUploadModel>();
        IPUploadXmlErrorsModel fileUploadErrors = context.Section.Pages.GetFirstOf<IPUploadXmlErrorsModel>();
        fileUploadErrors.ClearErrors();
        fileUploadErrors.Filename = fileUpload.Filename;
        
        if (context.FinalExecuteStep)
        {
            object redundancyPayment = fileUpload.GetRedundancyPaymentObject();
            Validate(fileUploadErrors, redundancyPayment);
        }

        return ValueTask.FromResult<NodeId?>(fileUploadErrors.HasErrors 
            ? context.CurrentNode.NextNodes[FileUploadErrorIndex] 
            : context.CurrentNode.NextNodes[SummaryIndex]);
    }
    
    private void Validate(IPUploadXmlErrorsModel fileUploadErrors, object redundancyPayment)
    {
        if (redundancyPayment is Domain.Spreadsheet.RP14A spreadsheetRP14A)
        {
            ValidateSpreadsheetUpload(fileUploadErrors, spreadsheetRP14A);
        }
        else if (redundancyPayment is Domain.Api.RP14A apiRP14A)
        {
            ValidateApiUpload(fileUploadErrors, apiRP14A);
        }
    }

    private void ValidateSpreadsheetUpload(IPUploadXmlErrorsModel fileUploadErrors, Domain.Spreadsheet.RP14A redundancyPayment)
    {
        foreach (Domain.Spreadsheet.RP14AEmployee employee in redundancyPayment.Employee)
        {
            List<ErrorInfoValidationResult> results = [];

            if (!TryValidateRecursive(employee, results, _serviceProvider))
            {
                foreach (ErrorInfoValidationResult result in results)
                {
                    fileUploadErrors.AddError(
                        employee.EmployeeName.Forenames,
                        employee.EmployeeName.Surname,
                        DateOnly.FromDateTime(employee.DateOfBirth),
                        employee.NINO,
                        result.Value,
                        result.ErrorInfo.Category, 
                        result.ErrorInfo.Property, 
                        result.ErrorInfo.Error,
                        result.ErrorInfo.Hint);
                }
            }
        }
    }
    
    private void ValidateApiUpload(IPUploadXmlErrorsModel fileUploadErrors, Domain.Api.RP14A redundancyPayment)
    {
        foreach (Domain.Api.RP14AEmployee employee in redundancyPayment.Employee)
        {
            List<ErrorInfoValidationResult> results = [];
            
            if (!TryValidateRecursive(employee, results, _serviceProvider))
            {
                foreach (ErrorInfoValidationResult result in results)
                {
                    fileUploadErrors.AddError(
                        employee.EmployeeName.Forenames,
                        employee.EmployeeName.Surname,
                        DateOnly.FromDateTime(employee.DateOfBirth),
                        employee.NINO,
                        result.Value,
                        result.ErrorInfo.Category, 
                        result.ErrorInfo.Property, 
                        result.ErrorInfo.Error,
                        result.ErrorInfo.Hint);
                }
            }
        }
    }

    private static bool TryValidateRecursive(object instance, List<ErrorInfoValidationResult> results, IServiceProvider serviceProvider)
    {
        List<ValidationResult> internalResults = [];
        ValidationContext context = new(instance);
        context.InitializeServiceProvider(serviceProvider.GetService);
        
        bool isValid = Validator.TryValidateObject(instance, context, internalResults, validateAllProperties: true);

        if (instance is IValidatableObject validatableObject)
        {
            internalResults.AddRange(validatableObject.Validate(context));
        }

        if (!isValid)
        {
            foreach (var result in internalResults)
            {
                List<string> values = [];

                foreach (string memberName in result.MemberNames)
                {
                    PropertyInfo property = instance.GetType().GetProperties().First(p => p.Name == memberName);
                    object? value = property.GetValue(instance);

                    if (value is not null && property.PropertyType == typeof(DateTime))
                    {
                        DateTime date = DateTime.Parse(value.ToString()!, CultureInfo.CurrentCulture);
                        values.Add(date.ToString("d", CultureInfo.CurrentCulture));
                    }
                    else
                    {
                        values.Add(property.GetValue(instance)?.ToString() ?? "Not entered");
                    }
                }

                ErrorInfo errorInfo = ValidationAnnotationProvider.GetErrorInfo(result.ErrorMessage!);
                results.Add(new ErrorInfoValidationResult(result, errorInfo, string.Join(", ", values)));
            }
        }

        var properties = instance.GetType().GetProperties()
            .Where(p => p.CanRead && p.PropertyType != typeof(string) && !p.PropertyType.IsValueType);

        foreach (var property in properties)
        {
            var value = property.GetValue(instance);

            if (value is null)
            {
                continue;
            }

            if (value is IEnumerable<object> collection)
            {
                foreach (var element in collection)
                {
                    isValid &= TryValidateRecursive(element, results, serviceProvider);
                }
            }
            else
            {
                isValid &= TryValidateRecursive(value, results, serviceProvider);
            }
        }

        return isValid;
    }
}