namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

internal static class EmploymentContinuityValidationInfo
{
    internal static readonly ValidationInfo InvalidContinuityEmployerNameLength = new()
    {
        Key = nameof(InvalidContinuityEmployerNameLength),
        Category = "Employment continuity",
        Property = "Employer name",
        SingularErrorPattern = "[1] employer name is the wrong length",
        PluralErrorPattern = "[COUNT] employer names are the wrong length",
        Hint = "Enter up to 60 characters"
    };
}