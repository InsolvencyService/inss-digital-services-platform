namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

internal static class PayRecordsContactValidationInfo
{
    internal static readonly ValidationInfo MissingPayRecordName = new()
    {
        Key = nameof(MissingPayRecordName),
        Category = "Pay records contact",
        Property = "Pay records contact name",
        SingularErrorPattern = "[1] pay records contact name is missing",
        PluralErrorPattern = "[COUNT] pay records contact names are missing"
    };
    
    internal static readonly ValidationInfo InvalidPayRecordNameLength = new()
    {
        Key = nameof(InvalidPayRecordNameLength),
        Category = "Pay records contact",
        Property = "Pay records contact name",
        SingularErrorPattern = "[1] pay records contact name is the wrong length",
        PluralErrorPattern = "[COUNT] pay records contact names are the wrong length",
        Hint = "Enter up to 60 characters"
    };
    
    internal static readonly ValidationInfo InvalidPayRecordPhoneLength = new()
    {
        Key = nameof(InvalidPayRecordPhoneLength),
        Category = "Pay records contact",
        Property = "Pay records contact phone number",
        SingularErrorPattern = "[1] pay records contact phone number is the wrong length",
        PluralErrorPattern = "[COUNT] pay records contact phone numbers are the wrong length",
        Hint = "Enter up to 12 characters"
    };
    
    internal static readonly ValidationInfo InvalidPayRecordEmailLength = new()
    {
        Key = nameof(InvalidPayRecordEmailLength),
        Category = "Pay records contact",
        Property = "Pay records contact email address",
        SingularErrorPattern = "[1] pay records contact email address is the wrong length",
        PluralErrorPattern = "[COUNT] pay records contact email addresses are the wrong length",
        Hint = "Enter up to 100 characters"
    };
}