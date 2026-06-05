namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

public static class DirectorValidationInfo
{
    public static ValidationInfo InvalidDirectorNinoFormat() => new()
    {
        Key = nameof(InvalidDirectorNinoFormat),
        Category = "Directors",
        Property = "Director national insurance number",
        SingularErrorPattern = "1 National Insurance number is in the wrong format",
        PluralErrorPattern = "[COUNT] National Insurance numbers are in the wrong format",
        Hint = "Enter a National Insurance number like QQ 12 34 56 C"
    };
    
    public static ValidationInfo InvalidDirectorInitialsLength() => new()
    {
        Key = nameof(InvalidDirectorInitialsLength),
        Category = "Directors",
        Property = "Director initials",
        SingularErrorPattern = "1 director initials are the wrong length",
        PluralErrorPattern = "[COUNT] director initials are the wrong length",
        Hint = "Enter up to 100 characters"
    };
    
    public static ValidationInfo InvalidDirectorSurnameLength() => new()
    {
        Key = nameof(InvalidDirectorSurnameLength),
        Category = "Directors",
        Property = "Director surname",
        SingularErrorPattern = "1 director surname is the wrong length",
        PluralErrorPattern = "[COUNT] director surname are the wrong length",
        Hint = "Enter up to 100 characters"
    };
}