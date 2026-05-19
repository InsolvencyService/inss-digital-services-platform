using GovUk.Forms.HostApp.UI.Test.Helpers;
using System.Globalization;
using System.Xml.Linq;

namespace GovUk.Forms.HostApp.UI.Test.Factories;

public static class Rp14aXmlFixtureFactory
{
    private const string NamespaceUri =
        "http://www.ins.gsi.gov.uk/FileUpload/RP14A_Application";

    private static readonly XNamespace _ns = NamespaceUri;
    public static string CreateValidCopy(
     TestArtifacts testArtifacts,
     string baselineFilePath,
     string scenarioName)
    {
        ArgumentNullException.ThrowIfNull(testArtifacts);

        string absolutePath = ResolveBaselinePath(baselineFilePath);
        string uniqueFileName = $"rp14a-{Guid.NewGuid():N}.xml";
        string targetPath = testArtifacts.FilePath(uniqueFileName);

        File.Copy(absolutePath, targetPath, overwrite: false);

        return targetPath;
    }

    public static string CreateWithCaseReference(
        TestArtifacts testArtifacts,
        string baselineFilePath,
        string? caseReference,
        string scenarioName)
    {
        return CreateWithElementValue(
            testArtifacts,
            baselineFilePath,
            "CaseReference",
            caseReference ?? string.Empty,
            scenarioName);
    }

    public static string CreateWithEmployerName(
        TestArtifacts testArtifacts,
        string baselineFilePath,
        string? employerName,
        string scenarioName)
    {
        return CreateWithElementValue(
            testArtifacts,
            baselineFilePath,
            "EmployerName",
            employerName ?? string.Empty,
            scenarioName);
    }

    public static string CreateWithEmployeeName(
        TestArtifacts testArtifacts,
        string baselineFilePath,
        string? surname,
        string? forename,
        string? title,
        string scenarioName)
    {
        Dictionary<string, string> mutations = new()
        {
            ["Surname"] = surname ?? string.Empty,
            ["Forenames"] = forename ?? string.Empty,
            ["Title"] = title ?? string.Empty
        };

        return CreateWithElementValues(
            testArtifacts,
            baselineFilePath,
            mutations,
            scenarioName);
    }

    public static string CreateWithNationalInsuranceNumber(
   TestArtifacts testArtifacts,
   string baselineFilePath,
   string? nationalInsuranceNumber,
   int occurrenceIndex,
   string scenarioName)
    {
        string filePath = CreateValidCopy(
            testArtifacts,
            baselineFilePath,
            scenarioName);

        XDocument document = XDocument.Load(
            filePath,
            LoadOptions.PreserveWhitespace);

        SetElementValue(
            document,
            "NINO",
            nationalInsuranceNumber ?? string.Empty,
            occurrenceIndex);

        SaveXml(document, filePath);

        return filePath;
    }

    public static string CreateWithMoneyOwedToEmployer(
        TestArtifacts testArtifacts,
        string baselineFilePath,
        string? moneyOwed,
        string scenarioName)
    {
        return CreateWithElementValue(
            testArtifacts,
            baselineFilePath,
            "MoneyOwedToEmployer",
            moneyOwed ?? string.Empty,
            scenarioName);
    }

    public static string CreateWithEmploymentDates(
      TestArtifacts testArtifacts,
      string baselineFilePath,
      DateOnly? startDate,
      DateOnly? endDate,
      string scenarioName)
    {
        string filePath = CreateValidCopy(
            testArtifacts,
            baselineFilePath,
            scenarioName);

        XDocument document = XDocument.Load(
            filePath,
            LoadOptions.PreserveWhitespace);

        XElement employee = document
            .Descendants(_ns + "Employee")
            .FirstOrDefault()
            ?? throw new InvalidOperationException("No Employee element found.");

        SetChildElementValue(
            employee,
            "StartDate",
            FormatDate(startDate));

        SetChildElementValue(
            employee,
            "EndDate",
            FormatDate(endDate));

        SaveXml(document, filePath);

        return filePath;
    }

    private static void SetChildElementValue(
        XElement parent,
        string elementName,
        string value)
    {
        XElement element = parent
            .Element(_ns + elementName)
            ?? throw new InvalidOperationException(
                $"Element '{elementName}' was not found under '{parent.Name.LocalName}'.");

        element.Value = value;
    }

    public static string CreateWithArrearsDates(
        TestArtifacts testArtifacts,
        string baselineFilePath,
        string? startDate,
        string? endDate,
        string scenarioName)
    {
        Dictionary<string, string> mutations = new()
        {
            ["AOP1StartDate"] = startDate ?? string.Empty,
            ["AOP1EndDate"] = endDate ?? string.Empty
        };

        return CreateWithElementValues(
            testArtifacts,
            baselineFilePath,
            mutations,
            scenarioName);
    }

    public static string CreateWithArrearsAmount(
        TestArtifacts testArtifacts,
        string baselineFilePath,
        int periodNumber,
        string? amountOwed,
        string scenarioName)
    {
        ValidatePositiveNumber(periodNumber, nameof(periodNumber));

        return CreateWithElementValue(
            testArtifacts,
            baselineFilePath,
            $"AOPOwed{periodNumber}",
            amountOwed ?? string.Empty,
            scenarioName);
    }

    public static string CreateWithElementValue(
        TestArtifacts testArtifacts,
        string baselineFilePath,
        string elementName,
        string value,
        string scenarioName)
    {
        Dictionary<string, string> mutations = new()
        {
            [elementName] = value
        };

        return CreateWithElementValues(
            testArtifacts,
            baselineFilePath,
            mutations,
            scenarioName);
    }

    public static string CreateWithElementValues(
    TestArtifacts testArtifacts,
    string baselineFilePath,
    IReadOnlyDictionary<string, string> mutations,
    string scenarioName)
    {
        ArgumentNullException.ThrowIfNull(mutations);

        string filePath = CreateValidCopy(
            testArtifacts,
            baselineFilePath,
            scenarioName);

        XDocument document = XDocument.Load(
            filePath,
            LoadOptions.PreserveWhitespace);

        foreach (KeyValuePair<string, string> mutation in mutations)
        {
            SetFirstElementValue(
                document,
                mutation.Key,
                mutation.Value);
        }

        SaveXml(document, filePath);

        return filePath;
    }

    private static void SetFirstElementValue(
        XDocument document,
        string elementName,
        string value)
    {
        XElement element = document
            .Descendants(_ns + elementName)
            .FirstOrDefault()
            ?? throw new InvalidOperationException(
                $"Element '{elementName}' was not found in the RP14A baseline XML.");

        element.Value = value;
    }

    private static void SaveXml(XDocument document, string filePath)
    {
        using StreamWriter writer = new(
            filePath,
            append: false,
            encoding: new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

        document.Save(writer, SaveOptions.DisableFormatting);
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
                $"RP14A baseline XML file was not found: {absolutePath}",
                absolutePath);
        }

        return absolutePath;
    }

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

    private static void SetElementValue(
    XDocument document,
    string elementName,
    string value,
    int occurrenceIndex = 0)
    {
        List<XElement> elements = document
            .Descendants(_ns + elementName)
            .ToList();

        if (elements.Count == 0)
        {
            throw new InvalidOperationException(
                $"Element '{elementName}' was not found in the RP14A baseline XML.");
        }

        if (occurrenceIndex < 0 || occurrenceIndex >= elements.Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(occurrenceIndex),
                $"Element '{elementName}' occurrence index {occurrenceIndex} is out of range. Found {elements.Count} elements.");
        }

        elements[occurrenceIndex].Value = value;
    }
}
