namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

public static class CaseValidationInfo
{
    public static ValidationInfo CaseReferenceMismatch() => new()
    {
        Key = nameof(CaseReferenceMismatch),
        Category = "Case",
        Property = "Case reference",
        SingularErrorPattern = "1 case reference does not match the validated case reference",
        PluralErrorPattern = "[COUNT] case references do not match the validated case reference"
    };
}