namespace GovUk.Forms.HostApp.UI.Test.Models;

public sealed record UploadErrorSummary(
    string Category,
    string ErrorType,
    string ErrorMessage,
    string? HintText = null,
    string ActionText = "View details");
