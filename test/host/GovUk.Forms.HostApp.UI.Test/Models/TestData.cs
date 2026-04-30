namespace GovUk.Forms.HostApp.UI.Test.Models;

public class TestData
{
    public class Errors
    {
        public string Message { get; set; } = string.Empty;
    }

    public sealed record UploadFileError(
    string SummaryTitle,
    string ErrorMessage);
}
