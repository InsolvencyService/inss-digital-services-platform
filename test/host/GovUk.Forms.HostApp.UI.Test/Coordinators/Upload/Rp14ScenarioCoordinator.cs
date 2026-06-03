using GovUk.Forms.HostApp.UI.Test.Builders;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Support;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;

public sealed class Rp14ScenarioCoordinator : ScenarioCoordinatorBase, IRp14ScenarioCoordinator
{
    private const string DefaultBaselineFilePath = "Resources/Rp14/rp14.xml";

    public Rp14ScenarioCoordinator(
        IFileUploadCoordinator fileUploadCoordinator,
        ScenarioContext scenarioContext,
        TestArtifacts testArtifacts,
        string? baselineFilePath = null)
        : base(
            fileUploadCoordinator,
            scenarioContext,
            testArtifacts,
            logTag: "RP14",
            defaultBaselineFilePath: DefaultBaselineFilePath,
            baselineFilePath: baselineFilePath)
    {
    }

    public Task UploadValidRp14Async() =>
        BuildAndUploadAsync(
            configure: null,
            "valid RP14 file");

    public Task UploadRp14WithCaseReferenceAsync(string? caseReference) =>
        BuildAndUploadAsync(
            builder => builder.WithCaseReference(caseReference),
            $"RP14 with case reference '{ToLogValue(caseReference)}'");

    public Task UploadRp14WithBusinessNameAsync(string? businessName) =>
        BuildAndUploadAsync(
            builder => builder.WithBusinessName(businessName),
            $"RP14 with business name '{ToLogValue(businessName)}'");

    public Task UploadRp14WithCompanyNumberAsync(string? companyNumber) =>
        BuildAndUploadAsync(
            builder => builder.WithCompanyNumber(companyNumber),
            $"RP14 with company number '{ToLogValue(companyNumber)}'");

    public Task UploadRp14WithIncorporationDateAsync(DateOnly? incorporationDate) =>
        BuildAndUploadAsync(
            builder => builder.WithIncorporationDate(incorporationDate),
            $"RP14 with incorporation date '{FormatDate(incorporationDate)}'");

    public Task UploadRp14WithPayeAsync(string? district, string? reference) =>
        BuildAndUploadAsync(
            builder => builder.WithPaye(district, reference),
            $"RP14 with PAYE district '{ToLogValue(district)}' and reference '{ToLogValue(reference)}'");

    public Task UploadRp14WithStandardIndustrialClassificationAsync(
        string? standardIndustrialClassification) =>
        BuildAndUploadAsync(
            builder => builder.WithStandardIndustrialClassification(
                standardIndustrialClassification),
            $"RP14 with standard industrial classification '{ToLogValue(standardIndustrialClassification)}'");

    public Task UploadRp14WithDirectorAsync(
        int directorNumber,
        string? surname,
        string? initials,
        string? nino)
    {
        ValidateRange(directorNumber, 1, 6, nameof(directorNumber));

        return BuildAndUploadAsync(
            builder => builder.WithDirector(directorNumber, surname, initials, nino),
            $"RP14 with director {directorNumber} details");
    }

    public Task UploadRp14WithDirectorsAsync(
        int directorCount,
        string? surname,
        string? initials,
        string? nino)
    {
        ValidateRange(directorCount, 1, 6, nameof(directorCount));

        return BuildAndUploadAsync(
            builder =>
            {
                for (int i = 1; i <= directorCount; i++)
                {
                    builder.WithDirector(
                        directorNumber: i,
                        surname: surname,
                        initials: initials,
                        nino: nino);
                }
            },
            $"RP14 with {directorCount} directors using NI number '{ToLogValue(nino)}'");
    }

    public Task UploadRp14WithShareholderAsync(
        int shareholderNumber,
        string? fullName,
        string? numberOfShares,
        string? percentage)
    {
        ValidateRange(shareholderNumber, 1, 6, nameof(shareholderNumber));

        return BuildAndUploadAsync(
            builder => builder.WithShareholder(shareholderNumber, fullName, numberOfShares, percentage),
            $"RP14 with shareholder {shareholderNumber} details");
    }

    public Task UploadRp14WithShareholdersAsync(
        int shareholderCount,
        string? fullName,
        string? numberOfShares,
        string? percentage)
    {
        ValidateRange(shareholderCount, 1, 6, nameof(shareholderCount));

        return BuildAndUploadAsync(
            builder =>
            {
                for (int i = 1; i <= shareholderCount; i++)
                {
                    builder.WithShareholder(
                        shareholderNumber: i,
                        fullName: fullName,
                        numberOfShares: numberOfShares,
                        percentage: percentage);
                }
            },
            $"RP14 with {shareholderCount} shareholder records");
    }

