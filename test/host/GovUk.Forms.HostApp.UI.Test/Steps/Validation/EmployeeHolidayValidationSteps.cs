using GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Models.TestData;
using GovUk.Forms.HostApp.UI.Test.Steps.Base;
using GovUk.Forms.HostApp.UI.Test.Tags;
using static GovUk.Forms.HostApp.UI.Test.Models.TestData.TestFactory;

namespace GovUk.Forms.HostApp.UI.Test.Steps.Validation;

[Scope(Feature = "Employee Holiday Validation")]
[Binding]
public sealed class EmployeeHolidayValidationSteps : ValidationStepsBase
{

    public EmployeeHolidayValidationSteps(
        UploadDocumentCoordinator uploadDocumentCoordinator,
        UploadErrorDetailsCoordinator uploadErrorDetailsCoordinator,
        ScenarioContext scenarioContext) : base(uploadDocumentCoordinator, uploadErrorDetailsCoordinator, scenarioContext)
    {
    }

    [Given("the RP14A contains no contracted holiday entitlement")]
    public async Task GivenTheRp14aContainsNoContractedHolidayEntitlement()
    {
        AffectedEmployee employee = CreateAffectedEmployee(
         cellValue: string.Empty);

        await UploadDocumentCoordinator
            .UploadRp14aWithHolidayContractedEntitlementDaysAsync(null);

        ScenarioContext.Set(employee);
    }

    [Given("the RP14A contains contracted holiday entitlement {string}")]
    public async Task GivenTheRPAContainsContractedHolidayEntitlement(string contractedHoliday)
    {
        AffectedEmployee employee = CreateAffectedEmployee(
             cellValue: contractedHoliday);

        await UploadDocumentCoordinator
            .UploadRp14aWithHolidayContractedEntitlementDaysAsync(contractedHoliday);

        ScenarioContext.Set(employee);
    }

    [Given("the RP14A contains no holiday days carried forward")]
    public async Task GivenTheRp14aContainsNoHolidayDaysCarriedForward()
    {
        AffectedEmployee employee = CreateAffectedEmployee(
             cellValue: string.Empty);

        await UploadDocumentCoordinator
            .UploadRp14aWithHolidayDaysCarriedForwardAsync(null);

        ScenarioContext.Set(employee);
    }
    [Given("the RP14A contains holiday days carried forward {string}")]
    public async Task GivenTheRP14AContainsHolidayDaysCarriedForward(string holidayCarriedForward)
    {
        AffectedEmployee employee = CreateAffectedEmployee(
            cellValue: holidayCarriedForward);

        await UploadDocumentCoordinator
            .UploadRp14aWithHolidayDaysCarriedForwardAsync(holidayCarriedForward);

        ScenarioContext.Set(employee);
    }

    [Given("the RP14A contains holiday days taken {string}")]
    public async Task GivenTheRp14aContainsHolidayDaysTaken(string holidayDaysTaken)
    {
        AffectedEmployee employee = CreateAffectedEmployee(
           cellValue: holidayDaysTaken);

        await UploadDocumentCoordinator
            .UploadRp14aWithHolidayDaysTakenAsync(holidayDaysTaken);

        ScenarioContext.Set(employee);
    }

    [Given("the RP14A contains no holiday days taken")]
    public async Task GivenTheRPAContainsNoHolidayDaysTaken()
    {
        AffectedEmployee employee = CreateAffectedEmployee(
       cellValue: string.Empty);

        await UploadDocumentCoordinator
            .UploadRp14aWithHolidayDaysTakenAsync(null);

        ScenarioContext.Set(employee);
    }


    [Given("the RP14A contains holiday owed {string}")]
    public async Task GivenTheRp14aContainsHolidayOwed(string holidayOwed)
    {
        AffectedEmployee employee = CreateAffectedEmployee(
           cellValue: holidayOwed);

        await UploadDocumentCoordinator
            .UploadRp14aWithHolidayOwedAsync(holidayOwed);

        ScenarioContext.Set(employee);
    }

    [Given("the RP14A contains no holiday owed")]
    public async Task GivenTheRPAContainsNoHolidayOwed()
    {
        AffectedEmployee employee = CreateAffectedEmployee(
        cellValue: string.Empty);

        await UploadDocumentCoordinator.UploadRp14aWithHolidayOwedAsync(null);

        ScenarioContext.Set(employee);
    }

    [Given("the RP14A contains holiday not paid start date after end date")]
    public async Task GivenTheRp14aContainsHolidayNotPaidStartDateAfterEndDate()
    {
        DateOnly endDate = new(2020, 02, 10);
        DateOnly startDate = new(2020, 02, 20);

        AffectedEmployee employee = CreateAffectedEmployee(
            cellValue: FormatDateRange(startDate, endDate));

        await UploadDocumentCoordinator
            .UploadRp14aWithHolidayNotPaidDatesAsync(
                startDate,
                endDate);

        ScenarioContext.Set(employee);
    }

    [Given("the RP14A contains {int} employees with invalid holiday owed")]
    public async Task GivenTheRp14aContainsEmployeesWithInvalidHolidayOwed(int employeeCount)
    {
        string[] invalidValues =
        [
            "28.8555",
        "12.3",
        "12.345",
        "-1"
        ];

        await UploadDocumentCoordinator
            .UploadRp14aWithInvalidHolidayOwedForEmployeesAsync(
                employeeCount,
                invalidValues);
    }

    [Given("the RP14A contains {int} invalid holiday days taken values {string}")]
    public async Task GivenTheRPAContainsInvalidHolidayDaysTakenValues(int employeeCount, string holidayDaysTaken)
    {
        await UploadDocumentCoordinator
            .UploadRp14aWithHolidayDaysTakenForEmployeesAsync(employeeCount, holidayDaysTaken);
    }

