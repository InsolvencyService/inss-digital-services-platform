namespace GovUk.Forms.HostApp.UI.Test.Pages.Submit;

public interface ISubmissionConfirmationPage
{
    Task WaitForPageToLoadAsync();
    Task ClickUploadAnotherFormButtonAsync();
    Task VerifyWhatHappensNextContentAsync();
    Task<string> GetReferenceNumberAsync();
}
