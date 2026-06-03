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

public sealed class Rp14FixtureBuilder
{
    private readonly Dictionary<string, string?> _mutations = [];

    public Rp14FixtureBuilder WithCaseReference(string? value) =>
        Set(Rp14ElementNames.CaseReference, value);

    public Rp14FixtureBuilder WithBusinessName(string? value) =>
        Set(Rp14ElementNames.NameOfBusiness, value);

    public Rp14FixtureBuilder WithCompanyNumber(string? value) =>
        Set(Rp14ElementNames.CompanyNumber, value);

    public Rp14FixtureBuilder WithIncorporationDate(DateOnly? value) =>
        Set(Rp14ElementNames.IncorporationDate, FormatDate(value));

    public Rp14FixtureBuilder WithNatureOfBusiness(string? value) =>
        Set(Rp14ElementNames.NatureOfBusiness, value);

    public Rp14FixtureBuilder WithPaye(string? district, string? reference)
    {
        return Set(Rp14ElementNames.PayeDistrict, district)
            .Set(Rp14ElementNames.PayeReference, reference);
    }
    public Rp14FixtureBuilder WithStandardIndustrialClassification(string? value) =>
     Set(Rp14ElementNames.StandardIndustrialClassification, value);

    public Rp14FixtureBuilder WithDirector(
        int directorNumber,
        string? surname,
        string? initials,
        string? nino)
    {
        ValidateRange(directorNumber, 1, 6, nameof(directorNumber));

        return Set(Rp14ElementNames.DirectorSurname(directorNumber), surname)
            .Set(Rp14ElementNames.DirectorInitials(directorNumber), initials)
            .Set(Rp14ElementNames.DirectorNino(directorNumber), nino);
    }

    public Rp14FixtureBuilder WithShareholder(
        int shareholderNumber,
        string? fullName,
        string? numberOfShares,
        string? percentage)
    {
        ValidateRange(shareholderNumber, 1, 6, nameof(shareholderNumber));

        return Set(Rp14ElementNames.ShareholderFullName(shareholderNumber), fullName)
            .Set(Rp14ElementNames.ShareholderNoOfShares(shareholderNumber), numberOfShares)
            .Set(Rp14ElementNames.ShareholderPercentage(shareholderNumber), percentage);
    }

    public Rp14FixtureBuilder WithNoOfEmployees(string? value) =>
        Set(Rp14ElementNames.NoOfEmployees, value);

    public Rp14FixtureBuilder WithContinuityEmployerName(string? value) =>
        Set(Rp14ElementNames.EmployerName, value);

    public Rp14FixtureBuilder WithInsolvencyDetails(DateOnly? date, string? type)
    {
        return Set(Rp14ElementNames.InsolvencyDate, FormatDate(date))
            .Set(Rp14ElementNames.InsolvencyType, type);
    }

    public Rp14FixtureBuilder WithTransferDetails(
        string? transferType,
        string? transferToName,
        DateOnly? transferDate,
        DateOnly? negotiationDate)
    {
        return Set(Rp14ElementNames.TransferType, transferType)
            .Set(Rp14ElementNames.TransferToName, transferToName)
            .Set(Rp14ElementNames.TransferDate, FormatDate(transferDate))
            .Set(Rp14ElementNames.NegotiationDate, FormatDate(negotiationDate));
    }

    public Rp14FixtureBuilder WithIpDetails(
        string? registrationNumber,
        string? firmName,
        string? ipName,
        string? emailAddress)
    {
        return Set(Rp14ElementNames.IpRegistrationNumber, registrationNumber)
            .Set(Rp14ElementNames.IpFirmName, firmName)
            .Set(Rp14ElementNames.IpName, ipName)
            .Set(Rp14ElementNames.IpEmailAddress, emailAddress);
    }

    public Rp14FixtureBuilder WithCustomMutation(string elementName, string? value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(elementName);

        _mutations[elementName] = value;
        return this;
    }

