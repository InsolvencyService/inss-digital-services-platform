using GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Steps.Base;
using static GovUk.Forms.HostApp.UI.Test.Models.TestData;

namespace GovUk.Forms.HostApp.UI.Test.Steps.Validation;

[Scope(Feature = "Employees Payment Validation")]
[Binding]
public sealed class EmployeesPaymentValidationSteps : ValidationStepsBase
{
    public EmployeesPaymentValidationSteps(
        UploadDocumentCoordinator uploadDocumentCoordinator,
        UploadErrorDetailsCoordinator uploadErrorDetailsCoordinator,
        ScenarioContext scenarioContext)
        : base(uploadDocumentCoordinator, uploadErrorDetailsCoordinator, scenarioContext)
    {
    }

    [Given(@"the RP14A contains employee arrears of pay owed ""(.*)""")]
    public async Task GivenTheRp14aContainsEmployeeArrearsOfPayOwed(string arrearsOfPay)
    {
        AffectedEmployee employee = CreateAffectedEmployee(
            cellValue: arrearsOfPay);

        await UploadDocumentCoordinator.UploadRp14aWithArrearsOfPayOwedAsync(
            arrearsOfPay);

        ScenarioContext.Set(employee);
    }

    [Then("I should see the following validation errors")]
    public async Task ThenIShouldSeeTheFollowingValidationErrors(DataTable dataTable)
    {
        Errors error = dataTable.CreateInstance<Errors>();

        UploadErrorSummary expectedError =
            error.Type.Equals("Money owed to employer", StringComparison.OrdinalIgnoreCase)
                ? CreateErrorSummary(error.Type, error.Message, error.Hint)
                : CreateEmployeeErrorSummary(error.Type, error.Message, error.Hint);

        await UploadErrorDetailsCoordinator.VerifyErrorSummaryIsDisplayedAsync(
            expectedError);

        ScenarioContext.Set(expectedError);
    }


    [Then("I should be able to view employee arrears of pay owed error details")]
    public async Task ThenIShouldBeAbleToViewEmployeeArrearsOfPayOwedErrorDetails()
    {
        await VerifySingleEmployeeErrorDetailsAsync(
            ErrorDetailsHeaderType.ArrearsOfPayOwed);
    }
}
