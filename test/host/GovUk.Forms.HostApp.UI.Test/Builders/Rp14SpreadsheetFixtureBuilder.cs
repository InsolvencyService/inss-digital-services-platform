using Bogus;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Support;
using Inss.Common.IPUpload.Employer.Spreadsheet;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace GovUk.Forms.HostApp.UI.Test.Builders;

public sealed class Rp14SpreadsheetFixtureBuilder : IRp14FixtureBuilder
{
    private const string Namespace = "http://www.ins.gsi.gov.uk/FileUpload/RP14_Application";

    private readonly RP14 _model = CreateDefault();
    private static readonly Faker _faker = new();

    private readonly List<Action<XDocument, XNamespace>> _customMutations = [];

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

        WithCustomMutation($"Director{directorNumber}Surname", surname ?? string.Empty);
        WithCustomMutation($"Director{directorNumber}Initials", initials ?? string.Empty);
        WithCustomMutation($"Director{directorNumber}NINO", nino ?? string.Empty);

        return this;
    }

    public IRp14FixtureBuilder WithDirectorSurname(int directorNumber, string? surname)
    {
        ValidateRange(directorNumber, 1, 6, nameof(directorNumber));

        return WithCustomMutation(
            $"Director{directorNumber}Surname",
            surname ?? string.Empty);
    }

    public IRp14FixtureBuilder WithDirectorNino(int directorNumber, string? nino)
    {
        ValidateRange(directorNumber, 1, 6, nameof(directorNumber));

        return WithCustomMutation(
            $"Director{directorNumber}NINO",
            nino ?? string.Empty);
    }

    public IRp14FixtureBuilder WithShareholder(
        int shareholderNumber,
        string? fullName,
        string? numberOfShares,
        string? percentage)
    {
        ValidateRange(shareholderNumber, 1, 6, nameof(shareholderNumber));

        WithCustomMutation($"Shareholder{shareholderNumber}FullName", fullName ?? string.Empty);
        WithCustomMutation($"Shareholder{shareholderNumber}NoOfSharesHeld", numberOfShares ?? string.Empty);
        WithCustomMutation($"Shareholder{shareholderNumber}Percentage", percentage ?? string.Empty);

        return this;
    }

    public IRp14FixtureBuilder WithShareholderFullName(int shareholderNumber, string? fullName)
    {
        ValidateRange(shareholderNumber, 1, 6, nameof(shareholderNumber));

        return WithCustomMutation(
            $"Shareholder{shareholderNumber}FullName",
            fullName ?? string.Empty);
    }

    public IRp14FixtureBuilder WithShareholderPercentage(int shareholderNumber, string? percentage)
    {
        ValidateRange(shareholderNumber, 1, 6, nameof(shareholderNumber));

        return WithCustomMutation(
            $"Shareholder{shareholderNumber}Percentage",
            percentage ?? string.Empty);
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

    public IRp14FixtureBuilder WithIpDetails(
        string? registrationNumber,
        string? firmName,
        string? ipName,
        string? emailAddress,
        string? telephoneNumber = null)
    {
        _model.InsolvencyPractitioner.IPRegistrationNumber = registrationNumber ?? string.Empty;
        _model.InsolvencyPractitioner.IPFirmName = firmName ?? string.Empty;
        _model.InsolvencyPractitioner.IPName = ipName ?? string.Empty;
        _model.InsolvencyPractitioner.IPEmailAddress = emailAddress ?? string.Empty;
        _model.InsolvencyPractitioner.IPTelephoneNumber = telephoneNumber ?? string.Empty;

        return this;
    }

    public IRp14FixtureBuilder WithIpRegistrationNumber(string? registrationNumber)
    {
        _model.InsolvencyPractitioner.IPRegistrationNumber = registrationNumber ?? string.Empty;
        return this;
    }

    public IRp14FixtureBuilder WithIpFirmName(string? firmName)
    {
        _model.InsolvencyPractitioner.IPFirmName = firmName ?? string.Empty;
        return this;
    }

    public IRp14FixtureBuilder WithIpName(string? ipName)
    {
        _model.InsolvencyPractitioner.IPName = ipName ?? string.Empty;
        return this;
    }

    public IRp14FixtureBuilder WithIpEmailAddress(string? emailAddress)
    {
        _model.InsolvencyPractitioner.IPEmailAddress = emailAddress ?? string.Empty;
        return this;
    }

    public IRp14FixtureBuilder WithIpTelephoneNumber(string? telephoneNumber)
    {
        _model.InsolvencyPractitioner.IPTelephoneNumber = telephoneNumber ?? string.Empty;
        return this;
    }

    public IRp14FixtureBuilder WithAssociatedCompany(int associatedCompanyNumber, string? companyName)
    {
        ValidateRange(associatedCompanyNumber, 1, 2, nameof(associatedCompanyNumber));

        return WithCustomMutation(
            $"AssocCompany{associatedCompanyNumber}Name",
            companyName ?? string.Empty);
    }

    public IRp14FixtureBuilder WithAssociatedCompanyNumber(int associatedCompanyNumber, string? companyNumber)
    {
        ValidateRange(associatedCompanyNumber, 1, 2, nameof(associatedCompanyNumber));

        return WithCustomMutation(
            $"AssocCompany{associatedCompanyNumber}Number",
            companyNumber ?? string.Empty);
    }

    public IRp14FixtureBuilder WithAssociatedCompanyReason(int associatedCompanyNumber, string? reason)
    {
        ValidateRange(associatedCompanyNumber, 1, 2, nameof(associatedCompanyNumber));

        return WithCustomMutation(
            $"AssocComp{associatedCompanyNumber}ReasonForAssociation",
            reason ?? string.Empty);
    }

    public IRp14FixtureBuilder WithEmploymentContinuityEmployerName(string? employerName)
    {
        _model.Employees.EmployeesClaimingContinuity.EmployerName = employerName ?? string.Empty;
        return this;
    }

    public IRp14FixtureBuilder WithTransferToName(string? transferToName)
    {
        _model.TransferDetails.TransferTo.Name = transferToName ?? string.Empty;
        return this;
    }

    public IRp14FixtureBuilder WithPayRecordsContactName(string? name)
    {
        _model.PayRecordsContact.Name = name ?? string.Empty;
        return this;
    }

    public IRp14FixtureBuilder WithPayRecordsContactPhoneNumber(string? phoneNumber)
    {
        _model.PayRecordsContact.PayRecordsPhoneNumber = phoneNumber ?? string.Empty;
        return this;
    }

    public IRp14FixtureBuilder WithPayRecordsContactEmailAddress(string? emailAddress)
    {
        _model.PayRecordsContact.PayRecordsEmailAddress = emailAddress ?? string.Empty;
        return this;
    }

    public IRp14FixtureBuilder WithCompanyAddressField(Rp14AddressField field, string? value)
    {
        string safeValue = value ?? string.Empty;

        _ = field switch
        {
            Rp14AddressField.Line1 => _model.CompanyAddrLine1 = safeValue,
            Rp14AddressField.Line2 => _model.CompanyAddrLine2 = safeValue,
            Rp14AddressField.Line3 => _model.CompanyAddrLine3 = safeValue,
            Rp14AddressField.Line4 => _model.CompanyAddrLine4 = safeValue,
            Rp14AddressField.Town => _model.CompanyAddrTown = safeValue,
            Rp14AddressField.County => _model.CompanyAddrCounty = safeValue,
            Rp14AddressField.Postcode => _model.CompanyAddrPostcode = safeValue,
            Rp14AddressField.Country => _model.CompanyAddrCountry = safeValue,
            _ => throw new ArgumentOutOfRangeException(nameof(field))
        };

        return this;
    }

    public IRp14FixtureBuilder WithCompanyAddressLine1(string? addressLine)
    {
        _model.CompanyAddrLine1 = addressLine ?? string.Empty;
        return this;
    }

    public IRp14FixtureBuilder WithCompanyAddressLineCount(int lineCount)
    {
        if (lineCount < 2) { _model.CompanyAddrLine2 = string.Empty; }
        if (lineCount < 3) { _model.CompanyAddrLine3 = string.Empty; }
        if (lineCount < 4) { _model.CompanyAddrLine4 = string.Empty; }
        return this;
    }

    public IRp14FixtureBuilder WithCompanyAddressLinesCount(int lineCount)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(lineCount, 5);

        _customMutations.Add((document, ns) =>
        {
            XElement previousLine = FindRequiredElement(
                document,
                ns,
                Rp14ElementNames.CompanyAddressLine(4));

            for (int i = 5; i <= lineCount; i++)
            {
                XElement newLine = new(
                    ns + Rp14ElementNames.CompanyAddressLine(i),
                    $"Address Line {i}");

                previousLine.AddAfterSelf(newLine);
                previousLine = newLine;
            }
        });

        return this;
    }

    public IRp14FixtureBuilder WithCustomMutation(string elementName, string? value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(elementName);

        _customMutations.Add((document, ns) =>
        {
            XElement element = FindRequiredElement(document, ns, elementName);

            if (value is null)
            {
                element.Remove();
            }
            else
            {
                element.Value = value;
            }
        });

        return this;
    }

    public Rp14TestFile Build(
        TestArtifacts testArtifacts,
        string scenarioName = "Default")
    {
        ArgumentNullException.ThrowIfNull(testArtifacts);

        string filePath = testArtifacts.FilePath($"rp14-{Guid.NewGuid():N}.xml");

        SerializeModel(filePath);
        ApplyCustomMutations(filePath);

        LogInfo($"Scenario '{scenarioName}': Fixture created at {filePath}");

        return new Rp14TestFile(
            filePath,
            new Dictionary<string, string>(),
            DateTime.UtcNow);
    }

    private void SerializeModel(string filePath)
    {
        XmlSerializer serializer = new(typeof(RP14));

        XmlSerializerNamespaces namespaces = new();
        namespaces.Add("ns2", Namespace);

        XmlWriterSettings settings = new()
        {
            Indent = true,
            Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)
        };

        using XmlWriter writer = XmlWriter.Create(filePath, settings);

        serializer.Serialize(writer, _model, namespaces);
    }

    private void ApplyCustomMutations(string filePath)
    {
        if (_customMutations.Count == 0)
        {
            return;
        }

        XDocument document = XmlFixtureHelper.LoadXmlDocument(filePath);
        XNamespace ns = XmlFixtureHelper.ExtractNamespace(document);

        foreach (Action<XDocument, XNamespace> apply in _customMutations)
        {
            apply(document, ns);
        }

        XmlFixtureHelper.SaveXmlDocument(document, filePath);
    }

    private static XElement FindRequiredElement(
        XDocument document,
        XNamespace ns,
        string elementName)
    {
        return document
            .Descendants(ns + elementName)
            .FirstOrDefault()
            ?? throw new InvalidOperationException(
                $"Element '{elementName}' was not found in the RP14 XML.");
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
        XmlFixtureHelper.Log("RP14", "INFO", message);

    private static RP14 CreateDefault() => new()
    {
        Header = new RP14Header
        {
            CaseReference = "CN01344678"
        },
        NameOfBusiness = _faker.Company.CompanyName(),
        CompanyNumber = "7821478",
        CompanyAddrLine1 = "9 High Street",
        CompanyAddrLine2 = "Test",
        CompanyAddrLine3 = "Test",
        CompanyAddrLine4 = "Test",
        CompanyAddrTown = "Birmingham",
        CompanyAddrCounty = "West Midlands",
        CompanyAddrPostcode = "B2 4LX",
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
            Director1 = new RP14DirectorsDirector1
            {
                Director1Surname = "Bulbs",
                Director1Initials = "J",
                Director1NINO = "KH557994B"
            },
            Director2 = new RP14DirectorsDirector2
            {
                Director2Surname = _faker.Name.LastName(),
                Director2Initials = _faker.Name.Suffix(),
                Director2NINO = "ZX567824C"
            },
            Director3 = new RP14DirectorsDirector3
            {
                Director3Surname = _faker.Name.LastName(),
                Director3Initials = _faker.Name.Suffix(),
                Director3NINO = "JK958477C"
            },
            Director4 = new RP14DirectorsDirector4
            {
                Director4Surname = "Johnson",
                Director4Initials = "J",
                Director4NINO = "KX436394D"
            },
            Director5 = new RP14DirectorsDirector5(),
            Director6 = new RP14DirectorsDirector6(),
            ClaimingRPAsEmployee = YesNoType.no,
            ClaimingRPAsEmployeeSpecified = true
        },
        Shareholders = new RP14Shareholders
        {
            Shareholder1 = new RP14ShareholdersShareholder1
            {
                Shareholder1FullName = "Barbara Johnson",
                Shareholder1NoOfSharesHeld = "40000",
                Shareholder1Percentage = 15m,
                Shareholder1PercentageSpecified = true
            },
            Shareholder2 = new RP14ShareholdersShareholder2
            {
                Shareholder2FullName = "Elizabeth Martinez",
                Shareholder2NoOfSharesHeld = "2000",
                Shareholder2Percentage = 10m,
                Shareholder2PercentageSpecified = true
            },
            Shareholder3 = new RP14ShareholdersShareholder3
            {
                Shareholder3FullName = "Patricia Hernandez",
                Shareholder3NoOfSharesHeld = "100000",
                Shareholder3Percentage = 20m,
                Shareholder3PercentageSpecified = true
            },
            Shareholder4 = new RP14ShareholdersShareholder4(),
            Shareholder5 = new RP14ShareholdersShareholder5(),
            Shareholder6 = new RP14ShareholdersShareholder6()
        },
        AssociatedCompanies = new RP14AssociatedCompanies
        {
            LegallyAssociatedCompanies = YesNoType.no,
            LegallyAssociatedCompaniesSpecified = true,
            AssociatedCompany1 = new RP14AssociatedCompaniesAssociatedCompany1
            {
                AssocCompany1Name = "Northbridge Logistics Ltd",
                AssocCompany1Number = "08274519",
                AssocComp1AddrLine1 = "45 Wellington Industrial Estate",
                AssocComp1AddrTown = "Manchester",
                AssocComp1AddrCounty = "Greater Manchester",
                AssocComp1AddrPostcode = "M17 1AB"
            },
            AssociatedCompany2 = new RP14AssociatedCompaniesAssociatedCompany2
            {
                AssocCompany2Name = "Silverstone Retail Services Ltd",
                AssocCompany2Number = "09163847",
                AssocComp2AddrLine1 = "128 Kingsway Business Park",
                AssocComp2AddrTown = "Leeds",
                AssocComp2AddrCounty = "West Yorkshire",
                AssocComp2AddrPostcode = "LS12 4RT",
                AssocComp2OfferToEmployEmployees =
                    new RP14AssociatedCompaniesAssociatedCompany2AssocComp2OfferToEmployEmployees
                    {
                        AssocComp2OfferMade = YesNoType.yes
                    }
            }
        },
        Employees = new RP14Employees
        {
            NoOfEmployees = "15",
            EmployeesClaimingContinuity = new RP14EmployeesEmployeesClaimingContinuity
            {
                ClaimingContinuity = YesNoType.yes,
                ClaimingContinuitySpecified = true,
                EmployerName = "Greenfield Industrial Services Ltd",
                EmployerAddrLine1 = "Unit Riverside Business Centre",
                EmployerAddrLine2 = "Park Lane Industrial Estate",
                EmployerAddrTown = "Birmingham",
                EmployerAddrCounty = "West Midlands",
                EmployerAddrPostcode = "B18 6HF",
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
                TransferToAddrLine1 = "Unit 5 Meridian Business Park",
                TransferToAddrLine2 = "Kingsway Industrial Estate",
                TransferToAddrTown = "Nottingham",
                TransferToAddrCounty = "Nottinghamshire",
                TransferToAddrPostcode = "NG2 1AA"
            },
            TransferDate = new DateTime(2026, 3, 28),
            TransferDateSpecified = true,
            NegotiationDate = new DateTime(2026, 3, 10),
            NegotiationDateSpecified = true
        },
        PayRecordsContact = new RP14PayRecordsContact
        {
            Name = _faker.Company.CompanyName(),
            PayRecordsAddrLine1 = "78 Oxford Drive",
            PayRecordsAddrLine2 = "78 Oxford Drive",
            PayRecordsAddrTown = "Cardiff",
            PayRecordsAddrCounty = "South Glamorgan",
            PayRecordsAddrPostcode = "CF10 4QY",
            PayRecordsPhoneNumber = "020234567891",
            PayRecordsEmailAddress = "testing@gmail.com"
        },
        InsolvencyPractitioner = new RP14InsolvencyPractitioner
        {
            IPRegistrationNumber = "9456",
            IPFirmName = "Anderson Insolvency Partners Ltd",
            IPName = "Michael Anderson",
            IPAddressLine1 = "Suite 4, Regent House",
            IPAddressLine2 = "18 Market Street",
            IPAddressTown = "Bristol",
            IPAddressCounty = "Somerset",
            IPAddressPostcode = "BS1 5AH",
            IPTelephoneNumber = "01174962841",
            IPEmailAddress = "michael.anderson@aipartners.co.uk",
            DeclarationAgreement = true
        }
    };
}