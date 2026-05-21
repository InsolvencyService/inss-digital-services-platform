using GovUk.Forms.HostApp.UI.Test.Builders;
using GovUk.Forms.HostApp.UI.Test.Factories;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Support;
using System.Diagnostics;
using System.Globalization;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;

public sealed class Rp14aScenarioCoordinator : IRp14aScenarioCoordinator
{
    private const string DefaultBaselineFilePath = "Resources/Rp14a/rp14A.xml";
    private const string AffectedEmployeesContextKey = "AffectedEmployees";
    private const string AffectedEmployeesByErrorTypeKey = "AffectedEmployeesByErrorType";

    private readonly IFileUploadCoordinator _fileUploadCoordinator;
    private readonly ScenarioContext _scenarioContext;
    private readonly TestArtifacts _testArtifacts;
    private readonly string _baselineFilePath;

    public Rp14aScenarioCoordinator(
        IFileUploadCoordinator fileUploadCoordinator,
        ScenarioContext scenarioContext,
        TestArtifacts testArtifacts,
        string? baselineFilePath = null)
    {
        _fileUploadCoordinator = fileUploadCoordinator
            ?? throw new ArgumentNullException(nameof(fileUploadCoordinator));

        _scenarioContext = scenarioContext
            ?? throw new ArgumentNullException(nameof(scenarioContext));

        _testArtifacts = testArtifacts
            ?? throw new ArgumentNullException(nameof(testArtifacts));

        _baselineFilePath = ValidateBaselineFilePath(baselineFilePath);
    }

    public async Task UploadValidRp14aAsync()
    {
        await UploadFixtureAsync(
            () => BuildFixture().FilePath,
            "valid RP14A fixture");
    }

    public async Task UploadRp14aWithCaseReferenceAsync(string? caseReference)
    {
        await UploadFixtureAsync(
            () => BuildFixture(builder =>
                builder.WithCaseReference(caseReference)).FilePath,
            $"RP14A with case reference '{ToLogValue(caseReference)}'");
    }

    public async Task UploadRp14aWithEmployerNameAsync(string? employerName)
    {
        await UploadFixtureAsync(
            () => BuildFixture(builder =>
                builder.WithEmployerName(employerName)).FilePath,
            $"RP14A with employer name '{ToLogValue(employerName)}'");
    }

    public async Task UploadRp14aWithHolidayDaysTakenAsync(
    string? holidayDaysTaken)
    {
        await UploadFixtureAsync(
            () => BuildFixture(builder =>
                builder.WithHolidayDaysTaken(holidayDaysTaken)).FilePath,
            $"RP14A with holiday days taken '{ToLogValue(holidayDaysTaken)}'");
    }

    public async Task UploadRp14aWithEmployerNameLengthAsync(int length)
    {
        ValidatePositiveNumber(length, nameof(length));

        string employerName = new('A', length);
        await UploadRp14aWithEmployerNameAsync(employerName);
    }

    public async Task UploadRp14aWithEmployeeNameAsync(
        string? surname,
        string? forename,
        string? title = null)
    {
        await UploadFixtureAsync(
            () => BuildFixture(builder =>
                builder.WithEmployeeName(surname, forename, title)).FilePath,
            $"RP14A with employee forename '{ToLogValue(forename)}', surname '{ToLogValue(surname)}', title '{ToLogValue(title)}'");
    }

    public async Task UploadRp14aWithEmployeeBasicPayPerWeekAsync(
        string? basicPayPerWeek)
    {
        await UploadFixtureAsync(
            () => BuildFixture(builder =>
                builder.WithEmployeeBasicPayPerWeek(basicPayPerWeek)).FilePath,
            $"RP14A with employee basic pay per week '{ToLogValue(basicPayPerWeek)}'");
    }

    public async Task UploadRp14aWithArrearsOfPayOwedAsync(string? arrearsOfPay)
    {
        await UploadFixtureAsync(
            () => BuildFixture(builder =>
                builder.WithArrearsAmount(1, arrearsOfPay)).FilePath,
            $"RP14A with arrears of pay '{ToLogValue(arrearsOfPay)}'");
    }

