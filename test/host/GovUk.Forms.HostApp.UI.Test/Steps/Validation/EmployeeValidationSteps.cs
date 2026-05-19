using Bogus;
using GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Support;
using System.Globalization;
using static GovUk.Forms.HostApp.UI.Test.Models.TestData;

namespace GovUk.Forms.HostApp.UI.Test.Steps.Validation;

[Scope(Feature = "Employee Validation")]
[Binding]
public sealed class EmployeeValidationSteps
{
    private const string AffectedEmployeesKey = "AffectedEmployees";
    private const string InvalidArrearsCountKey = "InvalidArrearsOfPayOwedCount";

    private readonly UploadDocumentCoordinator _uploadDocumentCoordinator;
    private readonly UploadErrorDetailsCoordinator _uploadErrorDetailsCoordinator;
    private readonly ScenarioContext _scenarioContext;
    private readonly Faker _faker = new();

    public EmployeeValidationSteps(
        UploadDocumentCoordinator uploadDocumentCoordinator,
        UploadErrorDetailsCoordinator uploadErrorDetailsCoordinator,
        ScenarioContext scenarioContext)
    {
        _uploadDocumentCoordinator = uploadDocumentCoordinator;
        _uploadErrorDetailsCoordinator = uploadErrorDetailsCoordinator;
        _scenarioContext = scenarioContext;
    }

    [Given("the RP14A contains an employee with no surname")]
    public async Task GivenTheRp14aContainsAnEmployeeWithNoSurname()
    {
        AffectedEmployee employee = CreateAffectedEmployee(
            surname: string.Empty,
            forename: _faker.Name.FirstName(),
            cellValue: string.Empty);

        await _uploadDocumentCoordinator.UploadRp14aWithEmployeeNameAsync(
            employee.Surname,
            employee.Forename);

        _scenarioContext.Set(employee);
    }

    [Given("the RP14A contains an employee surname longer than {int} characters")]
    public async Task GivenTheRp14aContainsAnEmployeeSurnameLongerThanCharacters(int length)
    {
        string surname = LengthHelper.OverMax(length);

        AffectedEmployee employee = CreateAffectedEmployee(
            surname: surname,
            forename: _faker.Name.FirstName(),
            cellValue: surname);

        await _uploadDocumentCoordinator.UploadRp14aWithEmployeeNameAsync(
            employee.Surname,
            employee.Forename);

        _scenarioContext.Set(employee);
    }

    [Given(@"the RP14A contains employee arrears of pay owed ""(.*)""")]
    public async Task GivenTheRp14aContainsEmployeeArrearsOfPayOwed(string arrearsOfPay)
    {
        AffectedEmployee employee = CreateAffectedEmployee(
            cellValue: arrearsOfPay);

        await _uploadDocumentCoordinator.UploadRp14aWithArrearsOfPayOwedAsync(
            arrearsOfPay);

        _scenarioContext.Set(employee);
    }

    [Given(@"the RP14A contains (.*) invalid arrears of pay owed")]
    public async Task GivenTheRp14aContainsInvalidArrearsOfPayOwed(int count)
    {
        await _uploadDocumentCoordinator.UploadRp14aWithInvalidArrearsOfPayOwedAsync(
            count);

        _scenarioContext.Set(count, InvalidArrearsCountKey);
    }

    [Given("the RP14A contains an employee with no national insurance number")]
    public async Task GivenTheRp14aContainsAnEmployeeWithNoNationalInsuranceNumber()
    {
        AffectedEmployee employee = CreateAffectedEmployee(
            niNumber: string.Empty,
            cellValue: string.Empty);

        await _uploadDocumentCoordinator.UploadRp14aWithNationalInsuranceNumberAsync(
            string.Empty, occurrenceIndex: 0);

        _scenarioContext.Set(employee);
    }

    [Given("the RP14A contains employee national insurance number {string}")]
    public async Task GivenTheRp14aContainsEmployeeNationalInsuranceNumber(
        string nationalInsuranceNumber)
    {
        AffectedEmployee employee = CreateAffectedEmployee(
            niNumber: nationalInsuranceNumber,
            cellValue: nationalInsuranceNumber);

        await _uploadDocumentCoordinator.UploadRp14aWithNationalInsuranceNumberAsync(
            nationalInsuranceNumber, occurrenceIndex: 0);

        _scenarioContext.Set(employee);
    }

    [Given("the RP14A contains money owed to employer {string}")]
    public async Task GivenTheRp14aContainsMoneyOwedToEmployer(string moneyOwed)
    {
        AffectedEmployee employee = CreateAffectedEmployee(
            cellValue: moneyOwed);

        await _uploadDocumentCoordinator.UploadRp14aWithMoneyOwedToEmployerAsync(
            moneyOwed);

        _scenarioContext.Set(employee);
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

        await _uploadDocumentCoordinator.UploadRp14aWithEmploymentDatesAsync(
            parsedStartDate,
            parsedEndDate);

        _scenarioContext.Set(employee);
    }


