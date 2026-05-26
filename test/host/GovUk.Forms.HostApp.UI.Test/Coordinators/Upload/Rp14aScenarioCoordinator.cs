using GovUk.Forms.HostApp.UI.Test.Builders;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Support;
using System.Diagnostics;
using System.Globalization;
using System.Xml.Linq;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;

public sealed class Rp14aScenarioCoordinator : IRp14aScenarioCoordinator
{
    private const string DefaultBaselineFilePath = "Resources/Rp14a/rp14A.xml";
    private const string AffectedEmployeesContextKey = "AffectedEmployees";
    private const string AffectedEmployeesByErrorTypeKey = "AffectedEmployeesByErrorType";
    private const string SurnameErrorKey = "Employee surname|1 missing employee surname";
    private const string BasicPayErrorKey = "Employee basic pay per week|1 invalid basic pay per week";
    private const string HolidayOwedErrorKey = "Holiday owed|1 invalid holiday owed";
    private const string HolidayOwedRangeErrorKey = "Holiday owed|1 invalid range of holiday owed";
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

    public Task UploadValidRp14aAsync() =>
        BuildAndUploadAsync(
            configure: null,
            "valid RP14A file");

    public Task UploadRp14aWithCaseReferenceAsync(string? caseReference) =>
        BuildAndUploadAsync(
            builder => builder.WithCaseReference(caseReference),
            $"RP14A with case reference '{ToLogValue(caseReference)}'");

    public Task UploadRp14aWithEmployerNameAsync(string? employerName) =>
        BuildAndUploadAsync(
            builder => builder.WithEmployerName(employerName),
            $"RP14A with employer name '{ToLogValue(employerName)}'");

    public Task UploadRp14aWithHolidayDaysTakenAsync(string? holidayDaysTaken) =>
        BuildAndUploadAsync(
            builder => builder.WithHolidayDaysTaken(holidayDaysTaken),
            $"RP14A with holiday days taken '{ToLogValue(holidayDaysTaken)}'");

    public async Task UploadRp14aWithEmployerNameLengthAsync(int length)
    {
        ValidatePositiveNumber(length, nameof(length));

        await UploadRp14aWithEmployerNameAsync(new string('A', length));
    }

    public Task UploadRp14aWithEmployeeNameAsync(
        string? surname,
        string? forename,
        string? title = null) =>
        BuildAndUploadAsync(
            builder => builder.WithEmployeeName(surname, forename, title),
            $"RP14A with employee forename '{ToLogValue(forename)}', surname '{ToLogValue(surname)}', title '{ToLogValue(title)}'");

    public Task UploadRp14aWithEmployeeBasicPayPerWeekAsync(string? basicPayPerWeek) =>
        BuildAndUploadAsync(
            builder => builder.WithEmployeeBasicPayPerWeek(basicPayPerWeek),
            $"RP14A with employee basic pay per week '{ToLogValue(basicPayPerWeek)}'");

    public Task UploadRp14aWithArrearsOfPayOwedAsync(string? arrearsOfPay) =>
        BuildAndUploadAsync(
            builder => builder.WithArrearsAmount(1, arrearsOfPay),
            $"RP14A with arrears of pay '{ToLogValue(arrearsOfPay)}'");

    public Task UploadRp14aWithHolidayOwedAsync(string? holidayOwed) =>
        BuildAndUploadAsync(
            builder => builder.WithHolidayOwed(holidayOwed),
            $"RP14A with holiday owed '{ToLogValue(holidayOwed)}'");

    public async Task UploadRp14aWithInvalidArrearsOfPayOwedAsync(int count)
    {
        ValidatePositiveNumber(count, nameof(count));

        List<AffectedEmployee> affectedEmployees = CreateInvalidArrearsScenario(count);

        await BuildAndUploadAsync(
            builder =>
            {
                for (int i = 1; i <= count; i++)
                {
                    builder.WithArrearsAmount(
                        i,
                        InvalidArrearsAmounts.All[(i - 1) % InvalidArrearsAmounts.All.Count]);
                }
            },
            $"RP14A with {count} invalid arrears values");

        _scenarioContext.Set(
            affectedEmployees,
            AffectedEmployeesContextKey);
    }

    public Task UploadRp14aWithHolidayNotPaidDatesAsync(
        DateOnly? startDate,
        DateOnly? endDate) =>
        BuildAndUploadAsync(
            builder => builder.WithHolidayNotPaidDates(startDate, endDate),
            $"RP14A with holiday not paid dates '{FormatDate(startDate)}' to '{FormatDate(endDate)}'");

