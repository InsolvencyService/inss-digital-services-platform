using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Support;
using System.Diagnostics;
using System.Globalization;
using System.Security;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace GovUk.Forms.HostApp.UI.Test.Builders;

public class Rp14aFixtureBuilder
{
    private readonly Dictionary<string, string?> _mutations = [];
    private readonly Dictionary<int, Dictionary<string, string?>> _employeeMutations = [];

    private int _targetEmployeeIndex;

    public Rp14aFixtureBuilder WithEmployeeIndex(int index)
    {
        if (index < 0)
        {
            throw new ArgumentException("Employee index must be non-negative.", nameof(index));
        }

        _targetEmployeeIndex = index;
        return this;
    }

    public Rp14aFixtureBuilder WithCaseReference(string? value) =>
        Set(RP14AElementNames.CaseReference, value);

    public Rp14aFixtureBuilder WithEmployerName(string? value) =>
        Set(RP14AElementNames.EmployerName, value);

    public Rp14aFixtureBuilder WithEmployeeBasicPayPerWeek(string? value) =>
        Set(RP14AElementNames.BasicPayPerWeek, value);

    public Rp14aFixtureBuilder WithMoneyOwedToEmployer(string? value) =>
        Set(RP14AElementNames.MoneyOwedToEmployer, value);

    public Rp14aFixtureBuilder WithHolidayContractedEntitlementDays(string? value) =>
        Set(RP14AElementNames.HolidayContractedEntitlementDays, value, nullMeansEmpty: false);

    public Rp14aFixtureBuilder WithHolidayDaysCarriedForward(string? value) =>
        Set(RP14AElementNames.HolidayDaysCarriedForward, value, nullMeansEmpty: false);

    public Rp14aFixtureBuilder WithHolidayDaysTaken(string? value) =>
        Set(RP14AElementNames.HolidayDaysTaken, value, nullMeansEmpty: false);

    public Rp14aFixtureBuilder WithHolidayOwed(string? value) =>
        Set(RP14AElementNames.NoDaysHolidayOwed, value, nullMeansEmpty: false);

    public Rp14aFixtureBuilder WithEmployeeName(
        string? surname,
        string? forename,
        string? title)
    {
        return Set(RP14AElementNames.Surname, surname)
            .Set(RP14AElementNames.Forenames, forename)
            .Set(RP14AElementNames.Title, title);
    }

    public Rp14aFixtureBuilder WithEmploymentDates(DateOnly? startDate, DateOnly? endDate)
    {
        return Set(RP14AElementNames.StartDate, FormatDate(startDate), nullMeansEmpty: false)
            .Set(RP14AElementNames.EndDate, FormatDate(endDate), nullMeansEmpty: false);
    }

    public Rp14aFixtureBuilder WithArrearsDates(DateOnly? startDate, DateOnly? endDate)
    {
        return Set(RP14AElementNames.AOP1StartDate, FormatDate(startDate), nullMeansEmpty: false)
            .Set(RP14AElementNames.AOP1EndDate, FormatDate(endDate), nullMeansEmpty: false);
    }

    public Rp14aFixtureBuilder WithHolidayNotPaidDates(DateOnly? startDate, DateOnly? endDate)
    {
        return Set(RP14AElementNames.Holiday1StartDate, FormatDate(startDate), nullMeansEmpty: false)
            .Set(RP14AElementNames.Holiday1EndDate, FormatDate(endDate), nullMeansEmpty: false);
    }

    public Rp14aFixtureBuilder WithArrearsAmount(int periodNumber, string? amountOwed)
    {
        ValidatePositiveNumber(periodNumber, nameof(periodNumber));

        return Set(
            $"{RP14AElementNames.AOPOwed}{periodNumber}",
            amountOwed);
    }

