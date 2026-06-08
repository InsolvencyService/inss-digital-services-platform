using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Support;
using Inss.Common.IPUpload.Employer.Api;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace GovUk.Forms.HostApp.UI.Test.Builders;

public sealed class Rp14ApiFixtureBuilder : IRp14FixtureBuilder
{
    private const string Namespace = "www.inss.gsi.gov.uk/RP14_Application";

    private static readonly XmlSerializerNamespaces _serializerNamespaces = CreateSerializerNamespaces();

    private readonly RP14 _model = CreateDefault();

    private readonly List<string> _addressLines = ["9 High Street", "Test", "Test", "Test"];

    private readonly List<RP14DirectorsDirector> _directors = CreateDefaultDirectors();

    private readonly List<RP14Shareholder> _shareholders = CreateDefaultShareholders();

    private readonly List<RP14AssociatedCompaniesAssociatedCompany> _associatedCompanies =
        CreateDefaultAssociatedCompanies();

    public IRp14FixtureBuilder WithCaseReference(string? value)
    {
        _model.Header.CaseReference = value ?? string.Empty;
        return this;
    }

    public IRp14FixtureBuilder WithBusinessName(string? value)
    {
        _model.NameOfBusiness = value ?? string.Empty;
        return this;
    }

    public IRp14FixtureBuilder WithCompanyNumber(string? value)
    {
        _model.CompanyNumber = value ?? string.Empty;
        return this;
    }

    public IRp14FixtureBuilder WithNatureOfBusiness(string? value)
    {
        _model.NatureOfBusiness = value ?? string.Empty;
        return this;
    }

    public IRp14FixtureBuilder WithStandardIndustrialClassification(string? value)
    {
        _model.SICCode = value ?? string.Empty;
        return this;
    }

    public IRp14FixtureBuilder WithPaye(string? district, string? reference)
    {
        _model.PAYE.District = district ?? string.Empty;
        _model.PAYE.Reference = reference ?? string.Empty;
        return this;
    }

    public IRp14FixtureBuilder WithIncorporationDate(DateOnly? value)
    {
        SetDate(
            value,
            date => _model.IncorporationDate = date,
            specified => _model.IncorporationDateSpecified = specified);

        return this;
    }

    public IRp14FixtureBuilder WithDirector(
        int directorNumber,
        string? surname,
        string? initials,
        string? nino)
    {
        ValidateRange(directorNumber, 1, 6, nameof(directorNumber));
        EnsureDirectorSlot(directorNumber);

        RP14DirectorsDirector director = _directors[directorNumber - 1];

        director.Name ??= new NameType();
        director.Name.Surname = surname ?? string.Empty;
        director.Name.Initials = initials ?? string.Empty;
        director.NINO = nino ?? string.Empty;

        return this;
    }

    public IRp14FixtureBuilder WithDirectorSurname(int directorNumber, string? surname)
    {
        ValidateRange(directorNumber, 1, 6, nameof(directorNumber));
        EnsureDirectorSlot(directorNumber);

        _directors[directorNumber - 1].Name ??= new NameType();
        _directors[directorNumber - 1].Name.Surname = surname ?? string.Empty;

        return this;
    }

    public IRp14FixtureBuilder WithDirectorNino(int directorNumber, string? nino)
    {
        ValidateRange(directorNumber, 1, 6, nameof(directorNumber));
        EnsureDirectorSlot(directorNumber);

        _directors[directorNumber - 1].NINO = nino ?? string.Empty;

        return this;
    }

    public IRp14FixtureBuilder WithShareholder(
        int shareholderNumber,
        string? fullName,
        string? numberOfShares,
        string? percentage)
    {
        ValidateRange(shareholderNumber, 1, 6, nameof(shareholderNumber));
        EnsureShareholderSlot(shareholderNumber);

        RP14Shareholder shareholder = _shareholders[shareholderNumber - 1];

        shareholder.Name ??= new NameType();
        shareholder.Name.FullName = fullName ?? string.Empty;
        shareholder.NoOfSharesHeld = numberOfShares ?? string.Empty;

        SetShareholderPercentage(shareholder, percentage);

        return this;
    }

    public IRp14FixtureBuilder WithShareholderFullName(int shareholderNumber, string? fullName)
    {
        ValidateRange(shareholderNumber, 1, 6, nameof(shareholderNumber));
        EnsureShareholderSlot(shareholderNumber);

        _shareholders[shareholderNumber - 1].Name ??= new NameType();
        _shareholders[shareholderNumber - 1].Name.FullName = fullName ?? string.Empty;

        return this;
    }

