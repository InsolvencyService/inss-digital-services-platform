using Bogus;
using GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Models.TestData;
using GovUk.Forms.HostApp.UI.Test.Steps.Base;
using GovUk.Forms.HostApp.UI.Test.Support;
using GovUk.Forms.HostApp.UI.Test.Tags;
using static GovUk.Forms.HostApp.UI.Test.Models.TestData.TestFactory;

namespace GovUk.Forms.HostApp.UI.Test.Steps.Validation;

[Scope(Feature = "Employee Validation")]
[Binding]
public sealed class EmployeeValidationSteps : ValidationStepsBase
{
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
        await VerifySingleEmployeeErrorDetailsAsync(
            ErrorDetailsHeaderType.EmployeeSurname);
    }


    [Then("I should be able to view national insurance number error details")]
    public async Task ThenIShouldBeAbleToViewNationalInsuranceNumberErrorDetails()
    {
        await VerifySingleEmployeeErrorDetailsAsync(
            ErrorDetailsHeaderType.NationalInsuranceNumber);
    }

    [Then("I should be able to view the employee employment dates error details")]
    public async Task ThenIShouldBeAbleToViewTheEmployeeEmploymentDatesErrorDetails()
    {
        await VerifySingleEmployeeErrorDetailsAsync(
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
