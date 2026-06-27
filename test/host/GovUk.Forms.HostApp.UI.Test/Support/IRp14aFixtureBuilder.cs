using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Models;

namespace GovUk.Forms.HostApp.UI.Test.Support;

public interface IRp14aFixtureBuilder
{
    IRp14aFixtureBuilder WithEmployeeIndex(int index);
    IRp14aFixtureBuilder WithCaseReference(string? value);
    IRp14aFixtureBuilder WithEmployerName(string? value);
    IRp14aFixtureBuilder WithEmployeeBasicPayPerWeek(string? value);
    IRp14aFixtureBuilder WithMoneyOwedToEmployer(string? value);
    IRp14aFixtureBuilder WithHolidayContractedEntitlementDays(string? value);
    IRp14aFixtureBuilder WithHolidayDaysCarriedForward(string? value);
    IRp14aFixtureBuilder WithHolidayDaysTaken(string? value);
    IRp14aFixtureBuilder WithHolidayOwed(string? value);
    IRp14aFixtureBuilder WithEmployeeName(string? surname, string? forename, string? title = null);
    IRp14aFixtureBuilder WithEmploymentDates(DateOnly? startDate, DateOnly? endDate);
    IRp14aFixtureBuilder WithArrearsDates(DateOnly? startDate, DateOnly? endDate);
    IRp14aFixtureBuilder WithHolidayNotPaidDates(DateOnly? startDate, DateOnly? endDate);
    IRp14aFixtureBuilder WithArrearsAmount(int periodNumber, string? amountOwed);
    IRp14aFixtureBuilder WithInvalidHolidayOwedForEmployees(int employeeCount, params string[] invalidValues);
    IRp14aFixtureBuilder WithMissingSurnames(int employeeCount);
    IRp14aFixtureBuilder WithCustomMutation(string elementName, string? value);
    IRp14aFixtureBuilder WithCaseReferences(params string?[] caseReferences);
    IRp14aFixtureBuilder WithNinosForEmployees(params string?[] ninos);
    IRp14aFixtureBuilder WithEmployerNames(params string?[] employerNames);
    IRp14aFixtureBuilder WithNationalInsuranceNumberForEmployees(int employeeCount, string? nationalInsuranceNumber);
    IRp14aFixtureBuilder WithMoneyOwedToEmployerForEmployees(int employeeCount, string? moneyOwed);
    IRp14aFixtureBuilder WithEmployeeBasicPayPerWeekForEmployees(int employeeCount, string? basicPayPerWeek);
    IRp14aFixtureBuilder WithHolidayDaysTakenForEmployees(int employeeCount, string? holidayDaysTaken);
    IRp14aFixtureBuilder WithHolidayDaysCarriedForwardForEmployees(int employeeCount, string? holidayDaysCarriedForward);
    IRp14aFixtureBuilder WithHolidayContractedEntitlementDaysForEmployees(int employeeCount, string? value);
    IRp14aFixtureBuilder WithHolidayNotPaidDatesForEmployees(int employeeCount, DateOnly? startDate, DateOnly? endDate);
    IRp14aFixtureBuilder WithSurnamesForEmployees(int employeeCount, string? surname);
    IRp14aFixtureBuilder WithEmploymentDatesForEmployees(int employeeCount, DateOnly? startDate, DateOnly? endDate);
    IRp14aFixtureBuilder WithArrearsDatesForEmployees(int employeeCount, DateOnly? startDate, DateOnly? endDate);
    Rp14aTestFile Build(TestArtifacts testArtifacts, string? baselineFilePath = null, string scenarioName = "Default");
}
