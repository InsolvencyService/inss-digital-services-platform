namespace GovUk.Forms.HostApp.UI.Test.Helpers;

public interface IAllureReportingHelper
{
    Task StepAsync(string stepName, Func<Task> action, string caller = "");
    Task<T> StepAsync<T>(string stepName, Func<Task<T>> action);
    Task AttachScreenshotAsync(IPage page, string name = "Screenshot", bool fullPage = true);
    void AttachText(string name, string content);
    void AttachFile(string filePath, string? name = null, string mimeType = "text/plain");
    void AddParameter(string name, object? value);
}
