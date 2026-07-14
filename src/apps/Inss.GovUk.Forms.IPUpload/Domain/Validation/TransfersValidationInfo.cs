namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

public static class TransfersValidationInfo
{
    public static ValidationInfo InvalidTransferToNameLength() => new()
    {
        Key = nameof(InvalidTransferToNameLength),
        Category = "Transfers",
        Property = "Transfer to name",
        SingularErrorPattern = "1 transfer to name is the wrong length",
        PluralErrorPattern = "[COUNT] transfer to names are the wrong length",
        Hint = "Enter up to 60 characters"
    };
}