    [Given("the RP14A contains arrears of pay start date {string} and end date {string}")]
    public async Task GivenTheRp14aContainsArrearsOfPayStartDateAndEndDate(
        string startDate,
        string endDate)
    {
        AffectedEmployee employee = CreateAffectedEmployee(
            cellValue: FormatDateRange(startDate, endDate));

        await _uploadDocumentCoordinator.UploadRp14aWithArrearsDatesAsync(
            startDate,
            endDate);

        _scenarioContext.Set(employee);
    }

    [When("I attempt to submit the RP14A")]
    public async Task WhenIAttemptToSubmitTheRp14a()
    {
        await _uploadDocumentCoordinator.NavigateToSubmitPageAsync();
    }

    [When("I proceed to the check answers page")]
    public async Task WhenIProceedToTheCheckAnswersPage()
    {
        await _uploadErrorDetailsCoordinator.NavigateToCheckAnswersPageAsync();
    }

    [Then("I should see the validation error {string}")]
    public async Task ThenIShouldSeeTheValidationError(string errorMessage)
    {
        UploadErrorSummary expectedError = CreateEmployeeErrorSummary(
            "Employee surname",
            errorMessage);

        await _uploadErrorDetailsCoordinator.VerifyErrorSummaryIsDisplayedAsync(
            expectedError);

        _scenarioContext.Set(expectedError);
    }

    [Then("I should see the national insurance number validation error {string}")]
    public async Task ThenIShouldSeeTheNationalInsuranceNumberValidationError(
        string errorMessage)
    {
        string resolvedErrorMessage =
            await _uploadErrorDetailsCoordinator.ResolveValidationErrorMessageAsync(
                errorMessage);

        UploadErrorSummary expectedError = CreateEmployeeErrorSummary(
            "Employee national insurance number",
            resolvedErrorMessage);

        await _uploadErrorDetailsCoordinator.VerifyValidationErrorAsync(
            errorMessage);

        _scenarioContext.Set(resolvedErrorMessage, ScenarioConstant.ErrorMessage);
        _scenarioContext.Set(expectedError);
    }

    [Then("I should see the following validation errors")]
    public async Task ThenIShouldSeeTheFollowingValidationErrors(DataTable dataTable)
    {
        Errors error = dataTable.CreateInstance<Errors>();

        UploadErrorSummary expectedError =
            error.Type.Equals("Money owed to employer", StringComparison.OrdinalIgnoreCase)
                ? CreateErrorSummary(error.Type, error.Message, error.Hint)
                : CreateEmployeeErrorSummary(error.Type, error.Message, error.Hint);

        await _uploadErrorDetailsCoordinator.VerifyErrorSummaryIsDisplayedAsync(
            expectedError);

        _scenarioContext.Set(expectedError);
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
        UploadErrorSummary expectedError = _scenarioContext.Get<UploadErrorSummary>();

        List<AffectedEmployee> affectedEmployees =
            _scenarioContext.Get<List<AffectedEmployee>>(AffectedEmployeesKey);

        await _uploadErrorDetailsCoordinator.VerifyErrorDetailsAsync(
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

    [Then("I should be able to go to the previous page from the error details page")]
    public async Task ThenIShouldBeAbleToGoToThePreviousPageFromTheErrorDetailsPage()
    {
        await _uploadErrorDetailsCoordinator.ClickBackAndVerifyUploadErrorPageIsDisplayedAsync();
    }

    [Then("I should be returned to the upload page")]
    public async Task ThenIShouldBeReturnedToTheUploadPage()
    {
        await _uploadDocumentCoordinator.VerifyUploadDocumentPageIsDisplayedAsync();

        string screenshotPath =
            await _uploadDocumentCoordinator.CaptureUploadDocumentPageVisualAsync();

        await VerifyFile(screenshotPath)
            .UseDirectory(ScenarioConstant.SnapShots)
            .UseFileName(ScenarioConstant.UploadPageWithWarning);
    }

    private async Task VerifySingleEmployeeErrorDetailsAsync(
        ErrorDetailsHeaderType headerType)
    {
        UploadErrorSummary expectedError =
            _scenarioContext.Get<UploadErrorSummary>();

        AffectedEmployee affectedEmployee =
            _scenarioContext.Get<AffectedEmployee>();

        await _uploadErrorDetailsCoordinator.VerifyErrorDetailsAsync(
            expectedError,
            affectedEmployee,
            headerType);
    }

    private static DateOnly? ParseDateOrNull(string value)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            value.Equals("<empty>", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return DateOnly.ParseExact(
            value,
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture);
    }
}