    public async Task UploadRp14aWithHolidayOwedAsync(
    string? holidayOwed)
    {
        await UploadFixtureAsync(
            () => BuildFixture(builder =>
                builder.WithHolidayOwed(holidayOwed)).FilePath,
            $"RP14A with holiday owed '{ToLogValue(holidayOwed)}'");
    }

    public async Task UploadRp14aWithInvalidArrearsOfPayOwedAsync(int count)
    {
        ValidatePositiveNumber(count, nameof(count));

        List<AffectedEmployee> affectedEmployees = CreateInvalidArrearsScenario(count);

        await UploadFixtureAsync(
            () => BuildFixture(builder =>
            {
                string[] invalidValues = ["15.3", "12.345", "-100"];

                for (int i = 1; i <= count; i++)
                {
                    builder.WithArrearsAmount(
                        i,
                        invalidValues[(i - 1) % invalidValues.Length]);
                }
            }).FilePath,
            $"RP14A with {count} invalid arrears values");

        _scenarioContext.Set(affectedEmployees, AffectedEmployeesContextKey);
    }

    public async Task UploadRp14aWithHolidayNotPaidDatesAsync(DateOnly? startDate, DateOnly? endDate)
    {
        await UploadFixtureAsync(
            () => BuildFixture(builder =>
                builder.WithHolidayNotPaidDates(
                    startDate,
                    endDate)).FilePath,
            $"RP14A with holiday not paid dates '{FormatDate(startDate)}' to '{FormatDate(endDate)}'");
    }

    public async Task UploadRp14aWithNationalInsuranceNumberAsync(
        string? insuranceNumber,
        int occurrenceIndex)
    {
        ValidateNonNegativeNumber(occurrenceIndex, nameof(occurrenceIndex));

        await UploadFixtureAsync(
            () => BuildFixture(builder =>
                builder
                    .WithEmployeeIndex(occurrenceIndex)
                    .WithCustomMutation(
                        RP14AElementNames.NationalInsuranceNumber,
                        ToXmlValue(insuranceNumber))).FilePath,
            $"RP14A with NI number '{ToLogValue(insuranceNumber)}' at index {occurrenceIndex}");
    }

    public async Task UploadRp14aWithMoneyOwedToEmployerAsync(string? moneyOwed)
    {
        await UploadFixtureAsync(
            () => BuildFixture(builder =>
                builder.WithMoneyOwedToEmployer(moneyOwed)).FilePath,
            $"RP14A with money owed to employer '{ToLogValue(moneyOwed)}'");
    }

    public async Task UploadRp14aWithEmploymentDatesAsync(DateOnly? startDate, DateOnly? endDate)
    {
        await UploadFixtureAsync(
            () => BuildFixture(builder =>
                builder.WithEmploymentDates(startDate, endDate)).FilePath,
            $"RP14A with employment dates {FormatDate(startDate)} to {FormatDate(endDate)}");
    }

    public async Task UploadRp14aWithHolidayContractedEntitlementDaysAsync(
     string? entitlementDays)
    {
        await UploadFixtureAsync(
            () => BuildFixture(builder =>
                builder.WithHolidayContractedEntitlementDays(entitlementDays)).FilePath,
            $"RP14A with holiday contracted entitlement days '{ToLogValue(entitlementDays)}'");
    }

    public async Task UploadRp14aWithHolidayDaysCarriedForwardAsync(
    string? daysCarriedForward)
    {
        await UploadFixtureAsync(
            () => BuildFixture(builder =>
                builder.WithHolidayDaysCarriedForward(daysCarriedForward)).FilePath,
            $"RP14A with holiday days carried forward '{ToLogValue(daysCarriedForward)}'");
    }

    public async Task UploadRp14aWithArrearsDatesAsync(DateOnly? startDate, DateOnly? endDate)
    {
        await UploadFixtureAsync(
            () => BuildFixture(builder =>
                builder.WithArrearsDates(startDate, endDate)).FilePath,
            $"RP14A with arrears dates '{ToLogValue(FormatDate(startDate))}' to '{ToLogValue(FormatDate(endDate))}'");
    }

