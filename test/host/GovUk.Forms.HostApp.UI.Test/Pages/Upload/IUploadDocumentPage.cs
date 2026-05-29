

using GovUk.Forms.HostApp.UI.Test.Models.TestData;

namespace GovUk.Forms.HostApp.UI.Test.Pages.Upload;

public interface IUploadDocumentPage
{
    Task WaitForPageToLoadAsync();
    Task ClickOnContinueButtonAsync();
    Task ClickOnBackButtonAsync();
    Task<IPage> ClickOnGiveFeedbackLinkAsync();
    Task ExpandCommonIssuesWhenUploadingRP14AFormsAsync();
    Task UploadFileAsync(string filePath);
    Task<string> GetUploadedFileNameAsync();
    Task<IReadOnlyList<string>> GetUploadedFileNamesAsync();
    Task VerifyUploadFileErrorAsync(UploadFileError expected);
    Task ClickErrorSummaryLinkAsync();
    Task VerifyUploadContentAriaSnapshotAsync();
    Task VerifyCommonIssuesWhenUploadingAriaSnapshotAsync();
}
