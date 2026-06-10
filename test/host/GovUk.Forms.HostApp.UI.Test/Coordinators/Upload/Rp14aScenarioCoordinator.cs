using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Support;
using System.Globalization;
using System.Xml.Linq;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;

public sealed class Rp14aScenarioCoordinator : ScenarioCoordinatorBase, IRp14aScenarioCoordinator
{
    private readonly Func<IRp14aFixtureBuilder> _builderFactory;
    private const string AffectedEmployeesContextKey = "AffectedEmployees";
    private const string AffectedEmployeesByErrorTypeKey = "AffectedEmployeesByErrorType";
    private const string SurnameErrorKey = "Employee surname|1 missing employee surname";
    private const string NationalInsuranceNumberErrorKey = "Employee national insurance number|1 invalid employee national insurance number format";
    private const string MoneyOwedToEmployerErrorKey = "Money owed to employer|1 invalid money owed to employer";
    private const string BasicPayErrorKey = "Employee basic pay per week|1 invalid basic pay per week";
    private const string HolidayOwedErrorKey = "Holiday owed|1 invalid holiday owed";
    private const string HolidayOwedRangeErrorKey = "Holiday owed|1 invalid range of holiday owed";

    private static readonly string _formattedScenarioDob =
        DateTime
            .ParseExact(ScenarioConstant.DOB, "yyyy-MM-dd", CultureInfo.InvariantCulture)
            .ToString("M/d/yyyy", CultureInfo.InvariantCulture);

    public Rp14aScenarioCoordinator(
        IFileUploadCoordinator fileUploadCoordinator,
        ScenarioContext scenarioContext,
        TestArtifacts testArtifacts,
        Func<IRp14aFixtureBuilder> builderFactory)
        : base(
            fileUploadCoordinator,
            scenarioContext,
            testArtifacts,
            logTag: "RP14A")
    {
        _builderFactory = builderFactory;
    }

    public Task UploadValidRp14aAsync() =>
        BuildAndUploadAsync(
            configure: null,
            "valid RP14A file");

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

