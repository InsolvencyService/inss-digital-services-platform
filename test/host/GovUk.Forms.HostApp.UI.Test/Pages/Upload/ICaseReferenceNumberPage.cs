namespace GovUk.Forms.HostApp.UI.Test.Pages.Upload;

public interface ICaseReferenceNumberPage
{
    Task WaitForPageToLoadAsync();
    Task EnterCaseReferenceNumberAsync(string caseReference);
    Task ClickContinueAsync();
    Task ClickBackAsync();
    Task VerifyAriaSnapshotAsync();
    Task VerifyErrorMessageAsync(string errorMessage);
}
