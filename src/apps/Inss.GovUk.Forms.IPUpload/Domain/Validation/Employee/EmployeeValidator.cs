using System.Globalization;
using System.Text.RegularExpressions;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employee;

public abstract partial class EmployeeValidator : BaseValidator
{
    protected static void ValidateAverageHoursWorked(ValidatorContext context, decimal averageHours)
    {
        string value = averageHours.ToString(CultureInfo.InvariantCulture);
        
        if (!AverageHoursFormatRegex().IsMatch(value))
        {
            context.AddError(EmployeeValidationInfo.InvalidAverageHoursWorkedFormat(), value);
        }
    }
    
    protected static void ValidateEmployerName(ValidatorContext context, string employerName)
    {
        if (employerName.Length > 99)
        {
            context.AddError(EmployerValidationInfo.InvalidEmployerNameLength(), employerName);
        } 
    }
    
    protected static void ValidateEmployeeSurname(ValidatorContext context, string employeeSurname)
    {
        if (string.IsNullOrWhiteSpace(employeeSurname))
        {
            context.AddError(EmployeeValidationInfo.MissingEmployeeSurname(), employeeSurname);
        }
        else if (employeeSurname.Length > 99)
        {
            context.AddError(EmployeeValidationInfo.InvalidEmployeeSurnameLength(), employeeSurname);
        } 
    }
    
    protected static void ValidateEmployeeNino(ValidatorContext context, string nino)
    {
        if (string.IsNullOrWhiteSpace(nino))
        {
            context.AddError(EmployeeValidationInfo.MissingEmployeeNino(), nino);
        }
        else if (!NinoFormatRegex().IsMatch(nino.Replace(" ", string.Empty)))
        {
            context.AddError(EmployeeValidationInfo.InvalidEmployeeNinoFormat(), nino);
        } 
    }

    protected static void ValidateMoneyOwedToEmployer(ValidatorContext context, decimal amountOwed)
    {
        string value = amountOwed.ToString(CultureInfo.InvariantCulture);
        
        if (!MoneyFormatRegex().IsMatch(value))
        {
            context.AddError(EmployeeValidationInfo.InvalidMoneyOwedToEmployerFormat(), value);
        }
    }

    protected static void ValidateEmploymentDates(ValidatorContext context, DateTime start, DateTime end)
    {
        if (start != DateTime.MinValue && start > end) // The spreadsheet will still process but have empty/default values
        {
            string value = $"{start:d}, {end:d}";
            context.AddError(EmployeeValidationInfo.InvalidEmployeeEmploymentDates(), value);
        }
    }
    
    protected static void ValidateBasicPay(ValidatorContext context, decimal basicPay)
    {
        string value = basicPay.ToString(CultureInfo.InvariantCulture);
        
        if (!MoneyFormatRegex().IsMatch(value))
        {
            context.AddError(EmployeePayValidationInfo.InvalidEmployeeBasicPayFormat(), value);
        }
    } 
    
    protected static void ValidateArrearsOfPayOwed(ValidatorContext context, decimal amountOwed)
    {
        string value = amountOwed.ToString(CultureInfo.InvariantCulture);
        
        if (!MoneyFormatRegex().IsMatch(value))
        {
            context.AddError(EmployeeValidationInfo.InvalidAopOwedFormat(), value);
        }
    }
    
    protected static void ValidateArrearsOfPayDates(ValidatorContext context, DateTime start, DateTime end)
    {
        if (start != DateTime.MinValue && start > end) // The spreadsheet will still process but have empty/default values
        {
            string value = $"{start:d}, {end:d}";
            context.AddError(EmployeeValidationInfo.InvalidEmployeeAopDates(), value);
        }
    }
    
    protected static void ValidateHolidayEntitlement(ValidatorContext context, decimal holiday)
    {
        string value = holiday.ToString(CultureInfo.InvariantCulture);
        
        if (!HolidayFormatRegex().IsMatch(value))
        {
            context.AddError(EmployeeHolidayValidationInfo.InvalidContractedHolidayEntitlementFormat(), value);
        }
        
        if (holiday is < 0 or > 365)
        {
            context.AddError(EmployeeHolidayValidationInfo.InvalidContractedHolidayEntitlementRange(), value);
        }
    }
    
    protected static void ValidateHolidayCarriedForward(ValidatorContext context, decimal holiday)
    {
        string value = holiday.ToString(CultureInfo.InvariantCulture);
        
        if (!HolidayFormatRegex().IsMatch(value))
        {
            context.AddError(EmployeeHolidayValidationInfo.InvalidHolidayCarriedForwardFormat(), value);
        }
        
        if (holiday is < 0 or > 365)
        {
            context.AddError(EmployeeHolidayValidationInfo.InvalidHolidayCarriedForwardRange(), value);
        }
    }
    
    protected static void ValidateHolidayDaysTaken(ValidatorContext context, decimal holiday)
    {
        string value = holiday.ToString(CultureInfo.InvariantCulture);
        
        if (!HolidayFormatRegex().IsMatch(value))
        {
            context.AddError(EmployeeHolidayValidationInfo.InvalidHolidayDaysTakenFormat(), value);
        }
        
        if (holiday is < 0 or > 365)
        {
            context.AddError(EmployeeHolidayValidationInfo.InvalidHolidayDaysTakenRange(), value);
        }
    }
    
    protected static void ValidateHolidayDaysOwed(ValidatorContext context, decimal holiday)
    {
        string value = holiday.ToString(CultureInfo.InvariantCulture);
        
        if (!HolidayOwedFormatRegex().IsMatch(value))
        {
            context.AddError(EmployeeHolidayValidationInfo.InvalidHolidayOwedFormat(), value);
        }
        
        if (holiday is < 0 or > 365)
        {
            context.AddError(EmployeeHolidayValidationInfo.InvalidHolidayOwedRange(), value);
        }
    }
    
    protected static void ValidateHolidayNotPaidDates(ValidatorContext context, DateTime start, DateTime end)
    {
        if (start != DateTime.MinValue && start > end) // The spreadsheet will still process but have empty/default values
        {
            string value = $"{start:d}, {end:d}";
            context.AddError(EmployeeHolidayValidationInfo.InvalidHolidayNotPaidRange(), value);
        }
    }
    
    [GeneratedRegex(ValidationInfo.AverageHoursFormat)]
    private static partial Regex AverageHoursFormatRegex();
    
    [GeneratedRegex(ValidationInfo.NinoFormat)]
    private static partial Regex NinoFormatRegex();
    
    [GeneratedRegex(ValidationInfo.MoneyFormat)]
    private static partial Regex MoneyFormatRegex();
    
    [GeneratedRegex(ValidationInfo.HolidayFormat)]
    private static partial Regex HolidayFormatRegex();
    
    [GeneratedRegex(ValidationInfo.HolidayOwedFormat)]
    private static partial Regex HolidayOwedFormatRegex();
}