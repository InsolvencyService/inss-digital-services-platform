using Bogus;
using GovUk.Forms.HostApp.UI.Test.Coordinators;
using GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Models.TestData;
using GovUk.Forms.HostApp.UI.Test.Steps.Base;
using GovUk.Forms.HostApp.UI.Test.Support;
using GovUk.Forms.HostApp.UI.Test.Tags;
using static GovUk.Forms.HostApp.UI.Test.Models.TestData.TestFactory;

namespace GovUk.Forms.HostApp.UI.Test.Steps.Validation;

[Scope(Feature = "Employees Validation")]
[Binding]
public sealed class EmployeeValidationSteps : ValidationStepsBase
{
    private readonly Faker _faker = new();
    private readonly CheckYourAnswersCoordinator _checkYourAnswersCoordinator;

    public EmployeeValidationSteps(
        UploadDocumentCoordinator uploadDocumentCoordinator,
        UploadErrorDetailsCoordinator uploadErrorDetailsCoordinator,
        ScenarioContext scenarioContext,
        CheckYourAnswersCoordinator checkYourAnswersCoordinator)
        : base(uploadDocumentCoordinator, uploadErrorDetailsCoordinator, scenarioContext)
    {
        _checkYourAnswersCoordinator = checkYourAnswersCoordinator;
    }

    [Given("the RP14A contains an employee with no surname")]
    public async Task GivenTheRp14aContainsAnEmployeeWithNoSurname()
    {
        AffectedEmployee employee = CreateAffectedEmployee(
            surname: string.Empty,
            forename: _faker.Name.FirstName(),
            cellValue: "Not entered");

        await UploadDocumentCoordinator.UploadRp14aWithEmployeeNameAsync(
            employee.Surname,
            employee.Forename);

        ScenarioContext.Set(employee);
    }

    [Given("the RP14A XML contains an employee surname of length {int}")]
    public async Task GivenTheRPAXMLContainsAnEmployeeSurnameOfLength(int length)
    {
        string surname = LengthHelper.AtMax(length);

        AffectedEmployee employee = CreateAffectedEmployee(
            surname: surname,
            forename: _faker.Name.FirstName(),
            cellValue: surname);

        await UploadDocumentCoordinator.UploadRp14aWithEmployeeNameAsync(employee.Surname, employee.Forename);

        ScenarioContext.Set(employee);
    }

