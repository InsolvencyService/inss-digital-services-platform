namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

internal static class EmployeeHolidayValidationInfo
{
    internal static ValidationInfo MissingContractedHolidayEntitlement() => new()
    {
        Key = nameof(MissingContractedHolidayEntitlement),
        Category = "Employee holiday",
        Property = "Contracted holiday entitlement",
        SingularErrorPattern = "[1] contracted holiday entitlement is missing",
        PluralErrorPattern = "[COUNT] contracted holiday entitlements are missing"
    };
    
    internal static ValidationInfo InvalidContractedHolidayEntitlementFormat() => new()
    {
        Key = nameof(InvalidContractedHolidayEntitlementFormat),
        Category = "Employee holiday",
        Property = "Contracted holiday entitlement",
        SingularErrorPattern = "[1] contracted holiday entitlement is incorrect",
        PluralErrorPattern = "[COUNT] contracted holiday entitlements are incorrect",
        Hint = "Enter a number like 12.34 or 100"
    };
    
    internal static ValidationInfo InvalidContractedHolidayEntitlementRange() => new()
    {
        Key = nameof(InvalidContractedHolidayEntitlementRange),
        Category = "Employee holiday",
        Property = "Contracted holiday entitlement",
        SingularErrorPattern = "[1] contracted holiday entitlement is outside the allowed range",
        PluralErrorPattern = "[COUNT] contracted holiday entitlements are outside the allowed range",
        Hint = "Enter a value between 0 and 365"
    };
    
    internal static ValidationInfo MissingHolidayCarriedForward() => new()
    {
        Key = nameof(MissingHolidayCarriedForward),
        Category = "Employee holiday",
        Property = "Holiday carried forward",
        SingularErrorPattern = "[1] carried forward holiday day are missing",
        PluralErrorPattern = "[COUNT] carried forward holiday days are missing"
    };
    
    internal static ValidationInfo InvalidHolidayCarriedForwardFormat() => new()
    {
        Key = nameof(InvalidHolidayCarriedForwardFormat),
        Category = "Employee holiday",
        Property = "Holiday carried forward",
        SingularErrorPattern = "[1] carried forward holiday days is incorrect",
        PluralErrorPattern = "[COUNT] carried forward holiday days are incorrect",
        Hint = "Enter a number like 28.25 or 33"
    };
    
    internal static ValidationInfo InvalidHolidayCarriedForwardRange() => new()
    {
        Key = nameof(InvalidHolidayCarriedForwardRange),
        Category = "Employee holiday",
        Property = "Holiday carried forward",
        SingularErrorPattern = "[1] carried forward holiday days is outside the allowed range",
        PluralErrorPattern = "[COUNT] carried forward holiday days are outside the allowed range",
        Hint = "Enter a value between 0 and 365"
    };
    
    internal static ValidationInfo MissingHolidayDaysTakenForward() => new()
    {
        Key = nameof(MissingHolidayDaysTakenForward),
        Category = "Employee holiday",
        Property = "Holiday days taken",
        SingularErrorPattern = "[1] holiday days taken are missing",
        PluralErrorPattern = "[COUNT] holiday days taken are missing"
    };
    
    internal static ValidationInfo InvalidHolidayDaysTakenFormat() => new()
    {
        Key = nameof(InvalidHolidayDaysTakenFormat),
        Category = "Employee holiday",
        Property = "Holiday days taken",
        SingularErrorPattern = "[1] holiday days taken is incorrect",
        PluralErrorPattern = "[COUNT] holiday days taken are incorrect",
        Hint = "Enter a number like 28.25 or 33"
    };
    
    internal static ValidationInfo InvalidHolidayDaysTakenRange() => new()
    {
        Key = nameof(InvalidHolidayDaysTakenRange),
        Category = "Employee holiday",
        Property = "Holiday days taken",
        SingularErrorPattern = "[1] holiday days taken is outside the allowed range",
        PluralErrorPattern = "[COUNT] holiday days taken are outside the allowed range",
        Hint = "Enter a value between 0 and 365"
    };
    
    internal static ValidationInfo MissingHolidayOwedForward() => new()
    {
        Key = nameof(MissingHolidayOwedForward),
        Category = "Employee holiday",
        Property = "Holiday owed",
        SingularErrorPattern = "[1] holiday owed is missing",
        PluralErrorPattern = "[COUNT] holiday owed is missing"
    };
    
    internal static ValidationInfo InvalidHolidayOwedFormat() => new()
    {
        Key = nameof(InvalidHolidayOwedFormat),
        Category = "Employee holiday",
        Property = "Holiday owed",
        SingularErrorPattern = "[1] holiday owed is incorrect",
        PluralErrorPattern = "[COUNT] holiday owed is incorrect",
        Hint = "Enter a number like 28.25 or 33"
    };
    
    internal static ValidationInfo InvalidHolidayOwedRange() => new()
    {
        Key = nameof(InvalidHolidayOwedRange),
        Category = "Employee holiday",
        Property = "Holiday owed",
        SingularErrorPattern = "[1] holiday owed is outside the allowed range",
        PluralErrorPattern = "[COUNT] holiday owed is outside the allowed range",
        Hint = "Enter a value between 0 and 365"
    };
    
    internal static ValidationInfo InvalidHolidayNotPaidRange() => new()
    {
        Key = nameof(InvalidHolidayNotPaidRange),
        Category = "Employee holiday",
        Property = "Holiday not paid",
        SingularErrorPattern = "[1] unpaid holiday dates is incorrect",
        PluralErrorPattern = "[COUNT] unpaid holiday dates are incorrect",
        Hint = "Start date must be before the end date"
    };
}