    public IRp14FixtureBuilder WithShareholderPercentage(int shareholderNumber, string? percentage)
    {
        ValidateRange(shareholderNumber, 1, 6, nameof(shareholderNumber));
        EnsureShareholderSlot(shareholderNumber);

        SetShareholderPercentage(
            _shareholders[shareholderNumber - 1],
            percentage);

        return this;
    }

    public IRp14FixtureBuilder WithShareholderPercentage(int shareholderNumber, decimal percentage)
    {
        ValidateRange(shareholderNumber, 1, 6, nameof(shareholderNumber));
        EnsureShareholderSlot(shareholderNumber);

        RP14Shareholder shareholder = _shareholders[shareholderNumber - 1];

        shareholder.Percentage = percentage;
        shareholder.PercentageSpecified = true;

        return this;
    }

    public IRp14FixtureBuilder WithNoOfEmployees(string? value)
    {
        _model.Employees.NoOfEmployees = value ?? string.Empty;
        return this;
    }

    public IRp14FixtureBuilder WithInsolvencyDetails(DateOnly? date, string? type)
    {
        SetDate(
            date,
            value => _model.InsolvencyDetails.InsolvencyDate = value,
            specified => _model.InsolvencyDetails.InsolvencyDateSpecified = specified);

        _model.InsolvencyDetails.InsolvencyType = type ?? string.Empty;

        return this;
    }

    public IRp14FixtureBuilder WithTransferDetails(
        string? transferType,
        string? transferToName,
        DateOnly? transferDate,
        DateOnly? negotiationDate)
    {
        if (string.IsNullOrWhiteSpace(transferType))
        {
            _model.TransferDetails.TransferTypeSpecified = false;
        }
        else
        {
            _model.TransferDetails.TransferType = ParseTransferType(transferType);
            _model.TransferDetails.TransferTypeSpecified = true;
        }

        _model.TransferDetails.TransferTo.Name = transferToName ?? string.Empty;

        SetDate(
            transferDate,
            value => _model.TransferDetails.TransferDate = value,
            specified => _model.TransferDetails.TransferDateSpecified = specified);

        SetDate(
            negotiationDate,
            value => _model.TransferDetails.NegotiationDate = value,
            specified => _model.TransferDetails.NegotiationDateSpecified = specified);

        return this;
    }

    public IRp14FixtureBuilder WithTransferToName(string? transferToName)
    {
        _model.TransferDetails.TransferTo.Name = transferToName ?? string.Empty;
        return this;
    }

    public IRp14FixtureBuilder WithIpDetails(
        string? registrationNumber,
        string? firmName,
        string? ipName,
        string? emailAddress,
        string? telephoneNumber = null)
    {
        _model.InsolvencyPractitioner.RegistrationNumber = registrationNumber ?? string.Empty;
        _model.InsolvencyPractitioner.FirmName = firmName ?? string.Empty;
        _model.InsolvencyPractitioner.Name = ipName ?? string.Empty;
        _model.InsolvencyPractitioner.EmailAddress = emailAddress ?? string.Empty;
        _model.InsolvencyPractitioner.TelephoneNumber = telephoneNumber ?? string.Empty;

        return this;
    }

    public IRp14FixtureBuilder WithIpRegistrationNumber(string? registrationNumber)
    {
        _model.InsolvencyPractitioner.RegistrationNumber = registrationNumber ?? string.Empty;
        return this;
    }

    public IRp14FixtureBuilder WithIpFirmName(string? firmName)
    {
        _model.InsolvencyPractitioner.FirmName = firmName ?? string.Empty;
        return this;
    }

    public IRp14FixtureBuilder WithIpName(string? ipName)
    {
        _model.InsolvencyPractitioner.Name = ipName ?? string.Empty;
        return this;
    }

    public IRp14FixtureBuilder WithIpEmailAddress(string? emailAddress)
    {
        _model.InsolvencyPractitioner.EmailAddress = emailAddress ?? string.Empty;
        return this;
    }

    public IRp14FixtureBuilder WithIpTelephoneNumber(string? telephoneNumber)
    {
        _model.InsolvencyPractitioner.TelephoneNumber = telephoneNumber ?? string.Empty;
        return this;
    }

    public IRp14FixtureBuilder WithAssociatedCompany(int associatedCompanyNumber, string? companyName)
    {
        ValidateRange(associatedCompanyNumber, 1, 2, nameof(associatedCompanyNumber));
        EnsureAssociatedCompanySlot(associatedCompanyNumber);

        _associatedCompanies[associatedCompanyNumber - 1].CompanyName = companyName ?? string.Empty;

        return this;
    }

