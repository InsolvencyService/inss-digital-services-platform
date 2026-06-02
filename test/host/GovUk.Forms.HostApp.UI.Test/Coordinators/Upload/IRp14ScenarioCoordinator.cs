using GovUk.Forms.HostApp.UI.Test.Support;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;

public interface IRp14ScenarioCoordinator
{
    Task UploadValidRp14Async();
    Task UploadRp14WithCaseReferenceAsync(string? caseReference);
    Task UploadRp14WithBusinessNameAsync(string? businessName);
    Task UploadRp14WithCompanyNumberAsync(string? companyNumber);
    Task UploadRp14WithIncorporationDateAsync(DateOnly? incorporationDate);
    Task UploadRp14WithPayeAsync(string? district, string? reference);
    Task UploadRp14WithStandardIndustrialClassificationAsync(string? standardIndustrialClassification);
    Task UploadRp14WithDirectorAsync(
        int directorNumber,
        string? surname,
        string? initials,
        string? nino);
    Task UploadRp14WithDirectorsAsync(
        int directorCount,
        string? surname,
        string? initials,
        string? nino);
    Task UploadRp14WithShareholderAsync(
        int shareholderNumber,
        string? fullName,
        string? numberOfShares,
        string? percentage);
    Task UploadRp14WithShareholdersAsync(
        int shareholderCount,
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
        string? emailAddress,
        string? telephoneNumber = null);
    Task UploadRp14WithNatureOfBusinessAsync(string? natureOfBusiness);
    Task UploadRp14WithAssociatedCompanyNamesAsync(int associatedCompanyCount, string companyName);
    Task UploadRp14WithAssociatedCompanyReasonsAsync(int associatedCompanyCount, string reason);
    Task UploadRp14WithAssociatedCompanyNumbersAsync(int associatedCompanyCount, string companyNumber);
    Task UploadRp14WithEmploymentContinuityEmployerNameAsync(string? employerName);
    Task UploadRp14WithTransferToNameAsync(string? transferToName);
    Task UploadRp14WithPayRecordsContactEmailAddressAsync(string? emailAddress);
    Task UploadRp14WithPayRecordsContactNameAsync(string? name);
    Task UploadRp14WithPayRecordsContactPhoneNumberAsync(string? phoneNumber);
    Task UploadRp14WithIpNameAsync(string? ipName);
    Task UploadRp14WithIpRegistrationNumberAsync(string? registrationNumber);
    Task UploadRp14WithIpFirmNameAsync(string? firmName);
    Task UploadRp14WithIpEmailAddressAsync(string? emailAddress);
    Task UploadRp14WithIpTelephoneNumberAsync(string? telephoneNumber);
    Task UploadRp14WithCompanyAddressLineCountAsync(int lineCount);
    Task UploadRp14WithCompanyAddressLine1Async(string? addressLine);
    Task UploadRp14WithCompanyAddressFieldAsync(Rp14AddressField field, string? value);
    Task UploadRp14WithCompanyAddressLinesCountAsync(int lineCount);
    Task UploadRp14WithDirectorSurnamesAsync(int directorCount, string surname);
}
