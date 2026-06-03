namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

internal static class AddressValidationInfo
{
    internal static ValidationInfo InvalidAddressLinesLength(string category) => new()
    {
        Key = nameof(InvalidAddressLinesLength),
        Category = category,
        Property = "Address lines",
        SingularErrorPattern = "[1] address lines are the wrong length",
        PluralErrorPattern = "[COUNT] address lines are the wrong length",
        Hint = "Enter up to 4 address lines"
    };
    
    internal static ValidationInfo InvalidAddressLineLength(string category) => new()
    {
        Key = nameof(InvalidAddressLineLength),
        Category = category,
        Property = "Address lines",
        SingularErrorPattern = "[1] address line is the wrong length",
        PluralErrorPattern = "[COUNT] address lines are the wrong length",
        Hint = "Enter up to 35 characters"
    };
    
    internal static ValidationInfo InvalidAddressTownLength(string category) => new()
    {
        Key = nameof(InvalidAddressTownLength),
        Category = category,
        Property = "Address town",
        SingularErrorPattern = "[1] address town is the wrong length",
        PluralErrorPattern = "[COUNT] address towns are the wrong length",
        Hint = "Enter up to 35 characters"
    };
    
    internal static ValidationInfo InvalidAddressCountyLength(string category) => new()
    {
        Key = nameof(InvalidAddressCountyLength),
        Category = category,
        Property = "Address county",
        SingularErrorPattern = "[1] address county is the wrong length",
        PluralErrorPattern = "[COUNT] address counties are the wrong length",
        Hint = "Enter up to 35 characters"
    };
    
    internal static ValidationInfo InvalidAddressPostcodeLength(string category) => new()
    {
        Key = nameof(InvalidAddressPostcodeLength),
        Category = category,
        Property = "Address postcode",
        SingularErrorPattern = "[1] address postcode is the wrong length",
        PluralErrorPattern = "[COUNT] address postcodes are the wrong length",
        Hint = "Enter up to 10 characters"
    };
    
    internal static ValidationInfo InvalidAddressCountryLength(string category) => new()
    {
        Key = nameof(InvalidAddressCountryLength),
        Category = category,
        Property = "Address country",
        SingularErrorPattern = "[1] address country is the wrong length",
        PluralErrorPattern = "[COUNT] address countries are the wrong length",
        Hint = "Enter up to 10 characters"
    };
}