    public IRp14FixtureBuilder WithAssociatedCompanyNumber(int associatedCompanyNumber, string? companyNumber)
    {
        ValidateRange(associatedCompanyNumber, 1, 2, nameof(associatedCompanyNumber));
        EnsureAssociatedCompanySlot(associatedCompanyNumber);

        _associatedCompanies[associatedCompanyNumber - 1].CompanyNumber = companyNumber ?? string.Empty;

        return this;
    }

    public IRp14FixtureBuilder WithAssociatedCompanyReason(int associatedCompanyNumber, string? reason)
    {
        ValidateRange(associatedCompanyNumber, 1, 2, nameof(associatedCompanyNumber));
        EnsureAssociatedCompanySlot(associatedCompanyNumber);

        _associatedCompanies[associatedCompanyNumber - 1].ReasonForAssociation = reason ?? string.Empty;

        return this;
    }

    public IRp14FixtureBuilder WithEmploymentContinuityEmployerName(string? employerName)
    {
        _model.Employees.EmployeesClaimingContinuity.EmployerName = employerName ?? string.Empty;
        return this;
    }

    public IRp14FixtureBuilder WithPayRecordsContactName(string? name)
    {
        _model.PayRecordsContact.Name = name ?? string.Empty;
        return this;
    }

    public IRp14FixtureBuilder WithPayRecordsContactPhoneNumber(string? phoneNumber)
    {
        _model.PayRecordsContact.PhoneNumber = phoneNumber ?? string.Empty;
        return this;
    }

    public IRp14FixtureBuilder WithPayRecordsContactEmailAddress(string? emailAddress)
    {
        _model.PayRecordsContact.EmailAddress = emailAddress ?? string.Empty;
        return this;
    }

    public IRp14FixtureBuilder WithCompanyAddressLine1(string? addressLine)
    {
        SetAddressLine(0, addressLine ?? string.Empty);
        return this;
    }

    public IRp14FixtureBuilder WithCompanyAddressLineCount(int lineCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(lineCount);

        while (_addressLines.Count < lineCount)
        {
            _addressLines.Add($"Address Line {_addressLines.Count + 1}");
        }

        if (_addressLines.Count > lineCount)
        {
            _addressLines.RemoveRange(lineCount, _addressLines.Count - lineCount);
        }

        return this;
    }

    public IRp14FixtureBuilder WithCompanyAddressLinesCount(int lineCount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(lineCount, 5);

        while (_addressLines.Count < lineCount)
        {
            _addressLines.Add($"Address Line {_addressLines.Count + 1}");
        }

        return this;
    }

    public IRp14FixtureBuilder WithCompanyAddressField(Rp14AddressField field, string? value)
    {
        string safeValue = value ?? string.Empty;

        switch (field)
        {
            case Rp14AddressField.Line1:
                SetAddressLine(0, safeValue);
                break;
            case Rp14AddressField.Line2:
                SetAddressLine(1, safeValue);
                break;
            case Rp14AddressField.Line3:
                SetAddressLine(2, safeValue);
                break;
            case Rp14AddressField.Line4:
                SetAddressLine(3, safeValue);
                break;
            case Rp14AddressField.Town:
                _model.Address.Town = safeValue;
                break;
            case Rp14AddressField.County:
                _model.Address.County = safeValue;
                break;
            case Rp14AddressField.Postcode:
                _model.Address.Postcode = safeValue;
                break;
            case Rp14AddressField.Country:
                _model.Address.Country = safeValue;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(field));
        }

