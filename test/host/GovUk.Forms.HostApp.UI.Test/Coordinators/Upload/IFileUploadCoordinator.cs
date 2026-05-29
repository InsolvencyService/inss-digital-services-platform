namespace GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;

public interface IFileUploadCoordinator
{
    Task UploadFileAsync(string filePath);
    Task VerifyUploadDocumentContentSnapShotAsync();
    Task VerifyUploadCommonIssuesContentVisualSnapShotAsync();
}
