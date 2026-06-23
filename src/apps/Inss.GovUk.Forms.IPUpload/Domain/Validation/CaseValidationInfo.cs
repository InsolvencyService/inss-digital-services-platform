namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

public static class CaseValidationInfo
{
    public static ValidationInfo CaseReferenceMismatch(string validatedCaseReference) => new()
    {
        Key = nameof(CaseReferenceMismatch),
        Category = "Case",
        Property = "Case reference",
        SingularErrorPattern = $"1 case reference does not match the validated case reference {validatedCaseReference}",
        PluralErrorPattern = $"[COUNT] case references do not match the validated case reference {validatedCaseReference}"
    };
}