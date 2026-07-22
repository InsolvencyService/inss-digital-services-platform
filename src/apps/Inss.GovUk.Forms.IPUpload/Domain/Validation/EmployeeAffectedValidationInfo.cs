namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

public static class EmployeeAffectedValidationInfo
{
    public static ValidationInfo InvalidEmployeeCountLength() => new()
    {
        Key = nameof(InvalidEmployeeCountLength),
        Category = "Employee",
        Property = "Employees affected by redundancy",
        SingularErrorPattern = "1 employee affected by redundancy is wrong length",
        PluralErrorPattern = "[COUNT] employee affected by redundancy are wrong length",
        Hint = "Enter a value between 0 and 1,000,000,000"
    };
}