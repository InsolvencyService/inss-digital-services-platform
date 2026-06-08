using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Models;

namespace GovUk.Forms.HostApp.UI.Test.Support;

public interface IRp14FixtureBuilder
{
    IRp14FixtureBuilder WithCaseReference(string? value);
    IRp14FixtureBuilder WithBusinessName(string? value);
    IRp14FixtureBuilder WithCompanyNumber(string? value);
    IRp14FixtureBuilder WithNatureOfBusiness(string? value);
    IRp14FixtureBuilder WithStandardIndustrialClassification(string? value);
    IRp14FixtureBuilder WithPaye(string? district, string? reference);
    IRp14FixtureBuilder WithIncorporationDate(DateOnly? value);
    IRp14FixtureBuilder WithDirector(int directorNumber, string? surname, string? initials, string? nino);
    IRp14FixtureBuilder WithDirectorSurname(int directorNumber, string? surname);
    IRp14FixtureBuilder WithDirectorNino(int directorNumber, string? nino);
    IRp14FixtureBuilder WithShareholder(int shareholderNumber, string? fullName, string? numberOfShares, string? percentage);
    IRp14FixtureBuilder WithShareholderFullName(int shareholderNumber, string? fullName);
    IRp14FixtureBuilder WithShareholderPercentage(int shareholderNumber, string? percentage);
    IRp14FixtureBuilder WithNoOfEmployees(string? value);
    IRp14FixtureBuilder WithInsolvencyDetails(DateOnly? date, string? type);
    IRp14FixtureBuilder WithTransferDetails(string? transferType, string? transferToName, DateOnly? transferDate, DateOnly? negotiationDate);
    IRp14FixtureBuilder WithTransferToName(string? transferToName);
    IRp14FixtureBuilder WithIpDetails(string? registrationNumber, string? firmName, string? ipName, string? emailAddress, string? telephoneNumber = null);
    IRp14FixtureBuilder WithIpRegistrationNumber(string? registrationNumber);
    IRp14FixtureBuilder WithIpFirmName(string? firmName);
    IRp14FixtureBuilder WithIpName(string? ipName);
    IRp14FixtureBuilder WithIpEmailAddress(string? emailAddress);
    IRp14FixtureBuilder WithIpTelephoneNumber(string? telephoneNumber);
    IRp14FixtureBuilder WithAssociatedCompany(int associatedCompanyNumber, string? companyName);
    IRp14FixtureBuilder WithAssociatedCompanyNumber(int associatedCompanyNumber, string? companyNumber);
    IRp14FixtureBuilder WithAssociatedCompanyReason(int associatedCompanyNumber, string? reason);
    IRp14FixtureBuilder WithEmploymentContinuityEmployerName(string? employerName);
    IRp14FixtureBuilder WithPayRecordsContactName(string? name);
    IRp14FixtureBuilder WithPayRecordsContactPhoneNumber(string? phoneNumber);
    IRp14FixtureBuilder WithPayRecordsContactEmailAddress(string? emailAddress);
    IRp14FixtureBuilder WithCompanyAddressLine1(string? addressLine);
    IRp14FixtureBuilder WithCompanyAddressLineCount(int lineCount);
    IRp14FixtureBuilder WithCompanyAddressLinesCount(int lineCount);
    IRp14FixtureBuilder WithCompanyAddressField(Rp14AddressField field, string? value);
    Rp14TestFile Build(TestArtifacts testArtifacts, string scenarioName = "Default");
}
