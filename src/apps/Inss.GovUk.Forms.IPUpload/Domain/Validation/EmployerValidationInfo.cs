namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

internal static class EmployerValidationInfo
{
    internal static readonly ValidationInfo InvalidEmployerNameLength = new()
    {
        Key = nameof(InvalidEmployerNameLength),
        Category = "Employer",
        Property = "Employer name",
        SingularErrorPattern = "[1] employer names is the wrong length",
        PluralErrorPattern = "[COUNT] employer names are the wrong length",
        Hint = "Enter up to 99 characters"
    };
}