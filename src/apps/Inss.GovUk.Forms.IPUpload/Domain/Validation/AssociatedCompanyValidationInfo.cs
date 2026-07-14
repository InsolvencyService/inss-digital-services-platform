namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

public static class AssociatedCompanyValidationInfo
{
    public static ValidationInfo InvalidAssociatedCompanyNameLength() => new()
    {
        Key = nameof(InvalidAssociatedCompanyNameLength),
        Category = "Associated company",
        Property = "Associated company name",
        SingularErrorPattern = "1 associated company name is the wrong length",
        PluralErrorPattern = "[COUNT] associated company names are the wrong length",
        Hint = "Enter up to 60 characters"
    };
    
    public static ValidationInfo InvalidAssociatedCompanyNumberLength() => new()
    {
        Key = nameof(InvalidAssociatedCompanyNumberLength),
        Category = "Associated company",
        Property = "Associated company number",
        SingularErrorPattern = "1 associated company number is the wrong length",
        PluralErrorPattern = "[COUNT] associated company numbers are the wrong length",
        Hint = "Enter up to 9 characters"
    };
    
    public static ValidationInfo InvalidAssociationReasonLength() => new()
    {
        Key = nameof(InvalidAssociationReasonLength),
        Category = "Associated company",
        Property = "Reason for association",
        SingularErrorPattern = "1 reason for association is the wrong length",
        PluralErrorPattern = "[COUNT] reason for associations are the wrong length",
        Hint = "Enter up to 255 characters"
    };
}