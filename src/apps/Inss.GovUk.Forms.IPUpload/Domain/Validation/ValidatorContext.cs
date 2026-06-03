using System.Globalization;
using System.Text.RegularExpressions;
using Inss.Common.IPUpload.Employee.Spreadsheet;
using Inss.GovUk.Forms.IPUpload.Application.Services;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

public abstract class ValidatorContext
{
    public List<Error> Errors { get; } = [];

    public abstract void AddError(string validationKey, string? value);

    protected void AddError(Error error)
    {
        Errors.Add(error);
    }
}

public sealed class EmployeeValidatorContext : ValidatorContext
{
    public string Forenames { get; set; }
    
    public string Surname { get; set; }
    
    public DateOnly Dob { get; set; }
    
    public string Nino { get; set; }

    public override void AddError(string validationKey, string? value)
    {
        AddError(new EmployeeError
        {
            Key = validationKey,
            Forenames = Forenames,
            Surname = Surname,
            Dob = Dob,
            Nino = Nino,
            Value = value ?? "Not entered"
        });
    }
}

// public sealed class ValidatorResult
// {
//     public string Key { get; init; }
//     
//     public Error Error { get; init; }
// }

public abstract partial class EmployeeValidator
{
    protected static void ValidateEmployerName(ValidatorContext context, string employerName)
    {
        if (employerName.Length > 99)
        {
            context.AddError(EmployerValidationInfo.InvalidEmployerNameLength.Key, employerName);
        } 
    }
    
    protected static void ValidateEmployeeSurname(ValidatorContext context, string employeeSurname)
    {
        if (string.IsNullOrWhiteSpace(employeeSurname))
        {
            context.AddError(EmployeeValidationInfo.MissingEmployeeSurname.Key, employeeSurname);
        }
        else if (employeeSurname.Length > 99)
        {
            context.AddError(EmployeeValidationInfo.InvalidEmployeeSurnameLength.Key, employeeSurname);
        } 
    }
    
    protected static void ValidateEmployeeNino(ValidatorContext context, string nino)
    {
        if (string.IsNullOrWhiteSpace(nino))
        {
            context.AddError(EmployeeValidationInfo.MissingEmployeeNino.Key, nino);
        }
        else if (!NinoFormatRegex().IsMatch(nino))
        {
            context.AddError(EmployeeValidationInfo.InvalidEmployeeNinoFormat.Key, nino);
        } 
    }

    protected static void ValidateMoneyOwedToEmployer(ValidatorContext context, decimal amountOwed)
    {
        string value = amountOwed.ToString(CultureInfo.CurrentCulture);
        
        if (!MoneyFormatRegex().IsMatch(value))
        {
            context.AddError(EmployeeValidationInfo.InvalidMoneyOwedToEmployerFormat.Key, value);
        }
    }

    protected static void ValidateEmploymentDates(ValidatorContext context, DateTime start, DateTime end)
    {
        if (start > end)
        {
            string value = $"{start:d}, {end:d}";
            context.AddError(EmployeeValidationInfo.InvalidEmployeeEmploymentDates.Key, value);
        }
    }
    
    protected static void ValidateBasicPay(ValidatorContext context, decimal basicPay)
    {
        string value = basicPay.ToString(CultureInfo.CurrentCulture);
        
        if (!MoneyFormatRegex().IsMatch(value))
        {
            context.AddError(EmployeePayValidationInfo.InvalidEmployeeBasicPayFormat.Key, value);
        }
    } 
    
    protected static void ValidateArrearsOfPayOwed(ValidatorContext context, decimal amountOwed)
    {
        string value = amountOwed.ToString(CultureInfo.CurrentCulture);
        
        if (!MoneyFormatRegex().IsMatch(value))
        {
            context.AddError(EmployeeValidationInfo.InvalidAopOwedFormat.Key, value);
        }
    }
    
    protected static void ValidateArrearsOfPayDates(ValidatorContext context, DateTime start, DateTime end)
    {
        if (start > end)
        {
            string value = $"{start:d}, {end:d}";
            context.AddError(EmployeeValidationInfo.InvalidEmployeeAopDates.Key, value);
        }
    }
    
    [GeneratedRegex(ValidationInfo.NinoFormat)]
    private static partial Regex NinoFormatRegex();
    
    [GeneratedRegex(ValidationInfo.MoneyFormat)]
    private static partial Regex MoneyFormatRegex();
}

public sealed class EmployeeSpreadsheetValidator : EmployeeValidator
{
    private readonly ICaseReferenceService _caseReferenceService;

    public EmployeeSpreadsheetValidator(ICaseReferenceService caseReferenceService)
    {
        _caseReferenceService = caseReferenceService;
    }
    
    public async Task<ValidatorContext> ValidateAsync(RP14A model)
    {
        EmployeeValidatorContext context = new();

        foreach (RP14AEmployee employee in model.Employee)
        {
            context.Forenames = employee.EmployeeName.Forenames;
            context.Surname = employee.EmployeeName.Surname;
            context.Dob = DateOnly.FromDateTime(employee.DateOfBirth);
            context.Nino = employee.NINO;
            
            bool caseReferenceExists = await _caseReferenceService.CheckExistsAsync(employee.Header.CaseReference);

            if (!caseReferenceExists)
            {
                context.AddError(CaseValidationInfo.UnknownCaseReference.Key, employee.Header.CaseReference);
            }

            ValidateEmployerName(context, employee.EmployerName);
            ValidateEmployeeSurname(context, employee.EmployeeName.Surname);
            ValidateEmployeeNino(context, employee.NINO);
            ValidateMoneyOwedToEmployer(context, employee.MoneyOwedToEmployer);
            ValidateEmploymentDates(context, employee.StartDate, employee.EndDate);

            ValidateBasicPay(context, employee.PayDetails.BasicPayPerWeek);
            ValidateArrearsOfPayOwed(context, employee.PayDetails.ArrearsOfPay.ArrearsOfPayPeriod1.AOPOwed1);
            ValidateArrearsOfPayOwed(context, employee.PayDetails.ArrearsOfPay.ArrearsOfPayPeriod2.AOPOwed2);
            ValidateArrearsOfPayOwed(context, employee.PayDetails.ArrearsOfPay.ArrearsOfPayPeriod3.AOPOwed3);
            ValidateArrearsOfPayOwed(context, employee.PayDetails.ArrearsOfPay.ArrearsOfPayPeriod4.AOPOwed4);
            ValidateArrearsOfPayDates(context, employee.PayDetails.ArrearsOfPay.ArrearsOfPayPeriod1.AOP1StartDate, employee.PayDetails.ArrearsOfPay.ArrearsOfPayPeriod1.AOP1EndDate);
            ValidateArrearsOfPayDates(context, employee.PayDetails.ArrearsOfPay.ArrearsOfPayPeriod2.AOP2StartDate, employee.PayDetails.ArrearsOfPay.ArrearsOfPayPeriod2.AOP2EndDate);
            ValidateArrearsOfPayDates(context, employee.PayDetails.ArrearsOfPay.ArrearsOfPayPeriod3.AOP3StartDate, employee.PayDetails.ArrearsOfPay.ArrearsOfPayPeriod3.AOP3EndDate);
            ValidateArrearsOfPayDates(context, employee.PayDetails.ArrearsOfPay.ArrearsOfPayPeriod4.AOP4StartDate, employee.PayDetails.ArrearsOfPay.ArrearsOfPayPeriod4.AOP4EndDate);
            
        }
        
        return context;
    }
}