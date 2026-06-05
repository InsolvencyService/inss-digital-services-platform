namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

public static class ShareholderValidationInfo
{
    public static ValidationInfo InvalidShareholderPercentage() => new()
    {
        Key = nameof(InvalidShareholderPercentage),
        Category = "Shareholders",
        Property = "Shareholder percentage",
        SingularErrorPattern = "[1] shareholder percentage is incorrect",
        PluralErrorPattern = "[COUNT] shareholder percentages are incorrect",
        Hint = "Enter a number like 50.50 or 100"
    };
    
    public static ValidationInfo InvalidShareholderNameLength() => new()
    {
        Key = nameof(InvalidShareholderNameLength),
        Category = "Shareholders",
        Property = "Shareholder name",
        SingularErrorPattern = "[1] shareholder name is the wrong length",
        PluralErrorPattern = "[COUNT] shareholder names are the wrong length",
        Hint = "Enter up to 100 characters"
    };
}