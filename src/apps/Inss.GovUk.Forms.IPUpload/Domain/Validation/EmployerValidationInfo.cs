namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

public static class EmployerValidationInfo
{
    public static ValidationInfo InvalidEmployerNameLength() => new()
    {
        Key = nameof(InvalidEmployerNameLength),
        Category = "Employer",
        Property = "Employer name",
        SingularErrorPattern = "[1] employer names is the wrong length",
        PluralErrorPattern = "[COUNT] employer names are the wrong length",
        Hint = "Enter up to 99 characters"
    };
}