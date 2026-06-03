namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

internal static class BusinessValidationInfo
{
    internal static ValidationInfo MissingBusinessName() => new()
    {
        Key = nameof(MissingBusinessName),
        Category = "Business",
        Property = "Name of business",
        SingularErrorPattern = "[1] business name is missing",
        PluralErrorPattern = "[COUNT] business names are missing"
    };
    
    internal static ValidationInfo InvalidBusinessNameLength() => new()
    {
        Key = nameof(InvalidBusinessNameLength),
        Category = "Business",
        Property = "Name of business",
        SingularErrorPattern = "[1] business name is the wrong length",
        PluralErrorPattern = "[COUNT] business names are the wrong length",
        Hint = "Enter up to 60 characters"
    };
    
    internal static ValidationInfo InvalidNatureOfBusinessLength() => new()
    {
        Key = nameof(InvalidNatureOfBusinessLength),
        Category = "Business",
        Property = "Nature of business",
        SingularErrorPattern = "[1] nature of business is the wrong length",
        PluralErrorPattern = "[COUNT] nature of businesses are the wrong length",
        Hint = "Enter up to 100 characters"
    };
    
    internal static ValidationInfo InvalidCompanyNumberLength() => new()
    {
        Key = nameof(InvalidCompanyNumberLength),
        Category = "Business",
        Property = "Company number",
        SingularErrorPattern = "[1] company number is the wrong length",
        PluralErrorPattern = "[COUNT] company numbers are the wrong length",
        Hint = "Enter up to 12 characters"
    };
    
    internal static ValidationInfo InvalidSICLength() => new()
    {
        Key = nameof(InvalidSICLength),
        Category = "Business",
        Property = "Standard industrial classification",
        SingularErrorPattern = "[1] standard industrial classification is the wrong length",
        PluralErrorPattern = "[COUNT] standard industrial classifications are the wrong length",
        Hint = "Enter up to 255 characters"
    };
}