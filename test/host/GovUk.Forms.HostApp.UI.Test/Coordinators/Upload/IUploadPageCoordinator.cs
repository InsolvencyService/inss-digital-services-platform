namespace GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;

public interface IUploadPageCoordinator
{
    Task VerifyUploadDocumentPageIsDisplayedAsync();
    Task ExpandCommonIssuesWhenUploadingRP14AFormsAsync();
}