    public async Task UploadComplexRp14aScenarioAsync(
     string? caseReference = null,
     string? employerName = null,
     string? surname = null,
     string? forename = null,
     string? title = null,
     string? arrearsAmount = null,
     string? basicPayPerWeek = null,
     string? holidayOwed = null,
     DateOnly? employmentStartDate = null,
     DateOnly? employmentEndDate = null)
    {
        ValidateDateOrder(employmentStartDate, employmentEndDate);

        Rp14aFixture fixture = BuildFixture(builder =>
        {
            if (caseReference is not null)
            {
                builder.WithCaseReference(caseReference);
            }

            if (employerName is not null)
            {
                builder.WithEmployerName(employerName);
            }

            if (surname is not null || forename is not null || title is not null)
            {
                builder.WithEmployeeName(surname, forename, title);
            }

            if (basicPayPerWeek is not null)
            {
                builder.WithEmployeeBasicPayPerWeek(basicPayPerWeek);
            }

            if (holidayOwed is not null)
            {
                builder.WithHolidayOwed(holidayOwed);
            }

            if (arrearsAmount is not null)
            {
                builder.WithArrearsAmount(1, arrearsAmount);
            }

            if (employmentStartDate.HasValue || employmentEndDate.HasValue)
            {
                builder.WithEmploymentDates(
                    employmentStartDate,
                    employmentEndDate);
            }
        });

        Dictionary<string, List<AffectedEmployee>> affectedEmployeesByError = new()
        {
            ["Employee surname|1 missing employee surname"] =
                Rp14aAffectedEmployeeReader.ReadAffectedEmployees(
                    fixture.FilePath,
                    employeeCount: 1,
                    cellValue: surname ?? string.Empty),

            ["Employee basic pay per week|1 invalid basic pay per week"] =
                Rp14aAffectedEmployeeReader.ReadAffectedEmployees(
                    fixture.FilePath,
                    employeeCount: 1,
                    cellValue: basicPayPerWeek ?? string.Empty),
            ["Holiday owed|1 invalid holiday owed"] =
                Rp14aAffectedEmployeeReader.ReadAffectedEmployees(
                    fixture.FilePath,
                    employeeCount: 1,
                    cellValue: holidayOwed ?? string.Empty),

            ["Holiday owed|1 invalid range of holiday owed"] =
                Rp14aAffectedEmployeeReader.ReadAffectedEmployees(
                    fixture.FilePath,
                    employeeCount: 1,
                    cellValue: holidayOwed ?? string.Empty)
        };


        _scenarioContext.Set(
            affectedEmployeesByError,
            AffectedEmployeesByErrorTypeKey);

        await UploadFixtureAsync(
            () => fixture.FilePath,
            "complex RP14A scenario");
    }

    public async Task UploadRp14aWithMissingEmployeeSurnamesAsync(int employeeCount)
    {
        ValidatePositiveNumber(employeeCount, nameof(employeeCount));

        Rp14aFixture fixture = BuildFixture(builder =>
            builder.WithMissingSurnames(employeeCount));

        List<AffectedEmployee> affectedEmployees =
            Rp14aAffectedEmployeeReader.ReadAffectedEmployees(
                fixture.FilePath,
                employeeCount,
                cellValue: string.Empty);

        await UploadFixtureAsync(
            () => fixture.FilePath,
            $"RP14A with {employeeCount} employees missing surname");

        _scenarioContext.Set(affectedEmployees, AffectedEmployeesContextKey);
    }

    public async Task UploadRp14aWithInvalidHolidayOwedForEmployeesAsync(int employeeCount, params string[] invalidValues)
    {
        ValidatePositiveNumber(employeeCount, nameof(employeeCount));

        Rp14aFixture fixture = BuildFixture(builder =>
            builder.WithInvalidHolidayOwedForEmployees(
                employeeCount,
                invalidValues));

        List<AffectedEmployee> affectedEmployees =
            Rp14aAffectedEmployeeReader.ReadAffectedEmployees(
                fixture.FilePath,
                employeeCount,
               cellValues: invalidValues);

        await UploadFixtureAsync(
            () => fixture.FilePath,
            $"RP14A with {employeeCount} employees invalid holiday owed");

        _scenarioContext.Set(
            affectedEmployees,
            AffectedEmployeesContextKey);
    }

