namespace GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;

public interface IRp14aScenarioCoordinator
{
    Task UploadValidRp14aAsync();

    Task UploadRp14aWithCaseReferenceAsync(string? caseReference);

    Task UploadRp14aWithEmployerNameAsync(string? employerName);

    Task UploadRp14aWithEmployeeNameAsync(
        string? surname,
        string? forename,
        string title = "Ms");

    Task UploadRp14aWithEmployerNameLengthAsync(int length);

    Task UploadRp14aWithArrearsOfPayOwedAsync(string? arrearsOfPay);

    Task UploadRp14aWithInvalidArrearsOfPayOwedAsync(int count);

    Task UploadRp14aWithNationalInsuranceNumberAsync(string? insuranceNumber);

    Task UploadRp14aWithMoneyOwedToEmployerAsync(string? moneyOwed);

    Task UploadRp14aWithEmploymentDatesAsync(
        string? startDate,
        string? endDate);

    Task UploadRp14aWithArrearsDatesAsync(
        string? startDate,
        string? endDate);

    Task UploadComplexRp14aScenarioAsync(
        string employerName,
        string surname,
        string forename,
        string arrearsAmount,
        string employmentStartDate,
        string employmentEndDate);
}
