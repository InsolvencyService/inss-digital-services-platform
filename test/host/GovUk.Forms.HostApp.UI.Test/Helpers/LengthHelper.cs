namespace GovUk.Forms.HostApp.UI.Test.Helpers;

public class LengthHelper
{
    public static string AtMax(int max) => new('A', max);

    public static string OverMax(int max) => new('A', max + 1);

    public static string UnderMax(int max) => new('A', max - 1);
}
