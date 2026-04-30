namespace GovUk.Forms.HostApp.UI.Test.Pages.Submit;

public interface IUploadDocumentSummaryPage
{
    Task VerifyPageIsDisplayedAsync();
    Task VerifyUploadedDocumentAsync(string expectedFileName);
    Task ClickChangeAsync();
    Task ClickSubmitAsync();
}
