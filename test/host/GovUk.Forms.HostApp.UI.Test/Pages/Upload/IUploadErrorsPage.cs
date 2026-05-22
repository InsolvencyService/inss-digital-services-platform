using GovUk.Forms.HostApp.UI.Test.Models;

namespace GovUk.Forms.HostApp.UI.Test.Pages.Upload;

public interface IUploadErrorsPage
{
    Task VerifyThatCaseReferenceErrorsAreDisplayedAsync(string errorMessage);
    Task ClickContinueAsync();
    Task WaitForPageToLoadAsync();
    Task VerifyErrorSummaryAsync(UploadErrorSummary expected);
    Task ClickOnViewDetailsAsync(UploadErrorSummary expected);
    Task VerifyUploadedFileNameAsync(string expectedFileName);
    Task<int> GetErrorCountAsync(string errorKey);
    Task VerifyErrorMessageAsync(string expectedError);
    Task VerifyHintAsync(string expectedHint);
    Task VerifyValidationCategoryIsDisplayedAsync(string category);
}
