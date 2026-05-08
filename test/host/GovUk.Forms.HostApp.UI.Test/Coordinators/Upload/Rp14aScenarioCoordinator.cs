using GovUk.Forms.HostApp.UI.Test.Builders;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Support;
using System.Globalization;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;

public sealed class Rp14aScenarioCoordinator : IRp14aScenarioCoordinator
{
    private readonly IFileUploadCoordinator _fileUploadCoordinator;
    private readonly ScenarioContext _scenarioContext;

    public Rp14aScenarioCoordinator(
        IFileUploadCoordinator fileUploadCoordinator,
        ScenarioContext scenarioContext)
    {
        _fileUploadCoordinator = fileUploadCoordinator;
        _scenarioContext = scenarioContext;
    }

    public async Task UploadValidRp14aAsync()
    {
        Rp14aScenarioBuilder scenario = Rp14aScenarioBuilder.Create()
            .WithCaseReference(Rp14aCaseReferences.Default);

        await _fileUploadCoordinator.UploadRp14aAsync(scenario);
    }

    public async Task UploadRp14aWithCaseReferenceAsync(string? caseReference)
    {
        Rp14aScenarioBuilder scenario = Rp14aScenarioBuilder.Create()
            .WithCaseReference(caseReference);

        await _fileUploadCoordinator.UploadRp14aAsync(scenario);
    }

    public async Task UploadRp14aWithEmployerNameAsync(string? employerName)
    {
        Rp14aScenarioBuilder scenario = Rp14aScenarioBuilder.Create()
            .WithCaseReference(Rp14aCaseReferences.EmployerScenario)
            .WithEmployerName(employerName);

        await _fileUploadCoordinator.UploadRp14aAsync(scenario);
    }

    public async Task UploadRp14aWithEmployerNameLengthAsync(int length)
    {
        ValidatePositiveNumber(length, nameof(length));

        Rp14aScenarioBuilder scenario = Rp14aScenarioBuilder.Create()
            .WithCaseReference(Rp14aCaseReferences.EmployerScenario)
            .WithEmployerNameLength(length);

        await _fileUploadCoordinator.UploadRp14aAsync(scenario);
    }

    public async Task UploadRp14aWithEmployeeNameAsync(
        string? surname,
        string? forename,
        string title = "Ms")
    {
        Rp14aScenarioBuilder scenario = Rp14aScenarioBuilder.Create()
            .WithCaseReference(Rp14aCaseReferences.EmployeeScenario)
            .WithEmployeeName(
                surname ?? string.Empty,
                forename ?? string.Empty,
                title);

        await _fileUploadCoordinator.UploadRp14aAsync(scenario);
    }

    public async Task UploadRp14aWithArrearsOfPayOwedAsync(string? arrearsOfPay)
    {
        Rp14aScenarioBuilder scenario = Rp14aScenarioBuilder.Create()
            .WithCaseReference(Rp14aCaseReferences.ArrearsOfPayScenario)
            .WithArrearsOfPayPeriod(
                periodNumber: 1,
                startDate: "2020-01-10",
                endDate: "2020-01-11",
                amountOwed: arrearsOfPay ?? string.Empty,
                payType: "overtime");

        await _fileUploadCoordinator.UploadRp14aAsync(scenario);
    }

