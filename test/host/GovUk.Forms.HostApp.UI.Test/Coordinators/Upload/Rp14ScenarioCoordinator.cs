using GovUk.Forms.HostApp.UI.Test.Builders;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Models;

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
        if (directorCount <= 0)
        {
            throw new ArgumentException(
                "Director count must be greater than 0.",
                nameof(directorCount));
        }

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
        if (shareholderCount <= 0)
        {
            throw new ArgumentException(
                "Shareholder count must be greater than 0.",
                nameof(shareholderCount));
        }

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
            $"RP14 with transfer details");

    public Task UploadRp14WithIpDetailsAsync(
        string? registrationNumber,
        string? firmName,
        string? ipName,
        string? emailAddress) =>
        BuildAndUploadAsync(
            builder => builder.WithIpDetails(registrationNumber, firmName, ipName, emailAddress),
            $"RP14 with IP details");

    public Task UploadRp14WithNatureOfBusinessAsync(string? natureOfBusiness) =>
    BuildAndUploadAsync(
        builder => builder.WithNatureOfBusiness(natureOfBusiness),
        $"RP14 with nature of business '{ToLogValue(natureOfBusiness)}'");

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
