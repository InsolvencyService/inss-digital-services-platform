namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

internal static class AddressValidationInfo
{
    internal static readonly ValidationInfo InvalidAddressLinesLength = new()
    {
        Key = nameof(InvalidAddressLinesLength),
        Category = "[CATEGORY]",
        Property = "Address lines",
        SingularErrorPattern = "[1] address lines are the wrong length",
        PluralErrorPattern = "[COUNT] address lines are the wrong length",
        Hint = "Enter up to 4 address lines"
    };
    
    internal static readonly ValidationInfo InvalidAddressLineLength = new()
    {
        Key = nameof(InvalidAddressLineLength),
        Category = "[CATEGORY]",
        Property = "Address lines",
        SingularErrorPattern = "[1] address line is the wrong length",
        PluralErrorPattern = "[COUNT] address lines are the wrong length",
        Hint = "Enter up to 35 characters"
    };
    
    internal static readonly ValidationInfo InvalidAddressTownLength = new()
    {
        Key = nameof(InvalidAddressTownLength),
        Category = "[CATEGORY]",
        Property = "Address town",
        SingularErrorPattern = "[1] address town is the wrong length",
        PluralErrorPattern = "[COUNT] address towns are the wrong length",
        Hint = "Enter up to 35 characters"
    };
    
    internal static readonly ValidationInfo InvalidAddressCountyLength = new()
    {
        Key = nameof(InvalidAddressCountyLength),
        Category = "[CATEGORY]",
        Property = "Address county",
        SingularErrorPattern = "[1] address county is the wrong length",
        PluralErrorPattern = "[COUNT] address counties are the wrong length",
        Hint = "Enter up to 35 characters"
    };
    
    internal static readonly ValidationInfo InvalidAddressPostcodeLength = new()
    {
        Key = nameof(InvalidAddressPostcodeLength),
        Category = "[CATEGORY]",
        Property = "Address postcode",
        SingularErrorPattern = "[1] address postcode is the wrong length",
        PluralErrorPattern = "[COUNT] address postcodes are the wrong length",
        Hint = "Enter up to 10 characters"
    };
    
    internal static readonly ValidationInfo InvalidAddressCountryLength = new()
    {
        Key = nameof(InvalidAddressCountryLength),
        Category = "[CATEGORY]",
        Property = "Address country",
        SingularErrorPattern = "[1] address country is the wrong length",
        PluralErrorPattern = "[COUNT] address countries are the wrong length",
        Hint = "Enter up to 10 characters"
    };
}