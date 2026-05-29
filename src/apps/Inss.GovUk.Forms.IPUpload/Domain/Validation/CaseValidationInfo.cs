namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

internal static class CaseValidationInfo
{
    internal static readonly ValidationInfo UnknownCaseReference = new()
    {
        Key = nameof(UnknownCaseReference),
        Category = "Case",
        Property = "Case reference",
        SingularErrorPattern = "[1] case reference was not found",
        PluralErrorPattern = "[COUNT] case references were not found"
    };
    
    internal static readonly ValidationInfo MissingCaseReference = new()
    {
        Key = nameof(MissingCaseReference),
        Category = "Case",
        Property = "Case reference",
        SingularErrorPattern = "[1] case reference is missing",
        PluralErrorPattern = "[COUNT] case references are missing",
        Hint = "Enter a reference number like CN12345678"
    };
    
    internal static readonly ValidationInfo InvalidCaseReferenceFormat = new()
    {
        Key = nameof(InvalidCaseReferenceFormat),
        Category = "Case",
        Property = "Case reference",
        SingularErrorPattern = "[1] case reference is in the wrong format",
        PluralErrorPattern = "[COUNT] case references are in the wrong format"
    };
    
    internal static readonly ValidationInfo InvalidCaseReferenceLength = new()
    {
        Key = nameof(InvalidCaseReferenceLength),
        Category = "Case",
        Property = "Case reference",
        SingularErrorPattern = "[1] case references are too long",
        PluralErrorPattern = "[COUNT] case references are too long",
        Hint = "Enter up to 12 characters"
    };
}