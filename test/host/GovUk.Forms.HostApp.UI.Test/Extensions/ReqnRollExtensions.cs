namespace GovUk.Forms.HostApp.UI.Test.Extensions;

public static class ReqnRollExtensions
{
    internal static void AddAttachmentAsLink(this IReqnrollOutputHelper outputHelper, string path)
    {
        outputHelper.WriteLine($"[Attachment: {path}]");
    }
}
