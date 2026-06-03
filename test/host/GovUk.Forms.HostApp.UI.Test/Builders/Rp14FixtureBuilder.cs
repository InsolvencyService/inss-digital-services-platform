using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Support;
using System.Xml.Linq;

namespace GovUk.Forms.HostApp.UI.Test.Builders;

public sealed class Rp14FixtureBuilder
{
    private readonly Dictionary<string, string?> _mutations = [];
    private readonly List<(string Description, Action<XDocument, XNamespace> Apply)> _customMutations = [];

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
        string? emailAddress,
        string? telephoneNumber = null)
    {
        return Set(Rp14ElementNames.IpRegistrationNumber, registrationNumber)
            .Set(Rp14ElementNames.IpFirmName, firmName)
            .Set(Rp14ElementNames.IpName, ipName)
            .Set(Rp14ElementNames.IpEmailAddress, emailAddress)
            .Set(Rp14ElementNames.IpTelephoneNumber, telephoneNumber);
    }

    public Rp14FixtureBuilder WithCustomMutation(string elementName, string? value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(elementName);

        _mutations[elementName] = value;
        return this;
    }

    public Rp14FixtureBuilder WithAssociatedCompany(int associatedCompanyNumber, string? companyName)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(associatedCompanyNumber);

        return Set(Rp14ElementNames.AssociatedCompanyName(associatedCompanyNumber), companyName);
    }

    public Rp14FixtureBuilder WithAssociatedCompanyReason(int associatedCompanyNumber, string? reason)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(associatedCompanyNumber);

        return Set(Rp14ElementNames.AssociatedCompanyReason(associatedCompanyNumber), reason);
    }

    public Rp14FixtureBuilder WithAssociatedCompanyNumber(int associatedCompanyNumber, string? companyNumber)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(associatedCompanyNumber);

        return Set(Rp14ElementNames.AssociatedCompanyNumber(associatedCompanyNumber), companyNumber);
    }

    public Rp14FixtureBuilder WithEmploymentContinuityEmployerName(string? employerName) =>
        Set(Rp14ElementNames.EmployerName, employerName);

    public Rp14FixtureBuilder WithTransferToName(string? transferToName) =>
        Set(Rp14ElementNames.TransferToName, transferToName);

    public Rp14FixtureBuilder WithPayRecordsContactName(string? name) =>
        SetScoped(Rp14ElementNames.PayRecordsContact, Rp14ElementNames.PayRecordsContactName, name);

    public Rp14FixtureBuilder WithPayRecordsContactPhoneNumber(string? phoneNumber) =>
        Set(Rp14ElementNames.PayRecordsPhoneNumber, phoneNumber);

    public Rp14FixtureBuilder WithPayRecordsContactEmailAddress(string? emailAddress) =>
        Set(Rp14ElementNames.PayRecordsEmailAddress, emailAddress);

    public Rp14FixtureBuilder WithIpName(string? ipName) =>
        Set(Rp14ElementNames.IpName, ipName);

    public Rp14FixtureBuilder WithIpRegistrationNumber(string? registrationNumber) =>
        Set(Rp14ElementNames.IpRegistrationNumber, registrationNumber);

    public Rp14FixtureBuilder WithIpFirmName(string? firmName) =>
        Set(Rp14ElementNames.IpFirmName, firmName);

    public Rp14FixtureBuilder WithIpEmailAddress(string? emailAddress) =>
        Set(Rp14ElementNames.IpEmailAddress, emailAddress);

    public Rp14FixtureBuilder WithIpTelephoneNumber(string? telephoneNumber) =>
        Set(Rp14ElementNames.IpTelephoneNumber, telephoneNumber);

    public Rp14FixtureBuilder WithCompanyAddressLineCount(int lineCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(lineCount);

        for (int i = 2; i <= lineCount; i++)
        {
            Set(Rp14ElementNames.CompanyAddressLine(i), $"Address Line {i}");
        }

        return this;
    }

    public Rp14FixtureBuilder WithCompanyAddressLine1(string? addressLine) =>
        Set(Rp14ElementNames.CompanyAddrLine1, addressLine);

    public Rp14FixtureBuilder WithCompanyAddressField(Rp14AddressField field, string? value)
    {
        string elementName = field switch
        {
            Rp14AddressField.Line1 => Rp14ElementNames.CompanyAddrLine1,
            Rp14AddressField.Line2 => Rp14ElementNames.CompanyAddressLine(2),
            Rp14AddressField.Line3 => Rp14ElementNames.CompanyAddressLine(3),
            Rp14AddressField.Line4 => Rp14ElementNames.CompanyAddressLine(4),
            Rp14AddressField.Town => Rp14ElementNames.CompanyAddrTown,
            Rp14AddressField.County => Rp14ElementNames.CompanyAddrCounty,
            Rp14AddressField.Postcode => Rp14ElementNames.CompanyAddrPostcode,
            Rp14AddressField.Country => Rp14ElementNames.CompanyAddrCountry,
            _ => throw new ArgumentOutOfRangeException(nameof(field))
        };

        return Set(elementName, value);
    }

    public Rp14FixtureBuilder WithCompanyAddressLinesCount(int lineCount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(lineCount, 5);

        _customMutations.Add((
            $"Add address lines 5 to {lineCount}",
            (document, ns) =>
            {
                XElement previousLine = document
                    .Descendants(ns + Rp14ElementNames.CompanyAddressLine(4))
                    .FirstOrDefault() ?? throw new InvalidOperationException(
                        $"Element '{Rp14ElementNames.CompanyAddressLine(4)}' was not found in the RP14 XML.");

                for (int i = 5; i <= lineCount; i++)
                {
                    XElement newLine = new(ns + Rp14ElementNames.CompanyAddressLine(i), $"Address Line {i}");
                    previousLine.AddAfterSelf(newLine);
                    previousLine = newLine;
                }
            }));

        return this;
    }

    private Rp14FixtureBuilder SetScoped(string parentElementName, string childElementName, string? value)
    {
        _mutations[$"{parentElementName}/{childElementName}"] = value;
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

        if (_mutations.Count == 0 && _customMutations.Count == 0)
        {
            LogInfo($"Scenario '{scenarioName}': No mutations specified, creating clean copy.");

            return new Rp14TestFile(filePath, new Dictionary<string, string>(), DateTime.UtcNow);
        }

        LogInfo(_customMutations.Count > 0
            ? $"Scenario '{scenarioName}': Building fixture with {_mutations.Count} mutations and {_customMutations.Count} custom mutations."
            : $"Scenario '{scenarioName}': Building fixture with {_mutations.Count} mutations.");

        try
        {
            XDocument document = LoadXmlDocument(filePath);
            XNamespace ns = ExtractNamespace(document);

            foreach ((string elementName, string? value) in _mutations)
            {
                ApplyMutation(document, ns, elementName, value);
            }

            foreach ((string _, Action<XDocument, XNamespace> apply) in _customMutations)
            {
                apply(document, ns);
            }

            SaveXmlDocument(document, filePath);

            Dictionary<string, string> appliedMutations = _mutations
                .Where(x => x.Value is not null)
                .ToDictionary(x => x.Key, x => x.Value!);

            for (int i = 0; i < _customMutations.Count; i++)
            {
                appliedMutations[$"[custom:{i}] {_customMutations[i].Description}"] = "applied";
            }

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
        XElement? element;

        if (elementName.Contains('/'))
        {
            string[] parts = elementName.Split('/');

            if (parts.Length != 2)
            {
                throw new InvalidOperationException(
                    $"Element path '{elementName}' must contain exactly one '/' separator.");
            }

            element = document
                .Descendants(ns + parts[0])
                .FirstOrDefault()?
                .Element(ns + parts[1]);
        }
        else
        {
            element = document
                .Descendants(ns + elementName)
                .FirstOrDefault();
        }

        if (element is null)
        {
            throw new InvalidOperationException(
                $"Element '{elementName}' was not found in the RP14 XML.");
        }

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

    private static string ResolveBaselinePath(string baselineFilePath) =>
        XmlFixtureHelper.ResolveBaselinePath(baselineFilePath, "RP14");

    private static XDocument LoadXmlDocument(string filePath) =>
        XmlFixtureHelper.LoadXmlDocument(filePath);

    private static void SaveXmlDocument(XDocument document, string filePath) =>
        XmlFixtureHelper.SaveXmlDocument(document, filePath);

    private static XNamespace ExtractNamespace(XDocument document) =>
        XmlFixtureHelper.ExtractNamespace(document);

    private static string FormatDate(DateOnly? date) =>
        XmlFixtureHelper.FormatDate(date);

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

    private static bool IsFileException(Exception ex) =>
        XmlFixtureHelper.IsFileException(ex);

    private static void LogInfo(string message) =>
        XmlFixtureHelper.Log("RP14", "INFO", message);

    private static void LogError(string message) =>
        XmlFixtureHelper.Log("RP14", "ERROR", message);
}

