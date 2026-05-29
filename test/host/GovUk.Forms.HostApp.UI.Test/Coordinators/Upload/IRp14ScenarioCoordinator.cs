namespace GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;

public interface IRp14ScenarioCoordinator
{
    Task UploadValidRp14Async();
    Task UploadRp14WithCaseReferenceAsync(string? caseReference);
    Task UploadRp14WithBusinessNameAsync(string? businessName);
    Task UploadRp14WithCompanyNumberAsync(string? companyNumber);
    Task UploadRp14WithIncorporationDateAsync(DateOnly? incorporationDate);
    Task UploadRp14WithPayeAsync(string? district, string? reference);
    Task UploadRp14WithNatureOfBusinessAsync(string? natureOfBusiness);
    Task UploadRp14WithStandardIndustrialClassificationAsync(string? standardIndustrialClassification);
    Task UploadRp14WithShareholdersAsync(
    int shareholderCount,
    string? fullName,
    string? numberOfShares,
    string? percentage);
    Task UploadRp14WithDirectorAsync(
        int directorNumber,
        string? surname,
        string? initials,
        string? nino);

    Task UploadRp14WithShareholderAsync(
        int shareholderNumber,
        string? fullName,
        string? numberOfShares,
        string? percentage);

    Task UploadRp14WithNoOfEmployeesAsync(string? noOfEmployees);
    Task UploadRp14WithInsolvencyDetailsAsync(DateOnly? insolvencyDate, string? insolvencyType);

    Task UploadRp14WithTransferDetailsAsync(
        string? transferType,
        string? transferToName,
        DateOnly? transferDate,
        DateOnly? negotiationDate);

    Task UploadRp14WithIpDetailsAsync(
        string? registrationNumber,
        string? firmName,
        string? ipName,
        string? emailAddress);
    Task UploadRp14WithDirectorsAsync(
    int directorCount,
    string? surname,
    string? initials,
    string? nino);
}
