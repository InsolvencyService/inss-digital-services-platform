namespace GovUk.Forms.HostApp.UI.Test.Support;

public static class InvalidCaseReferences
{
    public static readonly List<string> All =
    [
        "XX12345678", // wrong prefix
        "CNABCDEFGH", // letters instead of digits
        "1234567890", // missing CN
        "AB12345678"  // invalid prefix
    ];
}
