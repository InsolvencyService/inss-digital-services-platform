namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

public static class InsolvencyPractitionerValidationInfo
{
    public static ValidationInfo InvalidIPRegistrationNumberLength() => new()
    {
        Key = nameof(InvalidIPRegistrationNumberLength),
        Category = "Insolvency practitioner",
        Property = "Registration number",
        SingularErrorPattern = "[1] registration number is the wrong length",
        PluralErrorPattern = "[COUNT] registration numbers are the wrong length",
        Hint = "Enter up to 9 characters"
    };
    
    public static ValidationInfo InvalidIPFirmNameLength() => new()
    {
        Key = nameof(InvalidIPFirmNameLength),
        Category = "Insolvency practitioner",
        Property = "Firm number",
        SingularErrorPattern = "[1] firm name is the wrong length",
        PluralErrorPattern = "[COUNT] firm names are the wrong length",
        Hint = "Enter up to 255 characters"
    };
    
    public static ValidationInfo InvalidIPNameLength() => new()
    {
        Key = nameof(InvalidIPNameLength),
        Category = "Insolvency practitioner",
        Property = "Insolvency practitioner name",
        SingularErrorPattern = "[1] insolvency practitioner name is the wrong length",
        PluralErrorPattern = "[COUNT] insolvency practitioner names are the wrong length",
        Hint = "Enter up to 60 characters"
    };
    
    public static ValidationInfo InvalidIPEmailLength() => new()
    {
        Key = nameof(InvalidIPEmailLength),
        Category = "Insolvency practitioner",
        Property = "Insolvency practitioner email address",
        SingularErrorPattern = "[1] insolvency practitioner email address is the wrong length",
        PluralErrorPattern = "[COUNT] insolvency practitioner email addresses are the wrong length",
        Hint = "Enter up to 100 characters"
    };
    
    public static ValidationInfo InvalidIPPhoneLength() => new()
    {
        Key = nameof(InvalidIPPhoneLength),
        Category = "Insolvency practitioner",
        Property = "Insolvency practitioner phone number",
        SingularErrorPattern = "[1] insolvency practitioner phone number is the wrong length",
        PluralErrorPattern = "[COUNT] insolvency practitioner phone numbers are the wrong length",
        Hint = "Enter up to 12 characters"
    };
}