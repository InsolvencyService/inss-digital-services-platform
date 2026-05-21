using GovUk.Forms.HostApp.UI.Test.Factories;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Support;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Xml.Linq;

namespace GovUk.Forms.HostApp.UI.Test.Builders;

public class Rp14aFixtureBuilder
{
    private readonly Dictionary<string, string?> _mutations = [];
    private readonly Dictionary<int, Dictionary<string, string?>> _employeeMutations = [];

    private int _targetEmployeeIndex;
    private string? _baselineFilePath;
    private TestArtifacts? _testArtifacts;
    private string? _scenarioName = "Default";

    public Rp14aFixtureBuilder WithEmployeeIndex(int index)
    {
        if (index < 0)
        {
            throw new ArgumentException(
                "Employee index must be non-negative",
                nameof(index));
        }

        _targetEmployeeIndex = index;
        return this;
    }

    public Rp14aFixtureBuilder WithCaseReference(string? caseReference)
    {
        _mutations[RP14AElementNames.CaseReference] = caseReference ?? string.Empty;
        return this;
    }

    public Rp14aFixtureBuilder WithEmployerName(string? employerName)
    {
        _mutations[RP14AElementNames.EmployerName] = employerName ?? string.Empty;
        return this;
    }

    public Rp14aFixtureBuilder WithEmployeeName(
        string? surname,
        string? forename,
        string? title)
    {
        _mutations[RP14AElementNames.Surname] = surname ?? string.Empty;
        _mutations[RP14AElementNames.Forenames] = forename ?? string.Empty;
        _mutations[RP14AElementNames.Title] = title ?? string.Empty;

        return this;
    }

    public Rp14aFixtureBuilder WithEmployeeBasicPayPerWeek(string? basicPayPerWeek)
    {
        _mutations[RP14AElementNames.BasicPayPerWeek] =
            basicPayPerWeek ?? string.Empty;

        return this;
    }

    public Rp14aFixtureBuilder WithHolidayContractedEntitlementDays(string? entitlementDays)
    {
        _mutations[RP14AElementNames.HolidayContractedEntitlementDays] =
            entitlementDays;

        return this;
    }

    public Rp14aFixtureBuilder WithMoneyOwedToEmployer(string? moneyOwed)
    {
        _mutations[RP14AElementNames.MoneyOwedToEmployer] =
            moneyOwed ?? string.Empty;

        return this;
    }

    public Rp14aFixtureBuilder WithEmploymentDates(DateOnly? startDate, DateOnly? endDate)
    {
        _mutations[RP14AElementNames.StartDate] = FormatDate(startDate);
        _mutations[RP14AElementNames.EndDate] = FormatDate(endDate);

        return this;
    }

    public Rp14aFixtureBuilder WithArrearsDates(DateOnly? startDate, DateOnly? endDate)
    {
        _mutations[RP14AElementNames.AOP1StartDate] = FormatDate(startDate);
        _mutations[RP14AElementNames.AOP1EndDate] = FormatDate(endDate);
        return this;
    }

    public Rp14aFixtureBuilder WithInvalidHolidayOwedForEmployees(int employeeCount, params string[] invalidValues)
    {
        ValidatePositiveNumber(employeeCount, nameof(employeeCount));

        if (invalidValues.Length == 0)
        {
            throw new ArgumentException(
                "At least one invalid value must be provided.",
                nameof(invalidValues));
        }

        for (int employeeIndex = 0; employeeIndex < employeeCount; employeeIndex++)
        {
            AddEmployeeMutation(
                employeeIndex,
                RP14AElementNames.NoDaysHolidayOwed,
                invalidValues[employeeIndex % invalidValues.Length]);
        }

        return this;
    }
    public Rp14aFixtureBuilder WithArrearsAmount(int periodNumber, string? amountOwed)
    {
        ValidatePositiveNumber(periodNumber, nameof(periodNumber));

        _mutations[$"{RP14AElementNames.AOPOwed}{periodNumber}"] =
            amountOwed ?? string.Empty;

        return this;
    }
    public Rp14aFixtureBuilder WithMissingSurnames(int employeeCount)
    {
        ValidatePositiveNumber(employeeCount, nameof(employeeCount));

        for (int index = 0; index < employeeCount; index++)
        {
            AddEmployeeMutation(
                index,
                RP14AElementNames.Surname,
                string.Empty);
        }

        return this;
    }

