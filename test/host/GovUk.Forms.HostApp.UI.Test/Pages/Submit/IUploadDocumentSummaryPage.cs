namespace GovUk.Forms.HostApp.UI.Test.Pages.Submit;

public interface IUploadDocumentSummaryPage
{
    Task WaitForPageToLoadAsync();
    Task VerifyUploadedDocumentAsync(string expectedFileName);
    Task ClickChangeAsync();
    Task ClickSubmitAsync();
}
