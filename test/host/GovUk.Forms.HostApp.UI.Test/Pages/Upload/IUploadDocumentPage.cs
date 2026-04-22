namespace GovUk.Forms.HostApp.UI.Test.Pages.Upload;

public interface IUploadDocumentPage
{
    Task WaitForPageToLoadAsync();
    Task ClickOnContinueButtonAsync();
    Task ClickOnBackButtonAsync();
    Task<IPage> ClickOnGiveFeedbackLinkAsync();
    Task ExpandCommonIssuesWhenUploadingRP14AFormsAsync();
    Task UploadFileAsync(string filePath);
}
