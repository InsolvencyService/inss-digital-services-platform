using GovUk.Forms.HostApp.UI.Test.Models;

namespace GovUk.Forms.HostApp.UI.Test.Pages.Upload;

public interface IUploadErrorDetailsPage
{
    Task VerifyAffectedEmployeeAsync(AffectedEmployee employee);
    Task VerifyErrorMessageAsync(string expectedMessage);
    Task VerifyAffectedEmployeeTableHeadersAsync();
    Task WaitForPageToLoadAsync();
    Task VerifyErrorDetailsDoesNotContainAsync(string text);
    Task VerifyEmployerErrorSummaryIsDisplayedAsync();
    Task VerifyEmployeeErrorSummaryIsDisplayedAsync();
    Task<int> GetColumnIndexAsync(string columnName);
    Task VerifyEmployeeArrearsOfPaymentOwedErrorSummaryIsDisplayedAsync();
    Task VerifyEmployeeNationalInsuranceNumberHeaderIsDisplayedAsync();
    Task VerifyMoneyOwedToEmployerHeaderIsDisplayedAsync();
    Task VerifyEmploymentDatesHeaderIsDisplayedAsync();
    Task ClickBackButtonAsync();
    Task VerifyArrearsOfPayDatesHeaderIsDisplayedAsync();
    Task VerifyCaseReferenceHeaderIsDisplayedAsync();
}
