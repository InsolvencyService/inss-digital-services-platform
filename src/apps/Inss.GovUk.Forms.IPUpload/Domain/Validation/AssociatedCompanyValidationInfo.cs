namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

internal static class AssociatedCompanyValidationInfo
{
    internal static readonly ValidationInfo InvalidAssociatedCompanyNameLength = new()
    {
        Key = nameof(InvalidAssociatedCompanyNameLength),
        Category = "Associated company",
        Property = "Associated company name",
        SingularErrorPattern = "[1] associated company name is the wrong length",
        PluralErrorPattern = "[COUNT] associated company names are the wrong length",
        Hint = "Enter up to 60 characters"
    };
    
    internal static readonly ValidationInfo InvalidAssociatedCompanyNumberLength = new()
    {
        Key = nameof(InvalidAssociatedCompanyNumberLength),
        Category = "Associated company",
        Property = "Associated company number",
        SingularErrorPattern = "[1] associated company number is the wrong length",
        PluralErrorPattern = "[COUNT] associated company numbers are the wrong length",
        Hint = "Enter up to 255 characters"
    };
    
    internal static readonly ValidationInfo InvalidAssociationReasonLength = new()
    {
        Key = nameof(InvalidAssociationReasonLength),
        Category = "Associated company",
        Property = "Associated company number",
        SingularErrorPattern = "[1] association reason is the wrong length",
        PluralErrorPattern = "[COUNT] association reasons are the wrong length",
        Hint = "Enter up to 9 characters"
    };
}