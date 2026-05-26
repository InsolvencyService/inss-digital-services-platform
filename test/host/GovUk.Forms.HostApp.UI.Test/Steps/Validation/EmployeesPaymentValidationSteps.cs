using GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Models.TestData;
using GovUk.Forms.HostApp.UI.Test.Steps.Base;
using GovUk.Forms.HostApp.UI.Test.Tags;
using static GovUk.Forms.HostApp.UI.Test.Models.TestData.TestFactory;

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

    [Given(@"the RP14A contains (.*) invalid arrears of pay owed")]
    public async Task GivenTheRp14aContainsInvalidArrearsOfPayOwed(int count)
    {
        await UploadDocumentCoordinator.UploadRp14aWithInvalidArrearsOfPayOwedAsync(
            count);

        ScenarioContext.Set(count, InvalidArrearsCountKey);
    }

    [Given("the RP14A contains money owed to employer {string}")]
    public async Task GivenTheRp14aContainsMoneyOwedToEmployer(string moneyOwed)
    {
        AffectedEmployee employee = CreateAffectedEmployee(
            cellValue: moneyOwed);

        await UploadDocumentCoordinator.UploadRp14aWithMoneyOwedToEmployerAsync(
            moneyOwed);

        ScenarioContext.Set(employee);
    }

    [Given("the RP14A contains arrears of pay start date {string} and end date {string}")]
    public async Task GivenTheRp14aContainsArrearsOfPayStartDateAndEndDate(string startDate, string endDate)
    {
        AffectedEmployee employee = CreateAffectedEmployee(
            cellValue: FormatDateRange(startDate, endDate));

        DateOnly? parsedStartDate = ParseDateOrNull(startDate);
        DateOnly? parsedEndDate = ParseDateOrNull(endDate);

        await UploadDocumentCoordinator.UploadRp14aWithArrearsDatesAsync(
            parsedStartDate,
            parsedEndDate);

        ScenarioContext.Set(employee);
    }

    [Given("the RP14A contains employee basic pay per week {string}")]
    public async Task GivenTheRPA14ContainsEmployeeBasicPayPerWeek(string basicPayPerWeek)
    {
        AffectedEmployee employee = CreateAffectedEmployee(
             cellValue: basicPayPerWeek);

        await UploadDocumentCoordinator.UploadRp14aWithEmployeeBasicPayPerWeekAsync(
            basicPayPerWeek);

        ScenarioContext.Set(employee);
    }

    [Then("I should see the following basic pay per week validation errors")]
    public async Task ThenIShouldSeeTheFollowingBasicPayPerWeekValidationErrors(DataTable dataTable)
    {
        Error error = dataTable.CreateInstance<Error>();
        UploadErrorSummary expectedError = CreateEmployeeErrorSummary(error.Type, error.Message, error.Hint);
        await UploadErrorDetailsCoordinator.VerifyErrorSummaryIsDisplayedAsync(expectedError);
        ScenarioContext.Set(expectedError);
    }

    [Then("I should see the following validation errors")]
    public async Task ThenIShouldSeeTheFollowingValidationErrors(DataTable dataTable)
    {
        Error error = dataTable.CreateInstance<Error>();

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

    [Then("I should be able to view employee arrears of pay owed error details for multiple employees")]
    public async Task ThenIShouldBeAbleToViewEmployeeArrearsOfPayOwedErrorDetailsForMultipleEmployees()
    {
        UploadErrorSummary expectedError = ScenarioContext.Get<UploadErrorSummary>();

        List<AffectedEmployee> affectedEmployees =
            ScenarioContext.Get<List<AffectedEmployee>>(AffectedEmployeesKey);

        await UploadErrorDetailsCoordinator.VerifyErrorDetailsAsync(
            expectedError,
            affectedEmployees,
            ErrorDetailsHeaderType.ArrearsOfPayOwed);
    }

    [Then("I should be able to view money owed to employer error details")]
    public async Task ThenIShouldBeAbleToViewMoneyOwedToEmployerErrorDetails()
    {
        await VerifySingleEmployeeErrorDetailsAsync(
            ErrorDetailsHeaderType.MoneyOwedToEmployer);
    }

    [Then("I should be able to view error details")]
    public async Task ThenIShouldBeAbleToViewErrorDetails()
    {
        await VerifySingleEmployeeErrorDetailsAsync(
            ErrorDetailsHeaderType.ArrearsOfPayDates);
    }

    [Then("I should be able to view basic pay per week validation error details")]
    public async Task ThenIShouldBeAbleToViewBasicPayPerWeekValidationErrorDetails()
    {
        await VerifySingleEmployeeErrorDetailsAsync(ErrorDetailsHeaderType.BasicPayPerWeek);
    }
}
