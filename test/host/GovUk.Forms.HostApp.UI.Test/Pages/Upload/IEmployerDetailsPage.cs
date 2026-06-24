namespace GovUk.Forms.HostApp.UI.Test.Pages.Upload;

public interface IEmployerDetailsPage
{
    Task WaitForPageToLoadAsync();
    Task<string> GetCaseReferenceNumberAsync();
    Task<string> GetEmployerNameAsync();
    Task SelectYesAsync();
    Task SelectNoAsync();
    Task ClickContinueAsync();
    Task ClickBackAsync();
    Task VerifyAriaSnapshotAsync();
}