        return this;
    }

    public Rp14TestFile Build(TestArtifacts testArtifacts, string scenarioName = "Default")
    {
        ArgumentNullException.ThrowIfNull(testArtifacts);

        _model.Address.Line = _addressLines.ToArray();
        _model.Directors.Director = _directors.ToArray();
        _model.Shareholders = _shareholders.ToArray();
        _model.AssociatedCompanies.AssociatedCompany = _associatedCompanies.ToArray();

        string filePath = testArtifacts.FilePath($"rp14-api-{Guid.NewGuid():N}.xml");

        SerializeModel(filePath);

        LogInfo($"Scenario '{scenarioName}': Fixture created at {filePath}");

        return new Rp14TestFile(
            filePath,
            new Dictionary<string, string>(),
            DateTime.UtcNow);
    }

    private void SerializeModel(string filePath)
    {
        XmlSerializer serializer = new(typeof(RP14));

        XmlWriterSettings settings = new()
        {
            Indent = true,
            Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)
        };

        using XmlWriter writer = XmlWriter.Create(filePath, settings);

        serializer.Serialize(writer, _model, _serializerNamespaces);
    }

    private void SetAddressLine(int index, string value)
    {
        while (_addressLines.Count <= index)
        {
            _addressLines.Add(string.Empty);
        }

        _addressLines[index] = value;
    }

    private void EnsureDirectorSlot(int count)
    {
        while (_directors.Count < count)
        {
            _directors.Add(new RP14DirectorsDirector
            {
                Name = new NameType()
            });
        }
    }

    private void EnsureShareholderSlot(int count)
    {
        while (_shareholders.Count < count)
        {
            _shareholders.Add(new RP14Shareholder
            {
                Name = new NameType()
            });
        }
    }

    private void EnsureAssociatedCompanySlot(int count)
    {
        while (_associatedCompanies.Count < count)
        {
            _associatedCompanies.Add(new RP14AssociatedCompaniesAssociatedCompany());
        }
    }

    private static void SetShareholderPercentage(
        RP14Shareholder shareholder,
        string? percentage)
    {
        if (string.IsNullOrWhiteSpace(percentage))
        {
            shareholder.PercentageSpecified = false;
            return;
        }

        if (!decimal.TryParse(percentage, out decimal parsed))
        {
            throw new ArgumentException(
                $"'{percentage}' is not a valid decimal percentage. " +
                "Use the spreadsheet fixture builder for invalid percentage format tests.");
        }

        shareholder.Percentage = parsed;
        shareholder.PercentageSpecified = true;
    }

    private static void SetDate(
        DateOnly? value,
        Action<DateTime> setDate,
        Action<bool> setSpecified)
    {
        if (value.HasValue)
        {
            setDate(value.Value.ToDateTime(TimeOnly.MinValue));
            setSpecified(true);
        }
        else
        {
            setSpecified(false);
        }
    }

    private static RP14TransferDetailsTransferType ParseTransferType(string transferType) =>
        transferType.Trim().ToLowerInvariant() switch
        {
            "asset only" => RP14TransferDetailsTransferType.assetonly,
            "whole business" => RP14TransferDetailsTransferType.wholebusiness,
            "part of business" => RP14TransferDetailsTransferType.partofbusiness,
            "other" => RP14TransferDetailsTransferType.other,
            "none" => RP14TransferDetailsTransferType.none,
            _ => throw new ArgumentException(
                $"Unknown transfer type: '{transferType}'",
                nameof(transferType))
        };

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

    private static void LogInfo(string message) =>
        XmlFixtureHelper.Log("RP14-API", "INFO", message);

    private static XmlSerializerNamespaces CreateSerializerNamespaces()
    {
        XmlSerializerNamespaces namespaces = new();
        namespaces.Add("tns", Namespace);
        return namespaces;
    }

    private static RP14 CreateDefault() => new()
    {
        Header = new RP14Header
        {
            CaseReference = "CN01344678"
        },
        NameOfBusiness = "Livewire Ltd",
        CompanyNumber = "7821478",
        Address = new AddressType
        {
            Town = "Birmingham",
            County = "West Midlands",
            Postcode = "B2 4LX",
            Country = "England"
        },
        IncorporationDate = new DateTime(1973, 2, 21),
        IncorporationDateSpecified = true,
        NatureOfBusiness = "Light bulbs",
        PAYE = new PayeType
        {
            District = "123",
            Reference = "AA4567"
        },
        SICCode = "27400",
        Directors = new RP14Directors
        {
            ClaimingRPAsEmployee = YesNoType.no,
            ClaimingRPAsEmployeeSpecified = true
        },
        AssociatedCompanies = new RP14AssociatedCompanies
        {
            LegallyAssociatedCompanies = YesNoType.no,
            LegallyAssociatedCompaniesSpecified = true
        },
        Employees = new RP14Employees
        {
            NoOfEmployees = "15",
            EmployeesClaimingContinuity = new RP14EmployeesEmployeesClaimingContinuity
            {
                ClaimingContinuity = YesNoType.yes,
                ClaimingContinuitySpecified = true,
                EmployerName = "Greenfield Industrial Services Ltd",
                Address = new AddressType
                {
                    Line = ["Unit Riverside Business Centre", "Park Lane Industrial Estate"],
                    Town = "Birmingham",
                    County = "West Midlands",
                    Postcode = "B18 6HF"
                },
                TradingDate = new DateTime(2018, 9, 18),
                TradingDateSpecified = true,
                ShouldClaimsBeAccepted = YesNoType.yes,
                ShouldClaimsBeAcceptedSpecified = true
            },
            Strikes = YesNoType.no,
            StrikesSpecified = true,
            CarryOverHolidayEntitlement = YesNoType.yes,
            CarryOverHolidayEntitlementSpecified = true
        },
        InsolvencyDetails = new RP14InsolvencyDetails
        {
            InsolvencyDate = new DateTime(2026, 3, 14),
            InsolvencyDateSpecified = true,
            InsolvencyType = "Administration"
        },
        TransferDetails = new RP14TransferDetails
        {
            TransferType = RP14TransferDetailsTransferType.wholebusiness,
            TransferTypeSpecified = true,
            TransferTo = new RP14TransferDetailsTransferTo
            {
                Name = "Horizon Workforce Solutions Ltd",
                Address = new AddressType
                {
                    Line = ["Unit 5 Meridian Business Park", "Kingsway Industrial Estate"],
                    Town = "Nottingham",
                    County = "Nottinghamshire",
                    Postcode = "NG2 1AA"
                }
            },
            TransferDate = new DateTime(2026, 3, 28),
            TransferDateSpecified = true,
            NegotiationDate = new DateTime(2026, 3, 10),
            NegotiationDateSpecified = true
        },
        PayRecordsContact = new RP14PayRecordsContact
        {
            Name = "Payment Tests",
            Address = new AddressType
            {
                Line = ["78 Oxford Drive"],
                Town = "Cardiff",
                County = "South Glamorgan",
                Postcode = "CF10 4QY"
            },
            PhoneNumber = "020234567891",
            EmailAddress = "testing@gmail.com"
        },
        InsolvencyPractitioner = new RP14InsolvencyPractitioner
        {
            RegistrationNumber = "9456",
            FirmName = "Anderson Insolvency Partners Ltd",
            Name = "Michael Anderson",
            Address = new AddressType
            {
                Line = ["Suite 4, Regent House", "18 Market Street"],
                Town = "Bristol",
                County = "Somerset",
                Postcode = "BS1 5AH"
            },
            TelephoneNumber = "01174962841",
            EmailAddress = "michael.anderson@aipartners.co.uk",
            DeclarationAgreement = true
        }
    };

    private static List<RP14DirectorsDirector> CreateDefaultDirectors() =>
    [
        new() { Name = new NameType { Surname = "Bulbs", Initials = "J" }, NINO = "KH557994B" },
        new() { Name = new NameType { Surname = "Williams", Initials = "A" }, NINO = "ZX567824C" },
        new() { Name = new NameType { Surname = "Garcia", Initials = "G" }, NINO = "JK958477C" },
        new() { Name = new NameType { Surname = "Johnson", Initials = "J" }, NINO = "KX436394D" }
    ];

    private static List<RP14Shareholder> CreateDefaultShareholders() =>
    [
        new()
        {
            Name = new NameType { FullName = "Barbara Johnson" },
            NoOfSharesHeld = "40000",
            Percentage = 15m,
            PercentageSpecified = true
        },
        new()
        {
            Name = new NameType { FullName = "Elizabeth Martinez" },
            NoOfSharesHeld = "2000",
            Percentage = 10m,
            PercentageSpecified = true
        },
        new()
        {
            Name = new NameType { FullName = "Patricia Hernandez" },
            NoOfSharesHeld = "100000",
            Percentage = 20m,
            PercentageSpecified = true
        }
    ];

    private static List<RP14AssociatedCompaniesAssociatedCompany> CreateDefaultAssociatedCompanies() =>
    [
        new()
        {
            CompanyName = "Northbridge Logistics Ltd",
            CompanyNumber = "08274519",
            Address = new AddressType
            {
                Line = ["45 Wellington Industrial Estate"],
                Town = "Manchester",
                County = "Greater Manchester",
                Postcode = "M17 1AB"
            }
        },
        new()
        {
            CompanyName = "Silverstone Retail Services Ltd",
            CompanyNumber = "09163847",
            Address = new AddressType
            {
                Line = ["128 Kingsway Business Park"],
                Town = "Leeds",
                County = "West Yorkshire",
                Postcode = "LS12 4RT"
            },
            OfferToEmployEmployees = new RP14AssociatedCompaniesAssociatedCompanyOfferToEmployEmployees
            {
                OfferMade = YesNoType.yes
            }
        }
    ];
}