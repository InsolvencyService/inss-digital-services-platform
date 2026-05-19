using GovUk.Forms.HostApp.UI.Test.Factories;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Support;
using System.Globalization;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;

public sealed class Rp14aScenarioCoordinator : IRp14aScenarioCoordinator
{
    private const string BaselineRp14aFilePath =
        "Resources/Rp14a/rp14A.xml";

    private readonly IFileUploadCoordinator _fileUploadCoordinator;
    private readonly ScenarioContext _scenarioContext;
    private readonly TestArtifacts _testArtifacts;

    public Rp14aScenarioCoordinator(
        IFileUploadCoordinator fileUploadCoordinator,
        ScenarioContext scenarioContext,
        TestArtifacts testArtifacts)
    {
        _fileUploadCoordinator = fileUploadCoordinator;
        _scenarioContext = scenarioContext;
        _testArtifacts = testArtifacts;
    }

    public async Task UploadValidRp14aAsync()
    {
        string filePath = Rp14aXmlFixtureFactory.CreateValidCopy(
            _testArtifacts,
            BaselineRp14aFilePath,
            ScenarioName);

        await _fileUploadCoordinator.UploadFileAsync(filePath);
    }

    public async Task UploadRp14aWithCaseReferenceAsync(string? caseReference)
    {
        string filePath = Rp14aXmlFixtureFactory.CreateWithCaseReference(
            _testArtifacts,
            BaselineRp14aFilePath,
            caseReference,
            ScenarioName);

        await _fileUploadCoordinator.UploadFileAsync(filePath);
    }

    public async Task UploadRp14aWithEmployerNameAsync(string? employerName)
    {
        string filePath = Rp14aXmlFixtureFactory.CreateWithEmployerName(
            _testArtifacts,
            BaselineRp14aFilePath,
            employerName,
            ScenarioName);

        await _fileUploadCoordinator.UploadFileAsync(filePath);
    }

    public async Task UploadRp14aWithEmployerNameLengthAsync(int length)
    {
        ValidatePositiveNumber(length, nameof(length));

        await UploadRp14aWithEmployerNameAsync(new string('A', length));
    }

    public async Task UploadRp14aWithEmployeeNameAsync(
        string? surname,
        string? forename,
        string title = "Ms")
    {
        string filePath = Rp14aXmlFixtureFactory.CreateWithEmployeeName(
            _testArtifacts,
            BaselineRp14aFilePath,
            surname,
            forename,
            title,
            ScenarioName);

        await _fileUploadCoordinator.UploadFileAsync(filePath);
    }

    public async Task UploadRp14aWithArrearsOfPayOwedAsync(string? arrearsOfPay)
    {
        string filePath = Rp14aXmlFixtureFactory.CreateWithArrearsAmount(
            _testArtifacts,
            BaselineRp14aFilePath,
            periodNumber: 1,
            amountOwed: arrearsOfPay,
            scenarioName: ScenarioName);

        await _fileUploadCoordinator.UploadFileAsync(filePath);
    }

    public async Task UploadRp14aWithInvalidArrearsOfPayOwedAsync(int count)
    {
        ValidatePositiveNumber(count, nameof(count));

        string[] invalidValues = ["15.3", "12.345", "-100"];
        Dictionary<string, string> mutations = [];
        List<AffectedEmployee> affectedEmployees = [];

        for (int i = 1; i <= count; i++)
        {
            string invalidValue = invalidValues[(i - 1) % invalidValues.Length];

            mutations[$"AOPOwed{i}"] = invalidValue;

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

        string filePath = Rp14aXmlFixtureFactory.CreateWithElementValues(
            _testArtifacts,
            BaselineRp14aFilePath,
            mutations,
            ScenarioName);

        _scenarioContext.Set(affectedEmployees, "AffectedEmployees");

        await _fileUploadCoordinator.UploadFileAsync(filePath);
    }
    public async Task UploadRp14aWithNationalInsuranceNumberAsync(
        string? insuranceNumber,
        int occurrenceIndex)
    {
        string filePath = Rp14aXmlFixtureFactory.CreateWithNationalInsuranceNumber(
            _testArtifacts,
            BaselineRp14aFilePath,
            insuranceNumber,
            occurrenceIndex,
            ScenarioName);

        await _fileUploadCoordinator.UploadFileAsync(filePath);
    }

    public async Task UploadRp14aWithMoneyOwedToEmployerAsync(string? moneyOwed)
    {
        string filePath = Rp14aXmlFixtureFactory.CreateWithMoneyOwedToEmployer(
            _testArtifacts,
            BaselineRp14aFilePath,
            moneyOwed,
            ScenarioName);

        await _fileUploadCoordinator.UploadFileAsync(filePath);
    }

    public async Task UploadRp14aWithEmploymentDatesAsync(
        DateOnly? startDate,
        DateOnly? endDate)
    {
        string filePath = Rp14aXmlFixtureFactory.CreateWithEmploymentDates(
            _testArtifacts,
            BaselineRp14aFilePath,
            startDate,
            endDate,
            ScenarioName);

        await _fileUploadCoordinator.UploadFileAsync(filePath);
    }

    public async Task UploadRp14aWithArrearsDatesAsync(
        string? startDate,
        string? endDate)
    {
        string filePath = Rp14aXmlFixtureFactory.CreateWithArrearsDates(
            _testArtifacts,
            BaselineRp14aFilePath,
            startDate,
            endDate,
            ScenarioName);

        await _fileUploadCoordinator.UploadFileAsync(filePath);
    }

    public async Task UploadComplexRp14aScenarioAsync(
        string employerName,
        string surname,
        string forename,
        string arrearsAmount,
        DateOnly? employmentStartDate,
        DateOnly? employmentEndDate)
    {
        Dictionary<string, string> mutations = new()
        {
            ["EmployerName"] = employerName,
            ["Surname"] = surname,
            ["Forenames"] = forename,
            ["AOPOwed1"] = arrearsAmount,
            ["StartDate"] = FormatDate(employmentStartDate),
            ["EndDate"] = FormatDate(employmentEndDate)
        };

        string filePath = Rp14aXmlFixtureFactory.CreateWithElementValues(
            _testArtifacts,
            BaselineRp14aFilePath,
            mutations,
            ScenarioName);

        await _fileUploadCoordinator.UploadFileAsync(filePath);
    }

    private string ScenarioName => _scenarioContext.ScenarioInfo.Title;

    private static string FormatDate(DateOnly? date)
    {
        return date?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
               ?? string.Empty;
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