    public Task UploadRp14WithNoOfEmployeesAsync(string? noOfEmployees) =>
        BuildAndUploadAsync(
            builder => builder.WithNoOfEmployees(noOfEmployees),
            $"RP14 with number of employees '{ToLogValue(noOfEmployees)}'");

    public Task UploadRp14WithInsolvencyDetailsAsync(
        DateOnly? insolvencyDate,
        string? insolvencyType) =>
        BuildAndUploadAsync(
            builder => builder.WithInsolvencyDetails(insolvencyDate, insolvencyType),
            $"RP14 with insolvency date '{FormatDate(insolvencyDate)}' and type '{ToLogValue(insolvencyType)}'");

    public Task UploadRp14WithTransferDetailsAsync(
        string? transferType,
        string? transferToName,
        DateOnly? transferDate,
        DateOnly? negotiationDate) =>
        BuildAndUploadAsync(
            builder => builder.WithTransferDetails(transferType, transferToName, transferDate, negotiationDate),
            $"RP14 with transfer type '{ToLogValue(transferType)}', to '{ToLogValue(transferToName)}', " +
            $"transfer date '{FormatDate(transferDate)}', negotiation date '{FormatDate(negotiationDate)}'");

    public Task UploadRp14WithIpDetailsAsync(
        string? registrationNumber,
        string? firmName,
        string? ipName,
        string? emailAddress,
        string? telephoneNumber = null) =>
        BuildAndUploadAsync(
            builder => builder.WithIpDetails(registrationNumber, firmName, ipName, emailAddress, telephoneNumber),
            $"RP14 with IP registration '{ToLogValue(registrationNumber)}', firm '{ToLogValue(firmName)}', " +
            $"IP '{ToLogValue(ipName)}', email '{ToLogValue(emailAddress)}', telephone '{ToLogValue(telephoneNumber)}'");

    public Task UploadRp14WithNatureOfBusinessAsync(string? natureOfBusiness) =>
        BuildAndUploadAsync(
            builder => builder.WithNatureOfBusiness(natureOfBusiness),
            $"RP14 with nature of business '{ToLogValue(natureOfBusiness)}'");

    public Task UploadRp14WithAssociatedCompanyNamesAsync(int associatedCompanyCount, string companyName)
    {
        ValidatePositiveNumber(
            associatedCompanyCount,
            nameof(associatedCompanyCount));

        return BuildAndUploadAsync(
            builder =>
            {
                for (int i = 1; i <= associatedCompanyCount; i++)
                {
                    builder.WithAssociatedCompany(
                        associatedCompanyNumber: i,
                        companyName: companyName);
                }
            },
            $"RP14 with {associatedCompanyCount} associated companies");
    }

    public Task UploadRp14WithAssociatedCompanyReasonsAsync(int associatedCompanyCount, string reason)
    {
        ValidatePositiveNumber(
            associatedCompanyCount,
            nameof(associatedCompanyCount));

        return BuildAndUploadAsync(
            builder =>
            {
                for (int i = 1; i <= associatedCompanyCount; i++)
                {
                    builder.WithAssociatedCompanyReason(i, reason);
                }
            },
            $"RP14 with {associatedCompanyCount} associated company reasons");
    }

    public Task UploadRp14WithAssociatedCompanyNumbersAsync(int associatedCompanyCount, string companyNumber)
    {
        ValidatePositiveNumber(
            associatedCompanyCount,
            nameof(associatedCompanyCount));

        return BuildAndUploadAsync(
            builder =>
            {
                for (int i = 1; i <= associatedCompanyCount; i++)
                {
                    builder.WithAssociatedCompanyNumber(i, companyNumber);
                }
            },
            $"RP14 with {associatedCompanyCount} associated company numbers");
    }

    public Task UploadRp14WithEmploymentContinuityEmployerNameAsync(string? employerName) =>
        BuildAndUploadAsync(
            builder => builder.WithEmploymentContinuityEmployerName(employerName),
            $"RP14 with employment continuity employer name '{ToLogValue(employerName)}'");

