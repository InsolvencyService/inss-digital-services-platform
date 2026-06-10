namespace GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;

public interface IRp14aScenarioCoordinator
{
    Task UploadValidRp14aAsync();
    Task UploadRp14aWithEmployerNameAsync(params string?[] employerNames);
    Task UploadRp14aWithEmployerNameLengthAsync(int length);
    Task UploadRp14aWithEmployeeNameAsync(
        string? surname,
        string? forename,
        string? title = null);
    Task UploadRp14aWithEmployeeBasicPayPerWeekAsync(string? basicPayPerWeek);
    Task UploadRp14aWithArrearsOfPayOwedAsync(string? arrearsOfPay);
    Task UploadRp14aWithInvalidArrearsOfPayOwedAsync(int count);
    Task UploadRp14aWithNationalInsuranceNumberAsync(string? insuranceNumber, int occurrenceIndex);
    Task UploadRp14aWithMoneyOwedToEmployerAsync(string? moneyOwed);
    Task UploadRp14aWithEmploymentDatesAsync(DateOnly? startDate, DateOnly? endDate);
    Task UploadRp14aWithArrearsDatesAsync(DateOnly? startDate, DateOnly? endDate);
    Task UploadComplexRp14aScenarioAsync(
    string? caseReference = null,
    string? employerName = null,
    string? surname = null,
    string? forename = null,
    string? title = null,
    string? nationalInsuranceNumber = null,
    string? moneyOwedToEmployer = null,
    string? arrearsAmount = null,
    string? basicPayPerWeek = null,
    string? holidayOwed = null,
    DateOnly? employmentStartDate = null,
    DateOnly? employmentEndDate = null);
    Task UploadRp14aWithHolidayContractedEntitlementDaysAsync(string? entitlementDays);
    Task UploadRp14aWithHolidayDaysCarriedForwardAsync(string? daysCarriedForward);
    Task UploadRp14aWithHolidayDaysTakenAsync(string? holidayDaysTaken);
    Task UploadRp14aWithHolidayOwedAsync(string? holidayOwed);
    Task UploadRp14aWithHolidayNotPaidDatesAsync(DateOnly? startDate, DateOnly? endDate);
    Task UploadRp14aWithMissingEmployeeSurnamesAsync(int employeeCount);
    Task UploadRp14aWithInvalidHolidayOwedForEmployeesAsync(int employeeCount, params string[] invalidValues);
    Task UploadRp14aWithCaseReferenceAsync(params string?[] caseReferences);
    Task UploadRp14aWithNationalInsuranceNumberForEmployeesAsync(int employeeCount, string? nationalInsuranceNumber);
    Task UploadRp14aWithMoneyOwedToEmployerForEmployeesAsync(int employeeCount, string? moneyOwed);
    Task UploadRp14aWithEmployeeBasicPayPerWeekForEmployeesAsync(int employeeCount, string? basicPayPerWeek);
    Task UploadRp14aWithHolidayDaysTakenForEmployeesAsync(int employeeCount, string? holidayDaysTaken);
    Task UploadRp14aWithHolidayDaysCarriedForwardForEmployeesAsync(int employeeCount, string? holidayDaysCarriedForward);
    Task UploadRp14aWithHolidayContractedEntitlementDaysForEmployeesAsync(int employeeCount, string? value);
    Task UploadRp14aWithHolidayNotPaidDatesForEmployeesAsync(int employeeCount, DateOnly? startDate, DateOnly? endDate);
    Task UploadRp14aWithSurnameForEmployeesAsync(int employeeCount, string? surname);
    Task UploadRp14aWithEmploymentDatesForEmployeesAsync(int employeeCount, DateOnly? startDate, DateOnly? endDate);
    Task UploadRp14aWithArrearsDatesForEmployeesAsync(int employeeCount, DateOnly? startDate, DateOnly? endDate);
}
