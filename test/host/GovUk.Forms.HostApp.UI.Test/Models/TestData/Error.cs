namespace GovUk.Forms.HostApp.UI.Test.Models.TestData;

public sealed record Error(
    string Message,
    string Hint = "",
    string Type = "",
    string Category = "");
