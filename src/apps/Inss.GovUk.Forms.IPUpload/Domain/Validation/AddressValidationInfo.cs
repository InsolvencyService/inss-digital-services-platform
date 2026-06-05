namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

public static class AddressValidationInfo
{
    public static ValidationInfo InvalidAddressLinesLength(string category) => new()
    {
        Key = nameof(InvalidAddressLinesLength),
        Category = category,
        Property = "Address lines",
        SingularErrorPattern = "[1] address lines are the wrong length",
        PluralErrorPattern = "[COUNT] address lines are the wrong length",
        Hint = "Enter up to 4 address lines"
    };
    
    public static ValidationInfo MissingAddressLine1(string category) => new()
    {
        Key = nameof(InvalidAddressLineLength),
        Category = category,
        Property = "Address lines",
        SingularErrorPattern = "[1] first address line is missing",
        PluralErrorPattern = "[COUNT] first address lines are missing",
        Hint = "Enter up to 35 characters"
    };
    
    public static ValidationInfo InvalidAddressLineLength(string category) => new()
    {
        Key = nameof(InvalidAddressLineLength),
        Category = category,
        Property = "Address lines",
        SingularErrorPattern = "[1] address line is the wrong length",
        PluralErrorPattern = "[COUNT] address lines are the wrong length",
        Hint = "Enter up to 35 characters"
    };
    
    public static ValidationInfo InvalidAddressTownLength(string category) => new()
    {
        Key = nameof(InvalidAddressTownLength),
        Category = category,
        Property = "Address town",
        SingularErrorPattern = "[1] address town is the wrong length",
        PluralErrorPattern = "[COUNT] address towns are the wrong length",
        Hint = "Enter up to 35 characters"
    };
    
    public static ValidationInfo InvalidAddressCountyLength(string category) => new()
    {
        Key = nameof(InvalidAddressCountyLength),
        Category = category,
        Property = "Address county",
        SingularErrorPattern = "[1] address county is the wrong length",
        PluralErrorPattern = "[COUNT] address counties are the wrong length",
        Hint = "Enter up to 35 characters"
    };
    
    public static ValidationInfo InvalidAddressPostcodeLength(string category) => new()
    {
        Key = nameof(InvalidAddressPostcodeLength),
        Category = category,
        Property = "Address postcode",
        SingularErrorPattern = "[1] address postcode is the wrong length",
        PluralErrorPattern = "[COUNT] address postcodes are the wrong length",
        Hint = "Enter up to 10 characters"
    };
    
    public static ValidationInfo InvalidAddressCountryLength(string category) => new()
    {
        Key = nameof(InvalidAddressCountryLength),
        Category = category,
        Property = "Address country",
        SingularErrorPattern = "[1] address country is the wrong length",
        PluralErrorPattern = "[COUNT] address countries are the wrong length",
        Hint = "Enter up to 10 characters"
    };
}