    private Rp14aFixture BuildFixture(
    Action<Rp14aFixtureBuilder>? configure = null)
    {
        Rp14aFixtureBuilder builder = new();

        configure?.Invoke(builder);

        return builder.Build(
            _testArtifacts,
            _baselineFilePath,
            ScenarioName);
    }

    private async Task UploadFixtureAsync(
        Func<string> fixtureFactory,
        string description)
    {
        try
        {
            LogInfo($"Creating {description}...");

            string filePath = fixtureFactory();

            if (!File.Exists(filePath))
            {
                throw new InvalidOperationException(
                    $"Fixture file was not created at expected location: {filePath}");
            }

            LogInfo($"Uploading {description} from {filePath}");

            await _fileUploadCoordinator.UploadFileAsync(filePath);

            LogInfo($"Successfully uploaded {description}");
        }
        catch (Exception ex)
        {
            LogError($"Failed to upload {description}: {ex.GetType().Name} - {ex.Message}");
            throw;
        }
    }

    private static List<AffectedEmployee> CreateInvalidArrearsScenario(int count)
    {
        List<AffectedEmployee> affectedEmployees = [];
        string[] invalidValues = ["15.3", "12.345", "-100"];

        for (int i = 1; i <= count; i++)
        {
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
                CellValue = invalidValues[(i - 1) % invalidValues.Length]
            });
        }

        return affectedEmployees;
    }

    private static string ValidateBaselineFilePath(string? baselineFilePath)
    {
        string effectivePath = baselineFilePath ?? DefaultBaselineFilePath;

        if (string.IsNullOrWhiteSpace(effectivePath))
        {
            throw new ArgumentException(
                "Baseline file path cannot be null, empty, or whitespace",
                nameof(baselineFilePath));
        }

        string absolutePath = Path.IsPathRooted(effectivePath)
            ? effectivePath
            : Path.Join(AppContext.BaseDirectory, effectivePath);

        if (!File.Exists(absolutePath))
        {
            throw new FileNotFoundException(
                $"Baseline RP14A file not found at: {absolutePath}",
                absolutePath);
        }

        return effectivePath;
    }

    private static void ValidateDateOrder(DateOnly? startDate, DateOnly? endDate)
    {
        if (startDate.HasValue &&
            endDate.HasValue &&
            endDate < startDate)
        {
            throw new ArgumentException(
                $"End date ({endDate:yyyy-MM-dd}) cannot be before start date ({startDate:yyyy-MM-dd})",
                nameof(endDate));
        }
    }

    private string ScenarioName =>
        _scenarioContext.ScenarioInfo?.Title ?? "Unknown Scenario";

    private static string FormatDate(DateOnly? date)
    {
        return date?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? string.Empty;
    }

    private static string ToXmlValue(string? value)
    {
        return value ?? string.Empty;
    }

    private static string ToLogValue(string? value)
    {
        return value switch
        {
            null => "<null>",
            "" => "<empty>",
            _ when string.IsNullOrWhiteSpace(value) => "<whitespace>",
            _ => value
        };
    }

    private static void ValidatePositiveNumber(int value, string parameterName)
    {
        if (value <= 0)
        {
            throw new ArgumentException(
                $"Parameter '{parameterName}' must be greater than 0. Received: {value}",
                parameterName);
        }
    }

    private static void ValidateNonNegativeNumber(int value, string parameterName)
    {
        if (value < 0)
        {
            throw new ArgumentException(
                $"Parameter '{parameterName}' must be non-negative. Received: {value}",
                parameterName);
        }
    }

    private static void LogInfo(string message)
    {
        Debug.WriteLine($"[{DateTime.UtcNow:HH:mm:ss.fff}] [INFO] [RP14A] {message}");
    }

    private static void LogError(string message)
    {
        Debug.WriteLine($"[{DateTime.UtcNow:HH:mm:ss.fff}] [ERROR] [RP14A] {message}");
    }
}

