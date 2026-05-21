using Bogus;
using GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Steps.Base;
using GovUk.Forms.HostApp.UI.Test.Support;
using static GovUk.Forms.HostApp.UI.Test.Models.TestData;

namespace GovUk.Forms.HostApp.UI.Test.Steps.Validation;

[Scope(Feature = "Employee Validation")]
[Binding]
public sealed class EmployeeValidationSteps : ValidationStepsBase
{
    private const string InvalidArrearsCountKey = "InvalidArrearsOfPayOwedCount";
    private readonly Faker _faker = new();

    public EmployeeValidationSteps(
        UploadDocumentCoordinator uploadDocumentCoordinator,
        UploadErrorDetailsCoordinator uploadErrorDetailsCoordinator,
        ScenarioContext scenarioContext)
        : base(uploadDocumentCoordinator, uploadErrorDetailsCoordinator, scenarioContext)
    {
    }

    [Given("the RP14A contains an employee with no surname")]
    public async Task GivenTheRp14aContainsAnEmployeeWithNoSurname()
    {
        AffectedEmployee employee = CreateAffectedEmployee(
            surname: string.Empty,
            forename: _faker.Name.FirstName(),
            cellValue: string.Empty);

        await UploadDocumentCoordinator.UploadRp14aWithEmployeeNameAsync(
            employee.Surname,
            employee.Forename);

        ScenarioContext.Set(employee);
    }

