namespace Inss.GovUk.Forms.IPUpload.Domain.Validation.Employee;

internal sealed class RP14AValidationInfo : ValidationInfo
{
    // Case
    internal static readonly RP14AValidationInfo MissingCaseReference = new("Case", "Case reference", "[COUNT] missing a case reference");
    internal static readonly RP14AValidationInfo InvalidCaseReferenceFormat = new("Case", "Case reference", "[COUNT] invalid case reference format", "Format is CN12345678");
    internal static readonly RP14AValidationInfo InvalidCaseReferenceLength = new("Case", "Case reference", "[COUNT] too long case reference", "Up to 12 characters are allowed");
    
    // Employer
    public static readonly RP14AValidationInfo InvalidEmployerNameLength = new("Employer", "Employer name", "[COUNT] invalid length of the employer name", "Maximum of 99 characters allowed");
    
    // Employee
    internal static readonly RP14AValidationInfo MissingEmployeeSurname = new("Employee", "Employee surname", "[COUNT] missing employee surname");
    internal static readonly RP14AValidationInfo InvalidEmployeeSurnameLength = new("Employee", "Employee surname", "[COUNT] invalid length of the employee surname", "Maximum of 99 characters allowed");
    internal static readonly RP14AValidationInfo MissingNino = new("Employee", "Employee national insurance number", "[COUNT] missing employee national insurance number");
    internal static readonly RP14AValidationInfo InvalidNinoFormat = new("Employee", "Employee national insurance number", "[COUNT] invalid employee national insurance number format", "Format is CN12345678");
    internal static readonly RP14AValidationInfo InvalidAopOwedFormat = new("Employee", "Employee arrears of payment owed", "[COUNT] invalid arrears of pay owed", "Expected format is 12.34 or 100");
    internal static readonly RP14AValidationInfo InvalidMoneyOwedFormat = new("Employee", "Money owed to employer", "[COUNT] invalid money owed to employer", "Expected format is 12.34 or 100");
    internal static readonly RP14AValidationInfo InvalidEmploymentDates = new("Employee", "Employee employment dates", "[COUNT] invalid employment dates for the employee", "Start date must be before the end date");
    internal static readonly RP14AValidationInfo InvalidAopDates = new("Employee", "Employee arrears of payment dates", "[COUNT] invalid arrears of dates", "Start date must be before the end date");
    
    // Employee pay
    internal static readonly RP14AValidationInfo InvalidBasicPayFormat = new("Employee pay", "Employee basic pay per week", "[COUNT] invalid basic pay per week", "Expected format is 12.34 or 100");
    
    // Employee holiday
    internal static readonly RP14AValidationInfo InvalidHolidayEntitlementFormat = new("Employee holiday", "Contracted holiday entitlement", "[COUNT] invalid contracted holiday entitlement", "Expected format is 28.25 or 33");
    internal static readonly RP14AValidationInfo InvalidHolidayEntitlementRange = new("Employee holiday", "Contracted holiday entitlement", "[COUNT] invalid range of contracted holiday entitlement", "0 to 365 days allowed");
    internal static readonly RP14AValidationInfo InvalidHolidayCarriedForwardFormat = new("Employee holiday", "Holiday carried forward", "[COUNT] invalid holiday carried forward", "Expected format is 28.25 or 33");
    internal static readonly RP14AValidationInfo InvalidHolidayCarriedForwardRange = new("Employee holiday", "Holiday carried forward", "[COUNT] invalid range of holiday carried forward", "0 to 365 days allowed");
    internal static readonly RP14AValidationInfo InvalidHolidayDaysTakenFormat = new("Employee holiday", "Holiday days taken", "[COUNT] invalid holiday days taken", "Expected format is 28.25 or 33");
    internal static readonly RP14AValidationInfo InvalidHolidayDaysTakenRange = new("Employee holiday", "Holiday days taken", "[COUNT] invalid range of holiday days taken", "0 to 365 days allowed");
    internal static readonly RP14AValidationInfo InvalidHolidayOwedFormat = new("Employee holiday", "Holiday owed", "[COUNT] invalid holiday owed", "Expected format is 28.25 or 33");
    internal static readonly RP14AValidationInfo InvalidHolidayOwedRange = new("Employee holiday", "Holiday owed", "[COUNT] invalid range of holiday owed", "0 to 365 days allowed");
    internal static readonly RP14AValidationInfo InvalidHolidayNotPaidRange = new("Employee holiday", "Holiday not paid", "[COUNT] invalid holiday not paid of dates", "Start date must be before the end date");
    
    internal RP14AValidationInfo(string category, string property, string error, string? hint = null) : base(category, property, error, hint)
    {
    }
}