namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

internal static class EmployeePayValidationInfo
{
    internal static ValidationInfo InvalidEmployeeBasicPayFormat() => new()
    {
        Key = nameof(InvalidEmployeeBasicPayFormat),
        Category = "Employee pay",
        Property = "Employee basic pay per week",
        SingularErrorPattern = "[1] weekly pay amount is incorrect",
        PluralErrorPattern = "[COUNT] weekly pay amounts are incorrect",
        Hint = "Enter a number like 12.34 or 100"
    };
}