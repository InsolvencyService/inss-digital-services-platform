using System.Globalization;
using FluentValidation;
using Inss.GovUk.Forms.IPUpload.Domain.Employee.Spreadsheet;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

public static class ValidatorConstants
{
    public const string CaseReferenceFormat = "CN[0-9]{8}|cn[0-9]{8}|Cn[0-9]{8}|cN[0-9]{8}";
    public const string NinoFormat = "[A-CEGHJ-PR-TW-Za-ceghj-pr-tw-z]{1}[A-CEGHJ-NPR-TW-Za-ceghj-npr-tw-z]{1}[0-9]{6}[A-DFMa-dfm]{1}";
    public const string MoneyFormat = @"^\d+(\.\d{2})?$";
}

public readonly struct RP14AValidation
{
    public static readonly RP14AValidation MissingCaseReference = new("Case", "Case reference", "[COUNT] missing a case reference");
    public static readonly RP14AValidation InvalidCaseReferenceFormat = new("Case", "Case reference", "[COUNT] invalid case reference format", "Format is CN12345678");
    public static readonly RP14AValidation InvalidCaseReferenceLength = new("Case", "Case reference", "[COUNT] too long case reference", "Up to 12 characters are allowed");
    
    public static readonly RP14AValidation InvalidEmployerNameLength = new("Employer", "Employer name", "[COUNT] invalid length of the employer name", "Maximum of 99 characters allowed");
    
    public static readonly RP14AValidation MissingEmployeeSurname = new("Employee", "Employee surname", "[COUNT] missing employee surname");
    public static readonly RP14AValidation InvalidEmployeeSurnameLength = new("Employee", "Employee surname", "[COUNT] invalid length of the employee surname", "Maximum of 99 characters allowed");
    
    public static readonly RP14AValidation MissingNino = new("Employee", "Employee national insurance number", "[COUNT] missing employee national insurance number");
    public static readonly RP14AValidation InvalidNinoFormat = new("Employee", "Employee national insurance number", "[COUNT] invalid employee national insurance number format", "Format is CN12345678");
    
    public static readonly RP14AValidation InvalidAopOwedFormat = new("Employee", "Employee arrears of payment owed", "[COUNT] invalid arrears of pay owed", "Expected format is 12.34 or 100");
    
    public static readonly RP14AValidation InvalidMoneyOwedFormat = new("Employee", "Money owed to employer", "[COUNT] invalid money owed to employer", "Expected format is 12.34 or 100");
    
    public static readonly RP14AValidation InvalidEmploymentDates = new("Employee", "Employee employment dates", "[COUNT] invalid employment dates for the employee", "Start date must be before the end date");
    
    public static readonly RP14AValidation InvalidAopDates = new("Employee", "Employee arrears of payment dates", "[COUNT] invalid arrears of dates", "Start date must be before the end date");
    
    public static readonly RP14AValidation InvalidBasicPayFormat = new("Employee pay", "Employee basic pay per week", "[COUNT] invalid basic pay per week", "Expected format is 12.34 or 100");
    
    public static readonly RP14AValidation InvalidHolidayEntitlementFormat = new("Employee holiday", "Contracted holiday entitlement", "[COUNT] invalid contracted holiday entitlement", "Expected format is 28.25 or 33");
    public static readonly RP14AValidation InvalidHolidayEntitlementRange = new("Employee holiday", "Contracted holiday entitlement", "[COUNT] invalid range of contracted holiday entitlement", "0 to 365 days allowed");
    
    public static readonly RP14AValidation InvalidHolidayCarriedForwardFormat = new("Employee holiday", "Holiday carried forward", "[COUNT] invalid holiday carried forward", "Expected format is 28.25 or 33");
    public static readonly RP14AValidation InvalidHolidayCarriedForwardRange = new("Employee holiday", "Holiday carried forward", "[COUNT] invalid range of holiday carried forward", "0 to 365 days allowed");
    
    public static readonly RP14AValidation InvalidHolidayDaysTakenFormat = new("Employee holiday", "Holiday days taken", "[COUNT] invalid holiday days taken", "Expected format is 28.25 or 33");
    public static readonly RP14AValidation InvalidHolidayDaysTakenRange = new("Employee holiday", "Holiday days taken", "[COUNT] invalid range of holiday days taken", "0 to 365 days allowed");
    
    public static readonly RP14AValidation InvalidHolidayOwedFormat = new("Employee holiday", "Holiday owed", "[COUNT] invalid holiday owed", "Expected format is 28.25 or 33");
    public static readonly RP14AValidation InvalidHolidayOwedRange = new("Employee holiday", "Holiday owed", "[COUNT] invalid range of holiday owed", "0 to 365 days allowed");
    
    public static readonly RP14AValidation InvalidHolidayNotPaidRange = new("Employee holiday", "Holiday not paid", "[COUNT] invalid holiday not paid of dates", "Start date must be before the end date");
    
    public RP14AValidation(string category, string property, string error, string? hint = null)
    {
        PropertyFormat = $"{category}|{property}";
        ErrorFormat = !string.IsNullOrWhiteSpace(hint) ? $"{error}|{hint}" : error;
    }
    
    public string PropertyFormat { get; }
    public string ErrorFormat { get; }
    
    public static string GetCategory(string value)
    {
        string[] parts = value.Split('|');
        return parts.Length > 0 ? parts[0] : "No category defined";
    }
    
    public static string GetProperty(string value)
    {
        string[] parts = value.Split('|');
        return parts.Length == 2 ? parts[1] : "No property defined";
    }
    
    public static string GetError(string value)
    {
        string[] parts = value.Split('|');
        return parts.Length > 0 ? parts[0] : "No error defined";
    }
    
    public static string? GetHint(string value)
    {
        string[] parts = value.Split('|');
        return parts.Length == 2 ? parts[1] : null;
    }
} 