        ScenarioContext.Set(
            affectedEmployees,
            AffectedEmployeesContextKey);
    }

    public Task UploadRp14aWithHolidayNotPaidDatesAsync(
        DateOnly? startDate,
        DateOnly? endDate)
    {
        return BuildAndUploadAsync(
            builder => builder.WithHolidayNotPaidDates(startDate, endDate),
            $"RP14A with holiday not paid dates '{FormatDate(startDate)}' to '{FormatDate(endDate)}'");
    }

    public Task UploadRp14aWithNationalInsuranceNumberAsync(string? insuranceNumber, int occurrenceIndex)
    {
        ValidateNonNegativeNumber(occurrenceIndex, nameof(occurrenceIndex));

        return BuildAndUploadAsync(
            builder => builder
                .WithEmployeeIndex(occurrenceIndex)
                .WithCustomMutation(
                    RP14AElementNames.NINO,
                    ToXmlValue(insuranceNumber)),
            $"RP14A with NI number '{ToLogValue(insuranceNumber)}' at index {occurrenceIndex}");
    }

    public Task UploadRp14aWithMoneyOwedToEmployerAsync(string? moneyOwed) =>
        BuildAndUploadAsync(
            builder => builder.WithMoneyOwedToEmployer(moneyOwed),
            $"RP14A with money owed to employer '{ToLogValue(moneyOwed)}'");

    public Task UploadRp14aWithEmploymentDatesAsync(
        DateOnly? startDate,
        DateOnly? endDate)
    {
        return BuildAndUploadAsync(
            builder => builder.WithEmploymentDates(startDate, endDate),
            $"RP14A with employment dates {FormatDate(startDate)} to {FormatDate(endDate)}");
    }

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
        DateOnly? endDate)
    {

        return BuildAndUploadAsync(
            builder => builder.WithArrearsDates(startDate, endDate),
            $"RP14A with arrears dates '{FormatDate(startDate)}' to '{FormatDate(endDate)}'");
    }

    public async Task UploadComplexRp14aScenarioAsync(
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
     DateOnly? employmentEndDate = null)
    {

        Rp14aTestFile testFile = BuildTestFile(builder =>
        {
            ApplyIfNotNull(
                caseReference,
                value => builder.WithCaseReference(value));

            ApplyIfNotNull(
                employerName,
                value => builder.WithEmployerName(value));

            if (surname is not null || forename is not null || title is not null)
            {
                builder.WithEmployeeName(
                    surname,
                    forename,
                    title);
            }

            ApplyIfNotNull(
                nationalInsuranceNumber,
                value => builder.WithNationalInsuranceNumberForEmployees(1, value));

            ApplyIfNotNull(
                moneyOwedToEmployer,
                value => builder.WithMoneyOwedToEmployer(value));

            ApplyIfNotNull(
                arrearsAmount,
                value => builder.WithArrearsAmount(1, value));

            ApplyIfNotNull(
                basicPayPerWeek,
                value => builder.WithEmployeeBasicPayPerWeek(value));

            ApplyIfNotNull(
                holidayOwed,
                value => builder.WithHolidayOwed(value));

            if (employmentStartDate.HasValue || employmentEndDate.HasValue)
            {
                builder.WithEmploymentDates(
                    employmentStartDate,
                    employmentEndDate);
            }
        });

        SetComplexAffectedEmployees(
            testFile.FilePath,
            surname,
            nationalInsuranceNumber,
            moneyOwedToEmployer,
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
                cellValue: "Not entered");

        await UploadFileAsync(
            testFile.FilePath,
            $"RP14A with {employeeCount} employees missing surname");

        ScenarioContext.Set(affectedEmployees, AffectedEmployeesContextKey);
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

        ScenarioContext.Set(affectedEmployees, AffectedEmployeesContextKey);
    }

    public async Task UploadRp14aWithCaseReferenceAsync(params string?[] caseReferences)
    {
        if (caseReferences is null || caseReferences.Length == 0)
        {
            throw new ArgumentException(
                "At least one case reference must be provided.",
                nameof(caseReferences));
        }

        int employeeCount = caseReferences.Length;

        Rp14aTestFile testFile = BuildTestFile(
            builder => builder.WithCaseReferences(caseReferences));

        List<AffectedEmployee> affectedEmployees =
            Rp14aAffectedEmployeeReader.ReadAffectedEmployees(
                testFile.FilePath,
                employeeCount,
                cellValues: caseReferences
                    .Select(x => string.IsNullOrEmpty(x) ? "Not entered" : x)
                    .ToArray());

        await UploadFileAsync(
            testFile.FilePath,
            $"RP14A with {employeeCount} invalid case reference(s)");

        ScenarioContext.Set(
            affectedEmployees,
            AffectedEmployeesContextKey);
    }

    public async Task UploadRp14aWithTooLongCaseReferencesAsync(int count)
    {
        ValidatePositiveNumber(count, nameof(count));

        string[] caseReferences = Enumerable.Range(1, count)
            .Select(i => $"CN12345678{i:D3}")
            .ToArray();

        string[] ninos = Enumerable.Range(1, count)
            .Select(i => $"AB{i:D6}C")
            .ToArray();

        Rp14aTestFile testFile = BuildTestFile(builder =>
        {
            builder.WithCaseReferences(caseReferences);
            builder.WithNinosForEmployees(ninos);
        });

        List<AffectedEmployee> affectedEmployees =
            Rp14aAffectedEmployeeReader.ReadAffectedEmployees(
                testFile.FilePath,
                count,
                cellValues: caseReferences);

        await UploadFileAsync(
            testFile.FilePath,
            $"RP14A with {count} too-long case reference(s)");

        ScenarioContext.Set(affectedEmployees, AffectedEmployeesContextKey);
    }

    public async Task UploadRp14aWithEmployerNameAsync(params string?[] employerNames)
    {
        if (employerNames is null || employerNames.Length == 0)
        {
            throw new ArgumentException(
                "At least one employer name must be provided.",
                nameof(employerNames));
        }

        int employeeCount = employerNames.Length;

        Rp14aTestFile testFile = BuildTestFile(
            builder => builder.WithEmployerNames(employerNames));

        List<AffectedEmployee> affectedEmployees =
            Rp14aAffectedEmployeeReader.ReadAffectedEmployees(
                testFile.FilePath,
                employeeCount,
                cellValues: employerNames
                    .Select(x => x ?? string.Empty)
                    .ToArray());

        await UploadFileAsync(
            testFile.FilePath,
            $"RP14A with employer name(s) '{string.Join(", ", employerNames.Select(ToLogValue))}'");

        ScenarioContext.Set(
            affectedEmployees,
            AffectedEmployeesContextKey);
    }

    public async Task UploadRp14aWithNationalInsuranceNumberForEmployeesAsync(int employeeCount, string? nationalInsuranceNumber)
    {
        ValidatePositiveNumber(employeeCount, nameof(employeeCount));

        Rp14aTestFile testFile = BuildTestFile(builder =>
            builder.WithNationalInsuranceNumberForEmployees(
                employeeCount,
                nationalInsuranceNumber));

        List<AffectedEmployee> affectedEmployees =
            Rp14aAffectedEmployeeReader.ReadAffectedEmployees(
                testFile.FilePath,
                employeeCount,
                cellValue: string.IsNullOrEmpty(nationalInsuranceNumber) ? "Not entered" : nationalInsuranceNumber);

        await UploadFileAsync(
            testFile.FilePath,
            $"RP14A with {employeeCount} employees having NI number '{ToLogValue(nationalInsuranceNumber)}'");

        ScenarioContext.Set(
            affectedEmployees,
            AffectedEmployeesContextKey);
    }

    public async Task UploadRp14aWithMoneyOwedToEmployerForEmployeesAsync(int employeeCount, string? moneyOwed)
    {
        ValidatePositiveNumber(employeeCount, nameof(employeeCount));

        Rp14aTestFile testFile = BuildTestFile(builder =>
            builder.WithMoneyOwedToEmployerForEmployees(employeeCount, moneyOwed));

        List<AffectedEmployee> affectedEmployees =
            Rp14aAffectedEmployeeReader.ReadAffectedEmployees(
                testFile.FilePath,
                employeeCount,
                cellValue: moneyOwed ?? string.Empty);

        await UploadFileAsync(
            testFile.FilePath,
            $"RP14A with {employeeCount} employees having money owed to employer '{ToLogValue(moneyOwed)}'");

        ScenarioContext.Set(
            affectedEmployees,
            AffectedEmployeesContextKey);
    }

    public async Task UploadRp14aWithEmployeeBasicPayPerWeekForEmployeesAsync(int employeeCount, string? basicPayPerWeek)
    {
        ValidatePositiveNumber(employeeCount, nameof(employeeCount));

        Rp14aTestFile testFile = BuildTestFile(builder =>
            builder.WithEmployeeBasicPayPerWeekForEmployees(
                employeeCount,
                basicPayPerWeek));

        List<AffectedEmployee> affectedEmployees =
            Rp14aAffectedEmployeeReader.ReadAffectedEmployees(
                testFile.FilePath,
                employeeCount,
                cellValue: basicPayPerWeek ?? string.Empty);

        await UploadFileAsync(
            testFile.FilePath,
            $"RP14A with {employeeCount} employees having basic pay per week '{ToLogValue(basicPayPerWeek)}'");

        ScenarioContext.Set(
            affectedEmployees,
            AffectedEmployeesContextKey);
    }

    public async Task UploadRp14aWithHolidayDaysTakenForEmployeesAsync(int employeeCount, string? holidayDaysTaken)
    {
        ValidatePositiveNumber(employeeCount, nameof(employeeCount));

        Rp14aTestFile testFile = BuildTestFile(builder =>
            builder.WithHolidayDaysTakenForEmployees(employeeCount, holidayDaysTaken));

        List<AffectedEmployee> affectedEmployees =
            Rp14aAffectedEmployeeReader.ReadAffectedEmployees(
                testFile.FilePath,
                employeeCount,
                cellValue: holidayDaysTaken ?? string.Empty);

        await UploadFileAsync(
            testFile.FilePath,
            $"RP14A with {employeeCount} employees having invalid holiday days taken '{ToLogValue(holidayDaysTaken)}'");

        ScenarioContext.Set(affectedEmployees, AffectedEmployeesContextKey);
    }

    public async Task UploadRp14aWithHolidayDaysCarriedForwardForEmployeesAsync(int employeeCount, string? holidayDaysCarriedForward)
    {
        ValidatePositiveNumber(employeeCount, nameof(employeeCount));

        Rp14aTestFile testFile = BuildTestFile(builder =>
            builder.WithHolidayDaysCarriedForwardForEmployees(
                employeeCount,
                holidayDaysCarriedForward));

        List<AffectedEmployee> affectedEmployees =
            Rp14aAffectedEmployeeReader.ReadAffectedEmployees(
                testFile.FilePath,
                employeeCount,
                cellValue: holidayDaysCarriedForward ?? string.Empty);

        await UploadFileAsync(
            testFile.FilePath,
            $"RP14A with {employeeCount} employees having holiday days carried forward '{ToLogValue(holidayDaysCarriedForward)}'");

        ScenarioContext.Set(
            affectedEmployees,
            AffectedEmployeesContextKey);
    }

    public async Task UploadRp14aWithHolidayContractedEntitlementDaysForEmployeesAsync(int employeeCount, string? value)
    {
        ValidatePositiveNumber(employeeCount, nameof(employeeCount));

        Rp14aTestFile testFile = BuildTestFile(builder =>
            builder.WithHolidayContractedEntitlementDaysForEmployees(employeeCount, value));

        List<AffectedEmployee> affectedEmployees =
            Rp14aAffectedEmployeeReader.ReadAffectedEmployees(
                testFile.FilePath,
                employeeCount,
                cellValue: value ?? string.Empty);

        await UploadFileAsync(
            testFile.FilePath,
            $"RP14A with {employeeCount} employees having contracted holiday entitlement '{ToLogValue(value)}'");

        ScenarioContext.Set(affectedEmployees, AffectedEmployeesContextKey);
    }

    public async Task UploadRp14aWithHolidayNotPaidDatesForEmployeesAsync(int employeeCount, DateOnly? startDate, DateOnly? endDate)
    {
        ValidatePositiveNumber(employeeCount, nameof(employeeCount));

        Rp14aTestFile testFile = BuildTestFile(builder =>
            builder.WithHolidayNotPaidDatesForEmployees(employeeCount, startDate, endDate));

        string cellValue = FormatUiDateRange(startDate, endDate);

        List<AffectedEmployee> affectedEmployees =
            Rp14aAffectedEmployeeReader.ReadAffectedEmployees(
                testFile.FilePath,
                employeeCount,
                cellValue: cellValue);

        await UploadFileAsync(
            testFile.FilePath,
            $"RP14A with {employeeCount} employees having holiday not paid start date after end date");

        ScenarioContext.Set(affectedEmployees, AffectedEmployeesContextKey);
    }

    public async Task UploadRp14aWithSurnameForEmployeesAsync(int employeeCount, string? surname)
    {
        ValidatePositiveNumber(employeeCount, nameof(employeeCount));

        Rp14aTestFile testFile = BuildTestFile(builder =>
            builder.WithSurnamesForEmployees(employeeCount, surname));

        List<AffectedEmployee> affectedEmployees =
            Rp14aAffectedEmployeeReader.ReadAffectedEmployees(
                testFile.FilePath,
                employeeCount,
                cellValue: surname ?? string.Empty);

        await UploadFileAsync(
            testFile.FilePath,
            $"RP14A with {employeeCount} employees having surname '{ToLogValue(surname)}'");

        ScenarioContext.Set(affectedEmployees, AffectedEmployeesContextKey);
    }

    public async Task UploadRp14aWithEmploymentDatesForEmployeesAsync(int employeeCount, DateOnly? startDate, DateOnly? endDate)
    {
        ValidatePositiveNumber(employeeCount, nameof(employeeCount));

        Rp14aTestFile testFile = BuildTestFile(builder =>
            builder.WithEmploymentDatesForEmployees(employeeCount, startDate, endDate));

        string cellValue = FormatUiDateRange(startDate, endDate);

        List<AffectedEmployee> affectedEmployees =
            Rp14aAffectedEmployeeReader.ReadAffectedEmployees(
                testFile.FilePath,
                employeeCount,
                cellValue: cellValue);

        await UploadFileAsync(
            testFile.FilePath,
            $"RP14A with {employeeCount} employees having employment start date after end date");

        ScenarioContext.Set(affectedEmployees, AffectedEmployeesContextKey);
    }

    public async Task UploadRp14aWithArrearsDatesForEmployeesAsync(int employeeCount, DateOnly? startDate, DateOnly? endDate)
    {
        ValidatePositiveNumber(employeeCount, nameof(employeeCount));

        Rp14aTestFile testFile = BuildTestFile(builder =>
            builder.WithArrearsDatesForEmployees(employeeCount, startDate, endDate));

        string cellValue = FormatUiDateRange(startDate, endDate);

        List<AffectedEmployee> affectedEmployees =
            Rp14aAffectedEmployeeReader.ReadAffectedEmployees(
                testFile.FilePath,
                employeeCount,
                cellValue: cellValue);

        await UploadFileAsync(
            testFile.FilePath,
            $"RP14A with {employeeCount} employees having arrears start date after end date");

        ScenarioContext.Set(affectedEmployees, AffectedEmployeesContextKey);
    }

    private Task BuildAndUploadAsync(
        Action<IRp14aFixtureBuilder>? configure,
        string description)
    {
        Rp14aTestFile testFile = BuildTestFile(configure);

        return UploadFileAsync(testFile.FilePath, description);
    }

    private Rp14aTestFile BuildTestFile(Action<IRp14aFixtureBuilder>? configure = null)
    {
        IRp14aFixtureBuilder builder = _builderFactory();

        configure?.Invoke(builder);

        return builder.Build(TestArtifacts, scenarioName: ScenarioName);
    }

    private void SetComplexAffectedEmployees(
        string filePath,
        string? surname,
        string? nationalInsuranceNumber,
        string? moneyOwedToEmployer,
        string? basicPayPerWeek,
        string? holidayOwed)
    {
        XDocument document = XDocument.Load(filePath);
        Dictionary<string, List<AffectedEmployee>> affectedEmployeesByError = [];

        if (surname is not null)
        {
            affectedEmployeesByError[SurnameErrorKey] = ReadAffectedEmployees(document, surname);
        }

        if (nationalInsuranceNumber is not null)
        {
            affectedEmployeesByError[NationalInsuranceNumberErrorKey] = ReadAffectedEmployees(document, nationalInsuranceNumber);
        }

        if (moneyOwedToEmployer is not null)
        {
            affectedEmployeesByError[MoneyOwedToEmployerErrorKey] = ReadAffectedEmployees(document, moneyOwedToEmployer);
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

        ScenarioContext.Set(affectedEmployeesByError, AffectedEmployeesByErrorTypeKey);
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
                DateOfBirth = _formattedScenarioDob,
                NiNumber = ScenarioConstant.NationalInsuranceNumber,
                CellValue = InvalidArrearsAmounts.All[(i - 1) % InvalidArrearsAmounts.All.Count]
            });
        }

        return affectedEmployees;
    }

    private static void ApplyIfNotNull(string? value, Action<string> apply)
    {
        if (value is not null)
        {
            apply(value);
        }
    }

    private static string ToXmlValue(string? value) =>
        value ?? string.Empty;
}
