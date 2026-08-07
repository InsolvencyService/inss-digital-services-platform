namespace Inss.Platform.Ipus.Domain.Validation;

public static class BusinessValidationInfo
{
    public static ValidationInfo MissingBusinessName() => new()
    {
        Key = nameof(MissingBusinessName),
        Category = "Business",
        Property = "Name of business",
        SingularErrorPattern = "1 business name is missing",
        PluralErrorPattern = "[COUNT] business names are missing"
    };
    
    public static ValidationInfo InvalidBusinessNameLength() => new()
    {
        Key = nameof(InvalidBusinessNameLength),
        Category = "Business",
        Property = "Name of business",
        SingularErrorPattern = "1 business name is the wrong length",
        PluralErrorPattern = "[COUNT] business names are the wrong length",
        Hint = "Enter up to 60 characters"
    };
    
    public static ValidationInfo InvalidNatureOfBusinessLength() => new()
    {
        Key = nameof(InvalidNatureOfBusinessLength),
        Category = "Business",
        Property = "Nature of business",
        SingularErrorPattern = "1 nature of business is the wrong length",
        PluralErrorPattern = "[COUNT] nature of businesses are the wrong length",
        Hint = "Enter up to 100 characters"
    };
    
    public static ValidationInfo InvalidCompanyNumberLength() => new()
    {
        Key = nameof(InvalidCompanyNumberLength),
        Category = "Business",
        Property = "Company number",
        SingularErrorPattern = "1 company number is the wrong length",
        PluralErrorPattern = "[COUNT] company numbers are the wrong length",
        Hint = "Enter up to 12 characters"
    };
    
    public static ValidationInfo InvalidSICLength() => new()
    {
        Key = nameof(InvalidSICLength),
        Category = "Business",
        Property = "Standard industrial classification",
        SingularErrorPattern = "1 standard industrial classification is the wrong length",
        PluralErrorPattern = "[COUNT] standard industrial classifications are the wrong length",
        Hint = "Enter up to 255 characters"
    };
    
    public static ValidationInfo InvalidPayeFormat() => new()
    {
        Key = nameof(InvalidPayeFormat),
        Category = "Business",
        Property = "PAYE reference",
        SingularErrorPattern = "1 PAYE reference is in the wrong format",
        PluralErrorPattern = "[COUNT] PAYE reference are in the wrong format",
        Hint = "Enter district/reference e.g. 123/AA45678"
    };
    
    public static ValidationInfo InvalidPayeDistrictLength() => new()
    {
        Key = nameof(InvalidPayeDistrictLength),
        Category = "Business",
        Property = "PAYE reference",
        SingularErrorPattern = "1 PAYE reference district is the wrong length",
        PluralErrorPattern = "[COUNT] PAYE reference district are the wrong length",
        Hint = "Enter up to 3 characters"
    };
    
    public static ValidationInfo InvalidPayeReferenceLength() => new()
    {
        Key = nameof(InvalidPayeReferenceLength),
        Category = "Business",
        Property = "PAYE reference",
        SingularErrorPattern = "1 PAYE reference is the wrong length",
        PluralErrorPattern = "[COUNT] PAYE reference are the wrong length",
        Hint = "Enter up to 7 characters"
    };
}