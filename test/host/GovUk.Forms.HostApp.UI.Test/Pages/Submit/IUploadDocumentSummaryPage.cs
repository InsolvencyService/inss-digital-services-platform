namespace GovUk.Forms.HostApp.UI.Test.Pages.Submit;

public interface IUploadDocumentSummaryPage
{
    Task WaitForPageToLoadAsync();
    Task VerifyUploadedDocumentAsync(string expectedFileName);
    Task VerifyCaseReferenceNumberAsync(string expectedValue);
    Task VerifyEmployerNameAsync(string expectedValue);
    Task VerifyIsCorrectEmployerNameAsync(string expectedValue);
    Task ClickChangeAsync();
    Task ClickChangeCaseReferenceAsync();
    Task ClickSubmitAsync();
}