    [Given("the RP14A contains {int} employees with invalid holiday days carried forward {string}")]
    public async Task GivenTheRPAContainsEmployeesWithInvalidHolidayDaysCarriedForward(int count, string holidayCarriedForward)
    {
        await UploadDocumentCoordinator
       .UploadRp14aWithHolidayDaysCarriedForwardForEmployeesAsync(count, holidayCarriedForward);
    }


    [Then("I should see the validation error {string}")]
    public async Task ThenIShouldSeeTheValidationError(string errorMessage)
    {
        UploadErrorSummary expectedError = CreateErrorSummary(
           "Contracted holiday entitlement",
           errorMessage,
           null,
           "Employee holiday");

        await UploadErrorDetailsCoordinator.VerifyErrorSummaryIsDisplayedAsync(
            expectedError);

        ScenarioContext.Set(expectedError);
    }

    [Then("I should see the following validation errors")]
    public async Task ThenIShouldSeeTheFollowingContractedHolidayEntitlementValidationErrors(DataTable dataTable)
    {
        Error error = dataTable.CreateInstance<Error>();
        UploadErrorSummary expectedError = CreateErrorSummary(error.Type, error.Message, error.Hint);
        await UploadErrorDetailsCoordinator.VerifyErrorSummaryIsDisplayedAsync(expectedError);
        ScenarioContext.Set(expectedError);
    }

    [Then("I should see the following multiple validation errors")]
    public async Task ThenIShouldSeeTheFollowingMultipleValidationErrors(DataTable dataTable)
    {
        List<UploadErrorSummary> expectedErrors = dataTable
            .CreateSet<Error>()
            .Select(error => CreateErrorSummary(error.Type, error.Message, error.Hint))
            .ToList();

        await Task.WhenAll(
            expectedErrors.Select(UploadErrorDetailsCoordinator.VerifyErrorSummaryIsDisplayedAsync)
        );

        ScenarioContext.Set(expectedErrors);
    }


    [Then("I should be able to view contracted holiday entitlement validation error details")]
    public async Task ThenIShouldBeAbleToViewContractedHolidayEntitlementValidationErrorDetails()
    {
        await VerifySingleEmployeeErrorDetailsAsync(ErrorDetailsHeaderType.HolidayContractedEntitlementDays);
    }

    [Then("I should be able to view holiday days carried forward validation error details")]
    public async Task ThenIShouldBeAbleToViewHolidayDaysCarriedForwardValidationErrorDetails()
    {
        await VerifySingleEmployeeErrorDetailsAsync(ErrorDetailsHeaderType.HolidayDaysCarriedForward);
    }

    [Then("I should be able to view holiday days taken validation error details")]
    public async Task ThenIShouldBeAbleToViewHolidayDaysTakenValidationErrorDetails()
    {
        await VerifySingleEmployeeErrorDetailsAsync(ErrorDetailsHeaderType.HolidayDaysTaken);
    }

    [Then("I should be able to view Holiday owed validation error details")]
    public async Task ThenIShouldBeAbleToViewHolidayOwedValidationErrorDetails()
    {
        await VerifySingleEmployeeErrorDetailsAsync(ErrorDetailsHeaderType.NoDaysHolidayOwed);
    }

    [Then("I should be able to view the list of Holiday owed validation error details")]
    public async Task ThenIShouldBeAbleToViewTheListOfHolidayOwedValidationErrorDetails()
    {
        List<UploadErrorSummary> expectedErrors =
           ScenarioContext.Get<List<UploadErrorSummary>>();

        List<AffectedEmployee> affectedEmployees =
            ScenarioContext.Get<List<AffectedEmployee>>(AffectedEmployeesKey);

        await UploadErrorDetailsCoordinator.VerifyErrorDetailsAsync(
            expectedErrors.First(),
            affectedEmployees,
            ErrorDetailsHeaderType.NoDaysHolidayOwed);

        await UploadErrorDetailsCoordinator.ClickBackAndVerifyUploadErrorPageIsDisplayedAsync();
    }

    [Then("I should be able to view the invalid range of holiday owed validation error details")]
    public async Task ThenIShouldBeAbleToViewTheInvalidRangeOfHolidayOwedValidationErrorDetails()
    {
        List<UploadErrorSummary> expectedErrors =
            ScenarioContext.Get<List<UploadErrorSummary>>();

        List<AffectedEmployee> affectedEmployees =
            ScenarioContext.Get<List<AffectedEmployee>>(AffectedEmployeesKey);

        List<AffectedEmployee> invalidRangeEmployees = affectedEmployees
            .Where(employee => employee.CellValue == "-1")
            .ToList();

        if (invalidRangeEmployees.Count == 0)
        {
            throw new InvalidOperationException(
                "No affected employees found with holiday owed value '-1'.");
        }

        await UploadErrorDetailsCoordinator.VerifyErrorDetailsAsync(
            expectedErrors.Last(),
            invalidRangeEmployees,
            ErrorDetailsHeaderType.NoDaysHolidayOwed);

    }

    [Then("I should be able to view holiday days taken for multiple employees error details")]
    public async Task ThenIShouldBeAbleToViewHolidayDaysTakenForMultipleEmployeesErrorDetails()
    {
        UploadErrorSummary expectedErrors =
         ScenarioContext.Get<UploadErrorSummary>();

        List<AffectedEmployee> affectedEmployees =
            ScenarioContext.Get<List<AffectedEmployee>>(AffectedEmployeesKey);

        await UploadErrorDetailsCoordinator.VerifyErrorDetailsAsync(
            expectedErrors,
            affectedEmployees,
            ErrorDetailsHeaderType.HolidayDaysTaken);
    }


}