    public Rp14aFixtureBuilder WithCustomMutation(string elementName, string? value)
    {
        if (string.IsNullOrWhiteSpace(elementName))
        {
            throw new ArgumentException(
                "Element name cannot be empty",
                nameof(elementName));
        }

        _mutations[elementName] = value;

        return this;
    }
    public Rp14aFixtureBuilder WithHolidayDaysCarriedForward(string? daysCarriedForward)
    {
        _mutations[RP14AElementNames.HolidayDaysCarriedForward] =
            daysCarriedForward;

        return this;
    }

    public Rp14aFixtureBuilder WithHolidayDaysTaken(string? holidayDaysTaken)
    {
        _mutations[RP14AElementNames.HolidayDaysTaken] =
            holidayDaysTaken;

        return this;
    }

    public Rp14aFixtureBuilder WithHolidayOwed(string? holidayOwed)
    {
        _mutations[RP14AElementNames.NoDaysHolidayOwed] =
            holidayOwed;

        return this;
    }

    public Rp14aFixtureBuilder WithHolidayNotPaidDates(DateOnly? startDate, DateOnly? endDate)
    {
        _mutations[RP14AElementNames.Holiday1StartDate] =
            FormatDate(startDate);

        _mutations[RP14AElementNames.Holiday1EndDate] =
            FormatDate(endDate);

        return this;
    }
    public Rp14aFixture Build(
        TestArtifacts testArtifacts,
        string baselineFilePath,
        string scenarioName = "Default")
    {
        _testArtifacts = testArtifacts
            ?? throw new ArgumentNullException(nameof(testArtifacts));

        _baselineFilePath = baselineFilePath;
        _scenarioName = scenarioName;

        if (_mutations.Count == 0 && _employeeMutations.Count == 0)
        {
            LogInfo($"Scenario '{scenarioName}': No mutations specified, creating clean copy");
            return CreateValidCopy();
        }

        LogInfo($"Scenario '{scenarioName}': Building fixture with {_mutations.Count} mutations");

        try
        {
            string filePath = CreateAndMutateFile();

            Dictionary<string, string> appliedMutations = _mutations
                .Where(mutation => mutation.Value is not null)
                .ToDictionary(
                    mutation => mutation.Key,
                    mutation => mutation.Value!);

            Rp14aFixture fixture = new(
                filePath,
                appliedMutations,
                _targetEmployeeIndex,
                DateTime.UtcNow);

            LogInfo($"Scenario '{scenarioName}': Fixture created successfully at {filePath}");
            return fixture;
        }
        catch (Exception ex)
        {
            LogError($"Scenario '{scenarioName}': Failed to create fixture - {ex.Message}");
            throw;
        }
    }