    public Rp14aFixtureBuilder WithInvalidHolidayOwedForEmployees(
        int employeeCount,
        params string[] invalidValues)
    {
        ValidatePositiveNumber(employeeCount, nameof(employeeCount));

        if (invalidValues.Length == 0)
        {
            throw new ArgumentException("At least one invalid value must be provided.", nameof(invalidValues));
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
        ArgumentException.ThrowIfNullOrWhiteSpace(elementName);

        _mutations[elementName] = value;
        return this;
    }

    public Rp14aTestFile Build(
        TestArtifacts testArtifacts,
        string baselineFilePath,
        string scenarioName = "Default")
    {
        ArgumentNullException.ThrowIfNull(testArtifacts);
        ArgumentException.ThrowIfNullOrWhiteSpace(baselineFilePath);

        if (_mutations.Count == 0 && _employeeMutations.Count == 0)
        {
            LogInfo($"Scenario '{scenarioName}': No mutations specified, creating clean copy.");

            string cleanFilePath = CreateValidCopy(testArtifacts, baselineFilePath);

            return CreateFixture(
                cleanFilePath,
                []);
        }

        LogInfo($"Scenario '{scenarioName}': Building fixture with {_mutations.Count} global mutations.");

        try
        {
            string filePath = CreateAndMutateFile(testArtifacts, baselineFilePath);

            Dictionary<string, string> appliedMutations = _mutations
                .Where(mutation => mutation.Value is not null)
                .ToDictionary(
                    mutation => mutation.Key,
                    mutation => mutation.Value!);

            LogInfo($"Scenario '{scenarioName}': Fixture created successfully at {filePath}");

            return CreateFixture(filePath, appliedMutations);
        }
        catch (Exception ex)
        {
            LogError($"Scenario '{scenarioName}': Failed to create fixture - {ex.Message}");
            throw;
        }
    }

    private Rp14aFixtureBuilder Set(
        string elementName,
        string? value,
        bool nullMeansEmpty = true)
    {
        _mutations[elementName] = nullMeansEmpty
            ? value ?? string.Empty
            : value;

        return this;
    }

    private static Rp14aTestFile CreateFixture(
        string filePath,
        Dictionary<string, string> appliedMutations,
        int targetEmployeeIndex = 0)
    {
        return new Rp14aTestFile(
            filePath,
            appliedMutations,
            targetEmployeeIndex,
            DateTime.UtcNow);
    }

    private string CreateAndMutateFile(
        TestArtifacts testArtifacts,
        string baselineFilePath)
    {
        string filePath = CreateValidCopy(testArtifacts, baselineFilePath);

        XDocument document = LoadXmlDocument(filePath);
        XNamespace ns = ExtractNamespace(document);

        XElement targetEmployee = GetEmployeeByIndex(
            document,
            ns,
            _targetEmployeeIndex);

        foreach ((string elementName, string? value) in _mutations)
        {
            ApplyMutation(
                document,
                ns,
                targetEmployee,
                elementName,
                value);
        }

        foreach ((int employeeIndex, Dictionary<string, string?> mutations) in _employeeMutations)
        {
            XElement employee = GetEmployeeByIndex(
                document,
                ns,
                employeeIndex);

            foreach ((string elementName, string? value) in mutations)
            {
                ApplyMutation(
                    document,
                    ns,
                    employee,
                    elementName,
                    value);
            }
        }

        SaveXmlDocument(document, filePath);

        return filePath;
    }

    private static string CreateValidCopy(
        TestArtifacts testArtifacts,
        string baselineFilePath)
    {
        string absolutePath = ResolveBaselinePath(baselineFilePath);
        string uniqueFileName = $"rp14a-{Guid.NewGuid():N}.xml";
        string targetPath = testArtifacts.FilePath(uniqueFileName);

        try
        {
            File.Copy(
                absolutePath,
                targetPath,
                overwrite: false);

            LogInfo($"Created clean copy at {targetPath}");

            return targetPath;
        }
        catch (IOException ex) when (ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
        {
            LogWarning($"File already exists due to race condition: {targetPath}. Retrying.");
            return CreateValidCopy(testArtifacts, baselineFilePath);
        }
        catch (Exception ex) when (IsFileException(ex))
        {
            throw new InvalidOperationException(
                $"Failed to create RP14A fixture copy: {ex.Message}",
                ex);
        }
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
                $"Element '{elementName}' not found in RP14A XML for employee index {_targetEmployeeIndex}.");
        }

        if (value is null)
        {
            LogInfo($"Removing element: {elementName}");
            element.Remove();
            return;
        }

        element.Value = value;
        LogInfo($"Applied mutation: {elementName} = '{value}'");
    }

    private void AddEmployeeMutation(
        int employeeIndex,
        string elementName,
        string? value)
    {
        if (employeeIndex < 0)
        {
            throw new ArgumentException("Employee index must be non-negative.", nameof(employeeIndex));
        }

        if (!_employeeMutations.TryGetValue(employeeIndex, out Dictionary<string, string?>? mutations))
        {
            mutations = [];
            _employeeMutations[employeeIndex] = mutations;
        }

        mutations[elementName] = value;
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
            throw new InvalidOperationException("No Employee elements found in RP14A XML.");
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
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        try
        {
            return XDocument.Load(
                filePath,
                LoadOptions.PreserveWhitespace);
        }
        catch (Exception ex) when (IsFileException(ex) || ex is XmlException)
        {
            throw new InvalidOperationException(
                $"Failed to load XML from '{filePath}': {ex.Message}",
                ex);
        }
    }

    private static void SaveXmlDocument(
        XDocument document,
        string filePath)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        try
        {
            using StreamWriter writer = new(
                filePath,
                append: false,
                encoding: new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

            document.Save(
                writer,
                SaveOptions.DisableFormatting);
        }
        catch (Exception ex) when (IsFileException(ex))
        {
            throw new InvalidOperationException(
                $"Failed to save XML to '{filePath}': {ex.Message}",
                ex);
        }
    }

    private static string ResolveBaselinePath(string baselineFilePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(baselineFilePath);

        string absolutePath = Path.GetFullPath(
            baselineFilePath,
            AppContext.BaseDirectory);

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

    private static bool IsFileException(Exception ex)
    {
        return ex is IOException
            or UnauthorizedAccessException
            or DirectoryNotFoundException
            or PathTooLongException
            or NotSupportedException
            or SecurityException;
    }

    private static void LogInfo(string message) =>
        LogMessage("INFO", message);

    private static void LogWarning(string message) =>
        LogMessage("WARN", message);

    private static void LogError(string message) =>
        LogMessage("ERROR", message);

    private static void LogMessage(string level, string message)
    {
        string timestamp = DateTime.UtcNow.ToString(
            "yyyy-MM-dd HH:mm:ss.fff",
            CultureInfo.InvariantCulture);

        Debug.WriteLine($"[{timestamp}] [{level}] [RP14A] {message}");
    }
}