    [Given("the RP14A contains an employee with no national insurance number")]
    public async Task GivenTheRp14aContainsAnEmployeeWithNoNationalInsuranceNumber()
    {
        AffectedEmployee employee = CreateAffectedEmployee(niNumber: string.Empty, cellValue: "Not entered");

        await UploadDocumentCoordinator.UploadRp14aWithNationalInsuranceNumberAsync(string.Empty, occurrenceIndex: 0);

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

    [Given("the RP14A contains employment start date {string} with end date {string}")]
    public async Task GivenTheRp14aContainsEmploymentStartDateWithEndDate(string startDate, string endDate)
    {
        DateOnly? parsedStartDate = ParseDateOrNull(startDate);
        DateOnly? parsedEndDate = ParseDateOrNull(endDate);

        AffectedEmployee employee = CreateAffectedEmployee(
            cellValue: FormatDateRange(startDate, endDate));

        await UploadDocumentCoordinator.UploadRp14aWithEmploymentDatesAsync(parsedStartDate, parsedEndDate);

        ScenarioContext.Set(employee);
    }

    [Given("the RP14A contains {int} employees with no surname")]
    public async Task GivenTheRp14aContainsEmployeesWithNoSurname(int employeeCount)
    {
        await UploadDocumentCoordinator
            .UploadRp14aWithMissingEmployeeSurnamesAsync(employeeCount);
    }

    [Given("the RP14A contains {int} employees with surname of length {int}")]
    public async Task GivenTheRp14aContainsEmployeesWithSurnameOfLength(int employeeCount, int length)
    {
        string surname = LengthHelper.AtMax(length);
        await UploadDocumentCoordinator.UploadRp14aWithSurnameForEmployeesAsync(employeeCount, surname);
    }

    [Given("the RP14A contains {int} employees with employment start date after end date")]
    public async Task GivenTheRp14aContainsEmployeesWithEmploymentStartDateAfterEndDate(int employeeCount)
    {
        DateOnly startDate = new(2026, 4, 30);
        DateOnly endDate = new(2026, 4, 1);

        await UploadDocumentCoordinator.UploadRp14aWithEmploymentDatesForEmployeesAsync(
            employeeCount, startDate, endDate);
    }

    [Given("the RP14A contains {int} employees with national insurance number {string}")]
    public async Task GivenTheRPAContainsEmployeesWithNationalInsuranceNumber(int employeeCount, string nationalInsuranceNumber)
    {
        await UploadDocumentCoordinator.UploadRp14aWithNationalInsuranceNumberForEmployeesAsync(employeeCount, nationalInsuranceNumber);
    }

    [Given("the RP14A contains {int} employees with no national insurance number")]
    public async Task GivenTheRPAContainsEmployeesWithNoNationalInsuranceNumber(int employeeCount)
    {
        await UploadDocumentCoordinator.UploadRp14aWithNationalInsuranceNumberForEmployeesAsync(employeeCount, string.Empty);
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
    public async Task ThenIShouldSeeTheNationalInsuranceNumberValidationError(string errorMessage)
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
        Error error = dataTable.CreateInstance<Error>();

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
        await VerifySingleEmployeeErrorDetailsAsync(ErrorDetailsHeaderType.EmployeeSurname);
    }


    [Then("I should be able to view national insurance number error details")]
    public async Task ThenIShouldBeAbleToViewNationalInsuranceNumberErrorDetails()
    {
        await VerifySingleEmployeeErrorDetailsAsync(ErrorDetailsHeaderType.NationalInsuranceNumber);
    }

    [Then("I should be able to view the employee employment dates error details")]
    public async Task ThenIShouldBeAbleToViewTheEmployeeEmploymentDatesErrorDetails()
    {
        await VerifySingleEmployeeErrorDetailsAsync(ErrorDetailsHeaderType.EmploymentDates);
    }

    [Then("I should be able to view the employee employment dates error details for multiple employees")]
    public async Task ThenIShouldBeAbleToViewTheEmployeeEmploymentDatesErrorDetailsForMultipleEmployees()
    {
        UploadErrorSummary expectedError = ScenarioContext.Get<UploadErrorSummary>();

        List<AffectedEmployee> affectedEmployees =
            ScenarioContext.Get<List<AffectedEmployee>>(AffectedEmployeesKey);

        await UploadErrorDetailsCoordinator.VerifyErrorDetailsAsync(
            expectedError,
            affectedEmployees,
            ErrorDetailsHeaderType.EmploymentDates);
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
        await UploadDocumentCoordinator.VerifyUploadDocumentContentSnapShotAsync();
    }

    [Then("I should be able to view the validation error details for employees where the surname is the wrong length")]
    public async Task ThenIShouldBeAbleToViewTheValidationErrorDetailsForEmployeesWhereTheSurnameIsTheWrongLength()
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

    [Then("I should be able to view multiple national insurance numbers error details")]
    public async Task ThenIShouldBeAbleToViewMultipleNationalInsuranceNumbersErrorDetails()
    {
        UploadErrorSummary expectedError =
           ScenarioContext.Get<UploadErrorSummary>();

        List<AffectedEmployee> affectedEmployees =
            ScenarioContext.Get<List<AffectedEmployee>>(AffectedEmployeesKey);

        await UploadErrorDetailsCoordinator.VerifyErrorDetailsAsync(
            expectedError,
            affectedEmployees,
            ErrorDetailsHeaderType.NationalInsuranceNumber);
    }

    [Then("the submission should succeed")]
    public async Task ThenTheSubmissionShouldSucceed()
    {
        await _checkYourAnswersCoordinator.VerifyCheckYourAnswersPageIsDisplayedAsync();
    }

    [Then("I should see the employee surname error {string} with hint {string}")]
    public async Task ThenIShouldSeeTheEmployeeSurnameError(string errorMessage, string hintText)
    {
        UploadErrorSummary expectedError = new(
            Category: "Employee",
            ErrorType: "Employee surname",
            ErrorMessage: errorMessage,
            HintText: hintText);

        await UploadErrorDetailsCoordinator.VerifyErrorSummaryIsDisplayedAsync(expectedError);
    }

    [Then("I should see the following national insurance number validation errors")]
    public async Task ThenIShouldSeeTheFollowingNationalInsuranceNumberValidationErrors(DataTable dataTable)
    {
        Error error = dataTable.CreateInstance<Error>();
        UploadErrorSummary expectedError = CreateEmployeeErrorSummary(error.Type, error.Message, error.Hint);
        await UploadErrorDetailsCoordinator.VerifyErrorSummaryIsDisplayedAsync(expectedError);

        ScenarioContext.Set(expectedError);
    }
}
