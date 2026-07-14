namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

public static class EmployeeHolidayValidationInfo
{
    public static ValidationInfo InvalidContractedHolidayEntitlementFormat() => new()
    {
        Key = nameof(InvalidContractedHolidayEntitlementFormat),
        Category = "Employee holiday",
        Property = "Contracted holiday entitlement",
        SingularErrorPattern = "1 contracted holiday entitlement is incorrect",
        PluralErrorPattern = "[COUNT] contracted holiday entitlements are incorrect",
        Hint = "Enter a number like 22.5 or 33"
    };
    
    public static ValidationInfo InvalidContractedHolidayEntitlementRange() => new()
    {
        Key = nameof(InvalidContractedHolidayEntitlementRange),
        Category = "Employee holiday",
        Property = "Contracted holiday entitlement",
        SingularErrorPattern = "1 contracted holiday entitlement is outside the allowed range",
        PluralErrorPattern = "[COUNT] contracted holiday entitlements are outside the allowed range",
        Hint = "Enter a value between 0 and 365"
    };
    
    public static ValidationInfo InvalidHolidayCarriedForwardFormat() => new()
    {
        Key = nameof(InvalidHolidayCarriedForwardFormat),
        Category = "Employee holiday",
        Property = "Holiday carried forward",
        SingularErrorPattern = "1 carried forward holiday days is incorrect",
        PluralErrorPattern = "[COUNT] carried forward holiday days are incorrect",
        Hint = "Enter a number like 22.5 or 33"
    };
    
    public static ValidationInfo InvalidHolidayCarriedForwardRange() => new()
    {
        Key = nameof(InvalidHolidayCarriedForwardRange),
        Category = "Employee holiday",
        Property = "Holiday carried forward",
        SingularErrorPattern = "1 carried forward holiday days is outside the allowed range",
        PluralErrorPattern = "[COUNT] carried forward holiday days are outside the allowed range",
        Hint = "Enter a value between 0 and 365"
    };
    
    public static ValidationInfo InvalidHolidayDaysTakenFormat() => new()
    {
        Key = nameof(InvalidHolidayDaysTakenFormat),
        Category = "Employee holiday",
        Property = "Holiday days taken",
        SingularErrorPattern = "1 holiday days taken is incorrect",
        PluralErrorPattern = "[COUNT] holiday days taken are incorrect",
        Hint = "Enter a number like 22.5 or 33"
    };
    
    public static ValidationInfo InvalidHolidayDaysTakenRange() => new()
    {
        Key = nameof(InvalidHolidayDaysTakenRange),
        Category = "Employee holiday",
        Property = "Holiday days taken",
        SingularErrorPattern = "1 holiday days taken is outside the allowed range",
        PluralErrorPattern = "[COUNT] holiday days taken are outside the allowed range",
        Hint = "Enter a value between 0 and 365"
    };
    
    public static ValidationInfo InvalidHolidayOwedFormat() => new()
    {
        Key = nameof(InvalidHolidayOwedFormat),
        Category = "Employee holiday",
        Property = "Holiday owed",
        SingularErrorPattern = "1 holiday owed is incorrect",
        PluralErrorPattern = "[COUNT] holiday owed is incorrect",
        Hint = "Enter a number like 28.25 or 33"
    };
    
    public static ValidationInfo InvalidHolidayOwedRange() => new()
    {
        Key = nameof(InvalidHolidayOwedRange),
        Category = "Employee holiday",
        Property = "Holiday owed",
        SingularErrorPattern = "1 holiday owed is outside the allowed range",
        PluralErrorPattern = "[COUNT] holiday owed is outside the allowed range",
        Hint = "Enter a value between 0 and 365"
    };
    
    public static ValidationInfo InvalidHolidayNotPaidRange() => new()
    {
        Key = nameof(InvalidHolidayNotPaidRange),
        Category = "Employee holiday",
        Property = "Holiday not paid",
        SingularErrorPattern = "1 unpaid holiday dates is incorrect",
        PluralErrorPattern = "[COUNT] unpaid holiday dates are incorrect",
        Hint = "Start date must be before the end date"
    };
}