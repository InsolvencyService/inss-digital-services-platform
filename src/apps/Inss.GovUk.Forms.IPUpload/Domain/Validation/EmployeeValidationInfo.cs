namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

internal static class EmployeeValidationInfo
{
    internal static readonly ValidationInfo MissingEmployeeSurname = new()
    {
        Key = nameof(MissingEmployeeSurname),
        Category = "Employee",
        Property = "Employee surname",
        SingularErrorPattern = "[1] employee surname is missing",
        PluralErrorPattern = "[COUNT] employee surnames are missing"
    };

    internal static readonly ValidationInfo InvalidEmployeeSurnameLength = new()
    {
        Key = nameof(InvalidEmployeeSurnameLength),
        Category = "Employee",
        Property = "Employee surname",
        SingularErrorPattern = "[1] employee surname is the wrong length",
        PluralErrorPattern = "[COUNT] employee surnames are the wrong length",
        Hint = "Enter up to 99 characters"
    };
    
    internal static readonly ValidationInfo InvalidAopOwedFormat = new()
    {
        Key = nameof(InvalidAopOwedFormat),
        Category = "Employee",
        Property = "Employee arrears of payment owed",
        SingularErrorPattern = "[1] arrears of pay is incorrect",
        PluralErrorPattern = "[COUNT] arrears of pay are incorrect",
        Hint = "Enter a number like 12.34 or 100"
    };
    
    internal static readonly ValidationInfo MissingEmployeeNino = new()
    {
        Key = nameof(MissingEmployeeNino),
        Category = "Employee",
        Property = "Employee national insurance number",
        SingularErrorPattern = "[1] National Insurance numbers is missing",
        PluralErrorPattern = "[COUNT] National Insurance numbers are missing"
    };
    
    internal static readonly ValidationInfo InvalidEmployeeNinoFormat = new()
    {
        Key = nameof(InvalidEmployeeNinoFormat),
        Category = "Employee",
        Property = "Employee national insurance number",
        SingularErrorPattern = "[1] National Insurance number is in the wrong format",
        PluralErrorPattern = "[COUNT] National Insurance numbers are in the wrong format",
        Hint = "Enter a National Insurance number like QQ 12 34 56 C"
    };
    
    internal static readonly ValidationInfo InvalidMoneyOwedToEmployerFormat = new()
    {
        Key = nameof(InvalidMoneyOwedToEmployerFormat),
        Category = "Employee",
        Property = "Money owed to employer",
        SingularErrorPattern = "[1] amount owed to the employer is incorrect",
        PluralErrorPattern = "[COUNT] amounts owed to the employer are incorrect",
        Hint = "Enter a number like 12.34 or 100"
    };
    
    internal static readonly ValidationInfo InvalidEmployeeEmploymentDates = new()
    {
        Key = nameof(InvalidEmployeeEmploymentDates),
        Category = "Employee",
        Property = "Employee employment dates",
        SingularErrorPattern = "[1] employment date is incorrect",
        PluralErrorPattern = "[COUNT] employment dates are incorrect",
        Hint = "Start date must be before the end date"
    };
    
    internal static readonly ValidationInfo InvalidEmployeeAopDates = new()
    {
        Key = nameof(InvalidEmployeeAopDates),
        Category = "Employee",
        Property = "Employee arrears of payment dates",
        SingularErrorPattern = "[1] arrears date is incorrect",
        PluralErrorPattern = "[COUNT] arrears dates are incorrect (or clarify intent)",
        Hint = "Start date must be before the end date"
    };
}