    [Given("the RP14A contains an employee surname longer than {int} characters")]
    public async Task GivenTheRp14aContainsAnEmployeeSurnameLongerThanCharacters(int length)
    {
        string surname = LengthHelper.OverMax(length);

        AffectedEmployee employee = CreateAffectedEmployee(
            surname: surname,
            forename: _faker.Name.FirstName(),
            cellValue: surname);

        await UploadDocumentCoordinator.UploadRp14aWithEmployeeNameAsync(
            employee.Surname,
            employee.Forename);

        ScenarioContext.Set(employee);
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

    [Given("the RP14A contains an employee with no national insurance number")]
    public async Task GivenTheRp14aContainsAnEmployeeWithNoNationalInsuranceNumber()
    {
        AffectedEmployee employee = CreateAffectedEmployee(
            niNumber: string.Empty,
            cellValue: string.Empty);

        await UploadDocumentCoordinator.UploadRp14aWithNationalInsuranceNumberAsync(
            string.Empty, occurrenceIndex: 0);

        ScenarioContext.Set(employee);
    }

    [Given("the RP14A contains employee national insurance number {string}")]
    public async Task GivenTheRp14aContainsEmployeeNationalInsuranceNumber(
        string nationalInsuranceNumber)
    {
        AffectedEmployee employee = CreateAffectedEmployee(
            niNumber: nationalInsuranceNumber,
            cellValue: nationalInsuranceNumber);

        await UploadDocumentCoordinator.UploadRp14aWithNationalInsuranceNumberAsync(
            nationalInsuranceNumber, occurrenceIndex: 0);

        ScenarioContext.Set(employee);
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

    [Given("the RP14A contains employment start date {string} with end date {string}")]
    public async Task GivenTheRp14aContainsEmploymentStartDateWithEndDate(
    string startDate,
    string endDate)
    {
        DateOnly? parsedStartDate = ParseDateOrNull(startDate);
        DateOnly? parsedEndDate = ParseDateOrNull(endDate);

        AffectedEmployee employee = CreateAffectedEmployee(
            cellValue: FormatDateRange(startDate, endDate));

        await UploadDocumentCoordinator.UploadRp14aWithEmploymentDatesAsync(
            parsedStartDate,
            parsedEndDate);

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

    [Given("the RP14A contains {int} employees with no surname")]
    public async Task GivenTheRp14aContainsEmployeesWithNoSurname(int employeeCount)
    {
        await UploadDocumentCoordinator
            .UploadRp14aWithMissingEmployeeSurnamesAsync(employeeCount);
    }


    [When("I proceed to the check answers page")]
    public async Task WhenIProceedToTheCheckAnswersPage()
    {
        await UploadErrorDetailsCoordinator.NavigateToCheckAnswersPageAsync();
    }

    [Then("I should see the validation error {string}")]
    public async Task ThenIShouldSeeTheValidationError(string errorMessage)
    {
        UploadErrorSummary expectedError = CreateEmployeeErrorSummary(
            "Employee surname",
            errorMessage);

        await UploadErrorDetailsCoordinator.VerifyErrorSummaryIsDisplayedAsync(
            expectedError);

        ScenarioContext.Set(expectedError);
    }

    [Then("I should see the national insurance number validation error {string}")]
    public async Task ThenIShouldSeeTheNationalInsuranceNumberValidationError(
        string errorMessage)
    {
        string resolvedErrorMessage =
            await UploadErrorDetailsCoordinator.ResolveValidationErrorMessageAsync(
                errorMessage);

        UploadErrorSummary expectedError = CreateEmployeeErrorSummary(
            "Employee national insurance number",
            resolvedErrorMessage);

        await UploadErrorDetailsCoordinator.VerifyValidationErrorAsync(
            errorMessage);

        ScenarioContext.Set(resolvedErrorMessage, ScenarioConstant.ErrorMessage);
        ScenarioContext.Set(expectedError);
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

    [Then("I should be able to view employee error details")]
    public async Task ThenIShouldBeAbleToViewEmployeeErrorDetails()
    {
        await VerifySingleEmployeeErrorDetailsAsync(
            ErrorDetailsHeaderType.EmployeeSurname);
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

    [Then("I should be able to view national insurance number error details")]
    public async Task ThenIShouldBeAbleToViewNationalInsuranceNumberErrorDetails()
    {
        await VerifySingleEmployeeErrorDetailsAsync(
            ErrorDetailsHeaderType.NationalInsuranceNumber);
    }

    [Then("I should be able to view money owed to employer error details")]
    public async Task ThenIShouldBeAbleToViewMoneyOwedToEmployerErrorDetails()
    {
        await VerifySingleEmployeeErrorDetailsAsync(
            ErrorDetailsHeaderType.MoneyOwedToEmployer);
    }

    [Then("I should be able to view the employee employment dates error details")]
    public async Task ThenIShouldBeAbleToViewTheEmployeeEmploymentDatesErrorDetails()
    {
        await VerifySingleEmployeeErrorDetailsAsync(
            ErrorDetailsHeaderType.EmploymentDates);
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

    [Then("I should be able to go to the previous page from the error details page")]
    public async Task ThenIShouldBeAbleToGoToThePreviousPageFromTheErrorDetailsPage()
    {
        await UploadErrorDetailsCoordinator.ClickBackAndVerifyUploadErrorPageIsDisplayedAsync();
    }

    [Then("I should be returned to the upload page")]
    public async Task ThenIShouldBeReturnedToTheUploadPage()
    {
        await UploadDocumentCoordinator.VerifyUploadDocumentPageIsDisplayedAsync();

        string screenshotPath =
            await UploadDocumentCoordinator.CaptureUploadDocumentPageVisualAsync();

        await VerifyFile(screenshotPath)
            .UseDirectory(ScenarioConstant.SnapShots)
            .UseFileName(ScenarioConstant.UploadPageWithWarning);
    }

    [Then("I should see the following basic pay per week validation errors")]
    public async Task ThenIShouldSeeTheFollowingBasicPayPerWeekValidationErrors(DataTable dataTable)
    {
        Errors error = dataTable.CreateInstance<Errors>();
        UploadErrorSummary expectedError = CreateEmployeeErrorSummary(error.Type, error.Message, error.Hint);
        await UploadErrorDetailsCoordinator.VerifyErrorSummaryIsDisplayedAsync(expectedError);
        ScenarioContext.Set(expectedError);
    }

    [Then("I should be able to view the validation error details for employees where the surname is missing")]
    public async Task ThenIShouldBeAbleToViewTheValidationErrorDetailsForEmployeesWhereTheSurnameIsMissing()
    {
        UploadErrorSummary expectedError =
            ScenarioContext.Get<UploadErrorSummary>();

        List<AffectedEmployee> affectedEmployees =
            ScenarioContext.Get<List<AffectedEmployee>>(AffectedEmployeesKey);

        await UploadErrorDetailsCoordinator.VerifyErrorDetailsAsync(
            expectedError,
            affectedEmployees,
            ErrorDetailsHeaderType.EmployeeSurname);
    }
}