    public Rp14TestFile Build(
        TestArtifacts testArtifacts,
        string baselineFilePath,
        string scenarioName = "Default")
    {
        ArgumentNullException.ThrowIfNull(testArtifacts);
        ArgumentException.ThrowIfNullOrWhiteSpace(baselineFilePath);

        string filePath = CreateValidCopy(testArtifacts, baselineFilePath);

        if (_mutations.Count == 0)
        {
            LogInfo($"Scenario '{scenarioName}': No mutations specified, creating clean copy.");

            return new Rp14TestFile(filePath, new Dictionary<string, string>(), DateTime.UtcNow);
        }

        LogInfo($"Scenario '{scenarioName}': Building fixture with {_mutations.Count} mutations.");

        try
        {
            XDocument document = LoadXmlDocument(filePath);
            XNamespace ns = ExtractNamespace(document);

            foreach ((string elementName, string? value) in _mutations)
            {
                ApplyMutation(document, ns, elementName, value);
            }

            SaveXmlDocument(document, filePath);

            Dictionary<string, string> appliedMutations = _mutations
                .Where(x => x.Value is not null)
                .ToDictionary(x => x.Key, x => x.Value!);

            LogInfo($"Scenario '{scenarioName}': Fixture created successfully at {filePath}");

            return new Rp14TestFile(filePath, appliedMutations, DateTime.UtcNow);
        }
        catch (Exception ex)
        {
            LogError($"Scenario '{scenarioName}': Failed to create fixture - {ex.Message}");
            throw;
        }
    }

    private Rp14FixtureBuilder Set(
        string elementName,
        string? value,
        bool nullMeansEmpty = true)
    {
        _mutations[elementName] = nullMeansEmpty
            ? value ?? string.Empty
            : value;

        return this;
    }

    private static void ApplyMutation(
        XDocument document,
        XNamespace ns,
        string elementName,
        string? value)
    {
        XElement element = document
            .Descendants(ns + elementName)
            .FirstOrDefault()
            ?? throw new InvalidOperationException(
                $"Element '{elementName}' was not found in the RP14 XML.");

        if (value is null)
        {
            element.Remove();
            return;
        }

        element.Value = value;
    }

    private static string CreateValidCopy(
        TestArtifacts testArtifacts,
        string baselineFilePath)
    {
        string absolutePath = ResolveBaselinePath(baselineFilePath);
        string targetPath = testArtifacts.FilePath($"rp14-{Guid.NewGuid():N}.xml");

        try
        {
            File.Copy(absolutePath, targetPath, overwrite: false);
            return targetPath;
        }
        catch (Exception ex) when (IsFileException(ex))
        {
            throw new InvalidOperationException(
                $"Failed to create RP14 fixture copy: {ex.Message}",
                ex);
        }
    }

    private static string ResolveBaselinePath(string baselineFilePath)
    {
        string absolutePath = Path.GetFullPath(
            baselineFilePath,
            AppContext.BaseDirectory);

        if (!File.Exists(absolutePath))
        {
            throw new FileNotFoundException(
                $"RP14 baseline XML file not found: {absolutePath}",
                absolutePath);
        }

        return absolutePath;
    }

    private static XDocument LoadXmlDocument(string filePath)
    {
        try
        {
            return XDocument.Load(filePath, LoadOptions.PreserveWhitespace);
        }
        catch (Exception ex) when (ex is IOException or XmlException)
        {
            throw new InvalidOperationException(
                $"Failed to load RP14 XML file: {filePath}",
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
                encoding: new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

            document.Save(writer, SaveOptions.DisableFormatting);
        }
        catch (Exception ex) when (IsFileException(ex))
        {
            throw new InvalidOperationException(
                $"Failed to save RP14 XML to '{filePath}': {ex.Message}",
                ex);
        }
    }

    private static XNamespace ExtractNamespace(XDocument document)
    {
        return document.Root?.Name.NamespaceName
            ?? throw new InvalidOperationException("RP14 XML document is empty.");
    }

    private static string FormatDate(DateOnly? date)
    {
        return date?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? string.Empty;
    }

    private static void ValidateRange(
        int value,
        int min,
        int max,
        string parameterName)
    {
        if (value < min || value > max)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                $"Value must be between {min} and {max}. Received: {value}");
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

    private static void LogError(string message) =>
        LogMessage("ERROR", message);

    private static void LogMessage(string level, string message)
    {
        string timestamp = DateTime.UtcNow.ToString(
            "yyyy-MM-dd HH:mm:ss.fff",
            CultureInfo.InvariantCulture);

        Debug.WriteLine($"[{timestamp}] [{level}] [RP14] {message}");
    }
}
