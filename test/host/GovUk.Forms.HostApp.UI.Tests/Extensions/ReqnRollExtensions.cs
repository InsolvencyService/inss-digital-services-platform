using Reqnroll;

namespace GovUk.Forms.HostApp.UI.Tests.Extensions;

public static class ReqnRollExtensions
{
    internal static void AddAttachmentAsLink(this IReqnrollOutputHelper outputHelper, string path)
    {
        outputHelper.WriteLine($"[Attachment: {path}]");
    }
}