    public async Task UploadRp14aWithInvalidArrearsOfPayOwedAsync(int count)
    {
        ValidatePositiveNumber(count, nameof(count));

        Rp14aScenarioBuilder scenario = Rp14aScenarioBuilder.Create()
            .WithCaseReference(Rp14aCaseReferences.InvalidArrearsScenario);

        string[] invalidValues = ["15.3", "12.345", "-100"];
        List<AffectedEmployee> affectedEmployees = [];

        for (int i = 1; i <= count; i++)
        {
            string invalidValue = invalidValues[(i - 1) % invalidValues.Length];

            scenario.WithArrearsOfPayPeriod(
                periodNumber: i,
                startDate: $"2020-01-{9 + i:00}",
                endDate: $"2020-01-{10 + i:00}",
                amountOwed: invalidValue,
                payType: "overtime");

            affectedEmployees.Add(new AffectedEmployee
            {
                Forename = ScenarioConstant.Forname,
                Surname = ScenarioConstant.Surname,
                DateOfBirth = DateTime
              .ParseExact(
                  ScenarioConstant.DOB,
                  "yyyy-MM-dd",
                  CultureInfo.InvariantCulture)
              .ToString("d/M/yyyy", CultureInfo.InvariantCulture),
                NiNumber = ScenarioConstant.NationalInsuranceNumber,
                CellValue = invalidValue
            });
        }

        _scenarioContext.Set(affectedEmployees, "AffectedEmployees");

        await _fileUploadCoordinator.UploadRp14aAsync(scenario);
    }

    public async Task UploadRp14aWithNationalInsuranceNumberAsync(string? insuranceNumber)
    {
        Rp14aScenarioBuilder scenario = Rp14aScenarioBuilder.Create()
            .WithCaseReference(Rp14aCaseReferences.NationalInsuranceNumberScenario)
            .WithNationalInsuranceNumber(insuranceNumber);

        await _fileUploadCoordinator.UploadRp14aAsync(scenario);
    }

    public async Task UploadRp14aWithMoneyOwedToEmployerAsync(string? moneyOwed)
    {
        Rp14aScenarioBuilder scenario = Rp14aScenarioBuilder.Create()
            .WithCaseReference(Rp14aCaseReferences.MoneyOwedScenario)
            .WithMoneyOwedToEmployer(moneyOwed);

        await _fileUploadCoordinator.UploadRp14aAsync(scenario);
    }

    public async Task UploadRp14aWithEmploymentDatesAsync(
        string? startDate,
        string? endDate)
    {
        Rp14aScenarioBuilder scenario = Rp14aScenarioBuilder.Create()
            .WithCaseReference(Rp14aCaseReferences.EmploymentDatesScenario)
            .WithEmploymentDates(
                startDate ?? string.Empty,
                endDate ?? string.Empty);

        await _fileUploadCoordinator.UploadRp14aAsync(scenario);
    }

    public async Task UploadRp14aWithArrearsDatesAsync(
        string? startDate,
        string? endDate)
    {
        Rp14aScenarioBuilder scenario = Rp14aScenarioBuilder.Create()
            .WithCaseReference(Rp14aCaseReferences.ArrearsDatesScenario)
            .WithArrearsOfPayPeriod(
                periodNumber: 1,
                startDate: startDate ?? string.Empty,
                endDate: endDate ?? string.Empty,
                amountOwed: "100",
                payType: "overtime");

        await _fileUploadCoordinator.UploadRp14aAsync(scenario);
    }

    public async Task UploadComplexRp14aScenarioAsync(
        string employerName,
        string surname,
        string forename,
        string arrearsAmount,
        string employmentStartDate,
        string employmentEndDate)
    {
        Rp14aScenarioBuilder scenario = Rp14aScenarioBuilder.Create()
            .WithCaseReference(Rp14aCaseReferences.Default)
            .WithEmployerName(employerName)
            .WithEmployeeName(surname, forename)
            .WithArrearsOfPayPeriod(
                periodNumber: 1,
                startDate: "2020-01-10",
                endDate: "2020-01-11",
                amountOwed: arrearsAmount,
                payType: "overtime")
            .WithEmploymentDates(
                employmentStartDate,
                employmentEndDate);

        await _fileUploadCoordinator.UploadRp14aAsync(scenario);
    }

    private static void ValidatePositiveNumber(int value, string parameterName)
    {
        if (value <= 0)
        {
            throw new ArgumentException(
                $"Parameter '{parameterName}' must be greater than 0.",
                parameterName);
        }
    }
}
