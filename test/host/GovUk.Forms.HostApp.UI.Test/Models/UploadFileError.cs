namespace GovUk.Forms.HostApp.UI.Test.Models;

public partial class TestData
{
    public sealed record UploadFileError(
    string SummaryTitle,
    string ErrorMessage);

}