    public Task UploadRp14aWithNationalInsuranceNumberAsync(
        string? insuranceNumber,
        int occurrenceIndex)
    {
        ValidateNonNegativeNumber(occurrenceIndex, nameof(occurrenceIndex));

        return BuildAndUploadAsync(
            builder => builder
                .WithEmployeeIndex(occurrenceIndex)
                .WithCustomMutation(
                    RP14AElementNames.NationalInsuranceNumber,
                    ToXmlValue(insuranceNumber)),
            $"RP14A with NI number '{ToLogValue(insuranceNumber)}' at index {occurrenceIndex}");
    }

    public Task UploadRp14aWithMoneyOwedToEmployerAsync(string? moneyOwed) =>
        BuildAndUploadAsync(
            builder => builder.WithMoneyOwedToEmployer(moneyOwed),
            $"RP14A with money owed to employer '{ToLogValue(moneyOwed)}'");

    public Task UploadRp14aWithEmploymentDatesAsync(
        DateOnly? startDate,
        DateOnly? endDate) =>
        BuildAndUploadAsync(
            builder => builder.WithEmploymentDates(startDate, endDate),
            $"RP14A with employment dates {FormatDate(startDate)} to {FormatDate(endDate)}");

    public Task UploadRp14aWithHolidayContractedEntitlementDaysAsync(string? entitlementDays) =>
        BuildAndUploadAsync(
            builder => builder.WithHolidayContractedEntitlementDays(entitlementDays),
            $"RP14A with holiday contracted entitlement days '{ToLogValue(entitlementDays)}'");

    public Task UploadRp14aWithHolidayDaysCarriedForwardAsync(string? daysCarriedForward) =>
        BuildAndUploadAsync(
            builder => builder.WithHolidayDaysCarriedForward(daysCarriedForward),
            $"RP14A with holiday days carried forward '{ToLogValue(daysCarriedForward)}'");

    public Task UploadRp14aWithArrearsDatesAsync(
        DateOnly? startDate,
        DateOnly? endDate) =>
        BuildAndUploadAsync(
            builder => builder.WithArrearsDates(startDate, endDate),
            $"RP14A with arrears dates '{FormatDate(startDate)}' to '{FormatDate(endDate)}'");

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

        Rp14aTestFile testFile = BuildTestFile(builder =>
        {
            ApplyIfNotNull(caseReference, v => builder.WithCaseReference(v));
            ApplyIfNotNull(employerName, v => builder.WithEmployerName(v));
            ApplyIfNotNull(basicPayPerWeek, v => builder.WithEmployeeBasicPayPerWeek(v));
            ApplyIfNotNull(holidayOwed, v => builder.WithHolidayOwed(v));

            if (surname is not null || forename is not null || title is not null)
            {
                builder.WithEmployeeName(surname, forename, title);
            }

            if (arrearsAmount is not null)
            {
                builder.WithArrearsAmount(1, arrearsAmount);
            }

            if (employmentStartDate.HasValue || employmentEndDate.HasValue)
            {
                builder.WithEmploymentDates(employmentStartDate, employmentEndDate);
            }
        });

        SetComplexAffectedEmployees(
            testFile.FilePath,
            surname,
            basicPayPerWeek,
            holidayOwed);

