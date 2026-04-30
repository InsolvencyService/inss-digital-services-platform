using GovUk.Forms.HostApp.UI.Test.Models;

namespace GovUk.Forms.HostApp.UI.Test.Pages.Upload;

public interface IUploadErrorDetailsPage
{
    Task VerifyAffectedEmployeeAsync(AffectedEmployee employee);
    Task VerifyErrorMessageAsync(string expectedMessage);
    Task VerifyAffectedEmployeeTableHeadersAsync();
    Task WaitForPageToLoadAsync();
    Task VerifyErrorDetailsDoesNotContainAsync(string text);
    Task EmployerErrorSummaryIsDisplayedAsync();
}