    private Rp14aFixture CreateValidCopy()
    {
        ArgumentNullException.ThrowIfNull(_testArtifacts);
        ArgumentNullException.ThrowIfNull(_baselineFilePath);

        string absolutePath = ResolveBaselinePath(_baselineFilePath);
        string uniqueFileName = $"rp14a-{Guid.NewGuid():N}.xml";
        string targetPath = _testArtifacts.FilePath(uniqueFileName);

        try
        {
            File.Copy(absolutePath, targetPath, overwrite: false);

            LogInfo($"Created clean copy at {targetPath}");

            return new Rp14aFixture(
                targetPath,
                new Dictionary<string, string>(),
                _targetEmployeeIndex,
                DateTime.UtcNow);
        }
        catch (IOException ex) when (ex.Message.Contains(
                   "already exists",
                   StringComparison.OrdinalIgnoreCase))
        {
            LogWarning($"File already exists due to race condition: {targetPath}. Retrying...");
            return CreateValidCopy();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to create RP14A fixture copy: {ex.Message}",
                ex);
        }
    }

    private string CreateAndMutateFile()
    {
        ArgumentNullException.ThrowIfNull(_testArtifacts);
        ArgumentNullException.ThrowIfNull(_baselineFilePath);

        Rp14aFixture cleanFixture = CreateValidCopy();

        XDocument document = LoadXmlDocument(cleanFixture.FilePath);
        XNamespace ns = ExtractNamespace(document);

        XElement targetEmployee = GetEmployeeByIndex(
            document,
            ns,
            _targetEmployeeIndex);

        foreach (KeyValuePair<string, string?> mutation in _mutations)
        {
            ApplyMutation(
                document,
                ns,
                targetEmployee,
                mutation.Key,
                mutation.Value);
        }

        foreach (KeyValuePair<int, Dictionary<string, string?>> employeeMutation in _employeeMutations)
        {
            XElement employee = GetEmployeeByIndex(
                document,
                ns,
                employeeMutation.Key);

            foreach (KeyValuePair<string, string?> mutation in employeeMutation.Value)
            {
                ApplyMutation(
                    document,
                    ns,
                    employee,
                    mutation.Key,
                    mutation.Value);
            }
        }

        SaveXmlDocument(document, cleanFixture.FilePath);

        return cleanFixture.FilePath;
    }

    private void ApplyMutation(
        XDocument document,
        XNamespace ns,
        XElement targetEmployee,
        string elementName,
        string? value)
    {
        XElement? element = targetEmployee
            .Descendants(ns + elementName)
            .FirstOrDefault();

        element ??= document
            .Descendants(ns + elementName)
            .FirstOrDefault();

        if (element is null)
        {
            throw new InvalidOperationException(
                $"Element '{elementName}' not found in RP14A XML for employee index {_targetEmployeeIndex}");
        }

        if (value is null)
        {
            LogInfo($"Removing element: {elementName}");
            element.Remove();
            return;
        }

        element.Value = value;

        if (element.Value != value)
        {
            throw new InvalidOperationException(
                $"Mutation of '{elementName}' failed. Expected '{value}', but found '{element.Value}'.");
        }

        LogInfo($"Applied mutation: {elementName} = '{value}'");
    }

    private static XElement GetEmployeeByIndex(
        XDocument document,
        XNamespace ns,
        int index)
    {
        List<XElement> employees = document
            .Descendants(ns + RP14AElementNames.Employee)
            .ToList();

        if (employees.Count == 0)
        {
            throw new InvalidOperationException(
                "No Employee elements found in RP14A XML.");
        }

        if (index < 0 || index >= employees.Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(index),
                $"Employee index {index} is out of range. Found {employees.Count} employees.");
        }

        return employees[index];
    }

    private static XNamespace ExtractNamespace(XDocument document)
    {
        XElement root = document.Root
            ?? throw new InvalidOperationException("XML document is empty.");

        return root.Name.NamespaceName;
    }

    private static XDocument LoadXmlDocument(string filePath)
    {
        try
        {
            return XDocument.Load(
                filePath,
                LoadOptions.PreserveWhitespace);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to load XML from '{filePath}': {ex.Message}",
                ex);
        }
    }

    private static void SaveXmlDocument(XDocument document, string filePath)
    {
        try
        {
            using StreamWriter writer = new(
                filePath,
                append: false,
                encoding: new UTF8Encoding(
                    encoderShouldEmitUTF8Identifier: false));

            document.Save(
                writer,
                SaveOptions.DisableFormatting);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to save XML to '{filePath}': {ex.Message}",
                ex);
        }
    }

    private static string ResolveBaselinePath(string baselineFilePath)
    {
        if (string.IsNullOrWhiteSpace(baselineFilePath))
        {
            throw new ArgumentException(
                "Baseline RP14A file path cannot be empty.",
                nameof(baselineFilePath));
        }

        string absolutePath = Path.IsPathRooted(baselineFilePath)
            ? baselineFilePath
            : Path.Combine(AppContext.BaseDirectory, baselineFilePath);

        if (!File.Exists(absolutePath))
        {
            throw new FileNotFoundException(
                $"RP14A baseline XML file not found: {absolutePath}",
                absolutePath);
        }

        return absolutePath;
    }

    private static string FormatDate(DateOnly? date)
    {
        return date?.ToString(
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture) ?? string.Empty;
    }

    private static void ValidatePositiveNumber(int value, string parameterName)
    {
        if (value <= 0)
        {
            throw new ArgumentException(
                $"'{parameterName}' must be greater than 0. Received: {value}",
                parameterName);
        }
    }

    private static void LogInfo(string message)
    {
        LogMessage("INFO", message);
    }

    private static void LogWarning(string message)
    {
        LogMessage("WARN", message);
    }

    private static void LogError(string message)
    {
        LogMessage("ERROR", message);
    }

    private static void LogMessage(string level, string message)
    {
        string timestamp = DateTime.UtcNow.ToString(
            "yyyy-MM-dd HH:mm:ss.fff",
            CultureInfo.InvariantCulture);

        Debug.WriteLine($"[{timestamp}] [{level}] [RP14A] {message}");
    }

    private void AddEmployeeMutation(int employeeIndex, string elementName, string? value)
    {
        if (employeeIndex < 0)
        {
            throw new ArgumentException(
                "Employee index must be non-negative.",
                nameof(employeeIndex));
        }

        if (!_employeeMutations.TryGetValue(employeeIndex, out Dictionary<string, string?>? mutations))
        {
            mutations = [];
            _employeeMutations[employeeIndex] = mutations;
        }

        mutations[elementName] = value;
    }
}