public static class CustomStringRules
{
    public static IRuleBuilderOptions<T, string> ValidateCaseReference<T>(this IRuleBuilder<T, string> rule)
    {
        /*
         RuleFor(p => p)
            .NotEmpty()
            .OverridePropertyName(RP14AValidation.MissingCaseReference.PropertyFormat)
            .WithMessage(RP14AValidation.MissingCaseReference.ErrorFormat);
        RuleFor(p => p)
            .Matches(ValidatorConstants.CaseReferenceFormat)
            .OverridePropertyName(RP14AValidation.InvalidCaseReferenceFormat.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidCaseReferenceFormat.ErrorFormat);
        RuleFor(p => p)
            .MaximumLength(12)
            .OverridePropertyName(RP14AValidation.InvalidCaseReferenceLength.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidCaseReferenceLength.ErrorFormat);
         */
        return rule
            .NotEmpty()
            .OverridePropertyName(RP14AValidation.MissingCaseReference.PropertyFormat)
            .WithMessage(RP14AValidation.MissingCaseReference.ErrorFormat)
            .Matches(ValidatorConstants.CaseReferenceFormat)
            .OverridePropertyName(RP14AValidation.InvalidCaseReferenceFormat.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidCaseReferenceFormat.ErrorFormat)
            .MaximumLength(12)
            .OverridePropertyName(RP14AValidation.InvalidCaseReferenceLength.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidCaseReferenceLength.ErrorFormat);
    }
}