        await UploadFileAsync(
            testFile.FilePath,
            "complex RP14A scenario");
    }

    public async Task UploadRp14aWithMissingEmployeeSurnamesAsync(int employeeCount)
    {
        ValidatePositiveNumber(employeeCount, nameof(employeeCount));

        Rp14aTestFile testFile = BuildTestFile(builder =>
            builder.WithMissingSurnames(employeeCount));

        List<AffectedEmployee> affectedEmployees =
            Rp14aAffectedEmployeeReader.ReadAffectedEmployees(
                testFile.FilePath,
                employeeCount,
                cellValue: string.Empty);

        await UploadFileAsync(
            testFile.FilePath,
            $"RP14A with {employeeCount} employees missing surname");

        _scenarioContext.Set(affectedEmployees, AffectedEmployeesContextKey);
    }

    public async Task UploadRp14aWithInvalidHolidayOwedForEmployeesAsync(
        int employeeCount,
        params string[] invalidValues)
    {
        ValidatePositiveNumber(employeeCount, nameof(employeeCount));

        Rp14aTestFile testFile = BuildTestFile(builder =>
            builder.WithInvalidHolidayOwedForEmployees(
                employeeCount,
                invalidValues));

        List<AffectedEmployee> affectedEmployees =
            Rp14aAffectedEmployeeReader.ReadAffectedEmployees(
                testFile.FilePath,
                employeeCount,
                cellValues: invalidValues);

        await UploadFileAsync(
            testFile.FilePath,
            $"RP14A with {employeeCount} employees invalid holiday owed");

        _scenarioContext.Set(affectedEmployees, AffectedEmployeesContextKey);
    }

    private Task BuildAndUploadAsync(
        Action<Rp14aFixtureBuilder>? configure,
        string description)
    {
        return UploadFileAsync(
            BuildTestFile(configure).FilePath,
            description);
    }

    private Rp14aTestFile BuildTestFile(
        Action<Rp14aFixtureBuilder>? configure = null)
    {
        Rp14aFixtureBuilder builder = new();

        configure?.Invoke(builder);

        return builder.Build(
            _testArtifacts,
            _baselineFilePath,
            ScenarioName);
    }

    private async Task UploadFileAsync(
        string filePath,
        string description)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                throw new InvalidOperationException(
                    $"RP14A test file was not created at expected location: {filePath}");
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

    private void SetComplexAffectedEmployees(
        string filePath,
        string? surname,
        string? basicPayPerWeek,
        string? holidayOwed)
    {
        XDocument document = XDocument.Load(filePath);
        Dictionary<string, List<AffectedEmployee>> affectedEmployeesByError = new();

        if (surname is not null)
        {
            affectedEmployeesByError[SurnameErrorKey] = ReadAffectedEmployees(document, surname);
        }

        if (basicPayPerWeek is not null)
        {
            affectedEmployeesByError[BasicPayErrorKey] = ReadAffectedEmployees(document, basicPayPerWeek);
        }

        if (holidayOwed is not null)
        {
            List<AffectedEmployee> holidayOwedEmployees = ReadAffectedEmployees(document, holidayOwed);
            affectedEmployeesByError[HolidayOwedErrorKey] = holidayOwedEmployees;
            affectedEmployeesByError[HolidayOwedRangeErrorKey] = holidayOwedEmployees;
        }

        _scenarioContext.Set(
            affectedEmployeesByError,
            AffectedEmployeesByErrorTypeKey);
    }

    private static List<AffectedEmployee> ReadAffectedEmployees(
        XDocument document,
        string? cellValue)
    {
        return Rp14aAffectedEmployeeReader.ReadAffectedEmployees(
            document,
            employeeCount: 1,
            cellValue: cellValue ?? string.Empty);
    }

    private static List<AffectedEmployee> CreateInvalidArrearsScenario(int count)
    {
        List<AffectedEmployee> affectedEmployees = [];

        for (int i = 1; i <= count; i++)
        {
            affectedEmployees.Add(new AffectedEmployee
            {
                Forename = ScenarioConstant.Forename,
                Surname = ScenarioConstant.Surname,
                DateOfBirth = FormatScenarioDob(),
                NiNumber = ScenarioConstant.NationalInsuranceNumber,
                CellValue = InvalidArrearsAmounts.All[(i - 1) % InvalidArrearsAmounts.All.Count]
            });
        }

        return affectedEmployees;
    }

    private static void ApplyIfNotNull(
        string? value,
        Action<string?> apply)
    {
        if (value is not null)
        {
            apply(value);
        }
    }

    private static string ValidateBaselineFilePath(string? baselineFilePath)
    {
        string effectivePath = baselineFilePath ?? DefaultBaselineFilePath;

        ArgumentException.ThrowIfNullOrWhiteSpace(effectivePath);

        string absolutePath = Path.IsPathRooted(effectivePath)
            ? effectivePath
            : Path.Join(AppContext.BaseDirectory, effectivePath);

        if (!File.Exists(absolutePath))
        {
            throw new FileNotFoundException(
                $"Baseline RP14A file not found at: {absolutePath}",
                absolutePath);
        }

        return absolutePath;
    }

    private static void ValidateDateOrder(
        DateOnly? startDate,
        DateOnly? endDate)
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

    private static string FormatDate(DateOnly? date) =>
        date?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? string.Empty;

    private static readonly string _scenarioDob =
        DateTime
            .ParseExact(ScenarioConstant.DOB, "yyyy-MM-dd", CultureInfo.InvariantCulture)
            .ToString("M/d/yyyy", CultureInfo.InvariantCulture);

    private static string FormatScenarioDob() => _scenarioDob;

    private static string ToXmlValue(string? value) =>
        value ?? string.Empty;

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

    private static void ValidatePositiveNumber(
        int value,
        string parameterName)
    {
        if (value <= 0)
        {
            throw new ArgumentException(
                $"Parameter '{parameterName}' must be greater than 0. Received: {value}",
                parameterName);
        }
    }

    private static void ValidateNonNegativeNumber(
        int value,
        string parameterName)
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