    public Task UploadRp14WithTransferToNameAsync(string? transferToName) =>
        BuildAndUploadAsync(
            builder => builder.WithTransferToName(transferToName),
            $"RP14 with transfer to name '{ToLogValue(transferToName)}'");

    public Task UploadRp14WithPayRecordsContactEmailAddressAsync(string? emailAddress) =>
        BuildAndUploadAsync(
            builder => builder.WithPayRecordsContactEmailAddress(emailAddress),
            $"RP14 with pay records contact email address '{ToLogValue(emailAddress)}'");

    public Task UploadRp14WithPayRecordsContactNameAsync(string? name) =>
        BuildAndUploadAsync(
            builder => builder.WithPayRecordsContactName(name),
            $"RP14 with pay records contact name '{ToLogValue(name)}'");

    public Task UploadRp14WithPayRecordsContactPhoneNumberAsync(string? phoneNumber) =>
        BuildAndUploadAsync(
            builder => builder.WithPayRecordsContactPhoneNumber(phoneNumber),
            $"RP14 with pay records contact phone number '{ToLogValue(phoneNumber)}'");

    public Task UploadRp14WithIpNameAsync(string? ipName) =>
        BuildAndUploadAsync(
            builder => builder.WithIpName(ipName),
            $"RP14 with IP name '{ToLogValue(ipName)}'");

    public Task UploadRp14WithIpRegistrationNumberAsync(string? registrationNumber) =>
        BuildAndUploadAsync(
            builder => builder.WithIpRegistrationNumber(registrationNumber),
            $"RP14 with IP registration number '{ToLogValue(registrationNumber)}'");

    public Task UploadRp14WithIpFirmNameAsync(string? firmName) =>
        BuildAndUploadAsync(
            builder => builder.WithIpFirmName(firmName),
            $"RP14 with IP firm name '{ToLogValue(firmName)}'");

    public Task UploadRp14WithIpEmailAddressAsync(string? emailAddress) =>
        BuildAndUploadAsync(
            builder => builder.WithIpEmailAddress(emailAddress),
            $"RP14 with IP email address '{ToLogValue(emailAddress)}'");

    public Task UploadRp14WithIpTelephoneNumberAsync(string? telephoneNumber) =>
        BuildAndUploadAsync(
            builder => builder.WithIpTelephoneNumber(telephoneNumber),
            $"RP14 with IP telephone number '{ToLogValue(telephoneNumber)}'");

    public Task UploadRp14WithCompanyAddressLineCountAsync(int lineCount) =>
        BuildAndUploadAsync(
            builder => builder.WithCompanyAddressLineCount(lineCount),
            $"RP14 with {lineCount} company address lines");

    public Task UploadRp14WithCompanyAddressLine1Async(string? addressLine) =>
        BuildAndUploadAsync(
            builder => builder.WithCompanyAddressLine1(addressLine),
            $"RP14 with company address line 1 '{ToLogValue(addressLine)}'");

    public Task UploadRp14WithCompanyAddressFieldAsync(Rp14AddressField field, string? value) =>
        BuildAndUploadAsync(
            builder => builder.WithCompanyAddressField(field, value),
            $"RP14 with company address {field} '{ToLogValue(value)}'");

    public Task UploadRp14WithCompanyAddressLinesCountAsync(int lineCount) =>
        BuildAndUploadAsync(
            lineCount <= 4 ? null : builder => builder.WithCompanyAddressLinesCount(lineCount),
            $"RP14 with {lineCount} company address lines");

    public Task UploadRp14WithDirectorSurnamesAsync(int directorCount, string surname)
    {
        ValidatePositiveNumber(
            directorCount,
            nameof(directorCount));

        return BuildAndUploadAsync(
            builder =>
            {
                for (int i = 1; i <= directorCount; i++)
                {
                    builder.WithDirector(
                        directorNumber: i,
                        surname: surname,
                        initials: "J",
                        nino: $"AB12345{i}C");
                }
            },
            $"RP14 with {directorCount} invalid director surnames");
    }

    private Task BuildAndUploadAsync(
        Action<Rp14FixtureBuilder>? configure,
        string description)
    {
        Rp14TestFile testFile = BuildTestFile(configure);

        return UploadFileAsync(testFile.FilePath, description);
    }

    private Rp14TestFile BuildTestFile(Action<Rp14FixtureBuilder>? configure = null)
    {
        Rp14FixtureBuilder builder = new();

        configure?.Invoke(builder);

        return builder.Build(TestArtifacts, BaselineFilePath, ScenarioName);
    }
}