public class RP14ASpreadsheetEmployeeValidator : AbstractValidator<RP14AEmployee>
{
    public RP14ASpreadsheetEmployeeValidator()
    {
        RuleFor(p => p.Header.CaseReference).ValidateCaseReference();
        /*RuleFor(p => p.Header.CaseReference)
            .NotEmpty()
            .OverridePropertyName(RP14AValidation.MissingCaseReference.PropertyFormat)
            .WithMessage(RP14AValidation.MissingCaseReference.ErrorFormat);
        RuleFor(p => p.Header.CaseReference)
            .Matches(ValidatorConstants.CaseReferenceFormat)
            .OverridePropertyName(RP14AValidation.InvalidCaseReferenceFormat.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidCaseReferenceFormat.ErrorFormat);
        RuleFor(p => p.Header.CaseReference)
            .MaximumLength(12)
            .OverridePropertyName(RP14AValidation.InvalidCaseReferenceLength.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidCaseReferenceLength.ErrorFormat);*/
        
        RuleFor(p => p.EmployerName)
            .MaximumLength(99)
            .OverridePropertyName(RP14AValidation.InvalidEmployerNameLength.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidEmployerNameLength.ErrorFormat);
        
        RuleFor(p => p.EmployeeName.Surname)
            .NotEmpty()
            .OverridePropertyName(RP14AValidation.MissingEmployeeSurname.PropertyFormat)
            .WithMessage(RP14AValidation.MissingEmployeeSurname.ErrorFormat);
        RuleFor(p => p.EmployeeName.Surname)
            .MaximumLength(99)
            .OverridePropertyName(RP14AValidation.InvalidEmployeeSurnameLength.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidEmployeeSurnameLength.ErrorFormat);
        
        RuleFor(p => p.NINO)
            .NotEmpty()
            .OverridePropertyName(RP14AValidation.MissingNino.PropertyFormat)
            .WithMessage(RP14AValidation.MissingNino.ErrorFormat);
        RuleFor(p => p.NINO)
            .Matches(ValidatorConstants.NinoFormat)
            .OverridePropertyName(RP14AValidation.InvalidNinoFormat.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidNinoFormat.ErrorFormat);
        
        RuleFor(p => p.PayDetails.ArrearsOfPay.ArrearsOfPayPeriod1.AOPOwed1.ToString(CultureInfo.CurrentCulture))
            .Matches(ValidatorConstants.MoneyFormat)
            .OverridePropertyName(RP14AValidation.InvalidAopOwedFormat.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidAopOwedFormat.ErrorFormat);
        RuleFor(p => p.PayDetails.ArrearsOfPay.ArrearsOfPayPeriod2.AOPOwed2.ToString(CultureInfo.CurrentCulture))
            .Matches(ValidatorConstants.MoneyFormat)
            .OverridePropertyName(RP14AValidation.InvalidAopOwedFormat.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidAopOwedFormat.ErrorFormat);
        RuleFor(p => p.PayDetails.ArrearsOfPay.ArrearsOfPayPeriod3.AOPOwed3.ToString(CultureInfo.CurrentCulture))
            .Matches(ValidatorConstants.MoneyFormat)
            .OverridePropertyName(RP14AValidation.InvalidAopOwedFormat.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidAopOwedFormat.ErrorFormat);
        RuleFor(p => p.PayDetails.ArrearsOfPay.ArrearsOfPayPeriod4.AOPOwed4.ToString(CultureInfo.CurrentCulture))
            .Matches(ValidatorConstants.MoneyFormat)
            .OverridePropertyName(RP14AValidation.InvalidAopOwedFormat.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidAopOwedFormat.ErrorFormat);
        
        RuleFor(p => p.MoneyOwedToEmployer.ToString(CultureInfo.CurrentCulture))
            .Matches(ValidatorConstants.MoneyFormat)
            .OverridePropertyName(RP14AValidation.InvalidMoneyOwedFormat.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidMoneyOwedFormat.ErrorFormat);
        
        RuleFor(p => p).Custom((p, c) =>
        {
            if (p.StartDate > p.EndDate)
            {
                c.AddFailure(RP14AValidation.InvalidEmploymentDates.PropertyFormat, RP14AValidation.InvalidEmploymentDates.ErrorFormat);
            }
        });
        
        RuleFor(p => p.PayDetails.ArrearsOfPay.ArrearsOfPayPeriod1).Custom((p, c) =>
        {
            if (p.AOP1StartDate > p.AOP1EndDate)
            {
                c.AddFailure(RP14AValidation.InvalidAopDates.PropertyFormat, RP14AValidation.InvalidAopDates.ErrorFormat);
            }
        });
        
        RuleFor(p => p.PayDetails.ArrearsOfPay.ArrearsOfPayPeriod2).Custom((p, c) =>
        {
            if (p.AOP2StartDate > p.AOP2EndDate)
            {
                c.AddFailure(RP14AValidation.InvalidAopDates.PropertyFormat, RP14AValidation.InvalidAopDates.ErrorFormat);
            }
        });
        
        RuleFor(p => p.PayDetails.ArrearsOfPay.ArrearsOfPayPeriod3).Custom((p, c) =>
        {
            if (p.AOP3StartDate > p.AOP3EndDate)
            {
                c.AddFailure(RP14AValidation.InvalidAopDates.PropertyFormat, RP14AValidation.InvalidAopDates.ErrorFormat);
            }
        });
        
        RuleFor(p => p.PayDetails.ArrearsOfPay.ArrearsOfPayPeriod4).Custom((p, c) =>
        {
            if (p.AOP4StartDate > p.AOP4EndDate)
            {
                c.AddFailure(RP14AValidation.InvalidAopDates.PropertyFormat, RP14AValidation.InvalidAopDates.ErrorFormat);
            }
        });
        
        RuleFor(p => p.PayDetails.BasicPayPerWeek.ToString(CultureInfo.CurrentCulture))
            .Matches(ValidatorConstants.MoneyFormat)
            .OverridePropertyName(RP14AValidation.InvalidBasicPayFormat.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidBasicPayFormat.ErrorFormat);
        
        
        RuleFor(p => p.Holiday.HolidayContractedEntitlementDays.ToString(CultureInfo.CurrentCulture))
            .Matches(ValidatorConstants.MoneyFormat)
            .OverridePropertyName(RP14AValidation.InvalidHolidayEntitlementFormat.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidHolidayEntitlementFormat.ErrorFormat);
        RuleFor(p => p.Holiday.HolidayContractedEntitlementDays)
            .InclusiveBetween(0, 365)
            .OverridePropertyName(RP14AValidation.InvalidHolidayEntitlementRange.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidHolidayEntitlementRange.ErrorFormat);
        
        RuleFor(p => p.Holiday.HolidayDaysCarriedForward.ToString(CultureInfo.CurrentCulture))
            .Matches(ValidatorConstants.MoneyFormat)
            .OverridePropertyName(RP14AValidation.InvalidHolidayCarriedForwardFormat.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidHolidayCarriedForwardFormat.ErrorFormat);
        RuleFor(p => p.Holiday.HolidayDaysCarriedForward)
            .InclusiveBetween(0, 365)
            .OverridePropertyName(RP14AValidation.InvalidHolidayCarriedForwardRange.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidHolidayCarriedForwardRange.ErrorFormat);

        RuleFor(p => p.Holiday.HolidayDaysTaken.ToString(CultureInfo.CurrentCulture))
            .Matches(ValidatorConstants.MoneyFormat)
            .OverridePropertyName(RP14AValidation.InvalidHolidayDaysTakenFormat.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidHolidayDaysTakenFormat.ErrorFormat);
        RuleFor(p => p.Holiday.HolidayDaysTaken)
            .InclusiveBetween(0, 365)
            .OverridePropertyName(RP14AValidation.InvalidHolidayDaysTakenRange.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidHolidayDaysTakenRange.ErrorFormat);

        RuleFor(p => p.Holiday.NoDaysHolidayOwed.ToString(CultureInfo.CurrentCulture))
            .Matches(ValidatorConstants.MoneyFormat)
            .OverridePropertyName(RP14AValidation.InvalidHolidayOwedFormat.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidHolidayOwedFormat.ErrorFormat);
        RuleFor(p => p.Holiday.NoDaysHolidayOwed)
            .InclusiveBetween(0, 365)
            .OverridePropertyName(RP14AValidation.InvalidHolidayOwedRange.PropertyFormat)
            .WithMessage(RP14AValidation.InvalidHolidayOwedRange.ErrorFormat);

        RuleFor(p => p.Holiday.HolidayNotPaid.Holiday1).Custom((p, c) =>
        {
            if (p.Holiday1StartDate > p.Holiday1EndDate)
            {
                c.AddFailure(RP14AValidation.InvalidHolidayNotPaidRange.PropertyFormat, RP14AValidation.InvalidHolidayNotPaidRange.ErrorFormat);
            }
        });
        
        RuleFor(p => p.Holiday.HolidayNotPaid.Holiday2).Custom((p, c) =>
        {
            if (p.Holiday2StartDate > p.Holiday2EndDate)
            {
                c.AddFailure(RP14AValidation.InvalidHolidayNotPaidRange.PropertyFormat, RP14AValidation.InvalidHolidayNotPaidRange.ErrorFormat);
            }
        });
        
        RuleFor(p => p.Holiday.HolidayNotPaid.Holiday3).Custom((p, c) =>
        {
            if (p.Holiday3StartDate > p.Holiday3EndDate)
            {
                c.AddFailure(RP14AValidation.InvalidHolidayNotPaidRange.PropertyFormat, RP14AValidation.InvalidHolidayNotPaidRange.ErrorFormat);
            }
        });
    }
}