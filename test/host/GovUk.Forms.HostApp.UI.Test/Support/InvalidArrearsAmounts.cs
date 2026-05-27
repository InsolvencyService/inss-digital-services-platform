namespace GovUk.Forms.HostApp.UI.Test.Support;

public static class InvalidArrearsAmounts
{
    public const string OneDecimalPlace = "15.3";
    public const string TooManyDecimalPlaces = "12.345";
    public const string NegativeAmount = "-100";

    public static readonly IReadOnlyList<string> All =
    [
        OneDecimalPlace,
        TooManyDecimalPlaces,
        NegativeAmount
    ];
}
