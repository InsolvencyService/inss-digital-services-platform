using GovUk.Forms.HostApp.UI.Test.Factories;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Models.TestData;
using GovUk.Forms.HostApp.UI.Test.Support;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;

public sealed class UploadDocumentCoordinator :
    BaseCoordinator,
    IUploadPageCoordinator,
    IFileUploadCoordinator,
    IRp14aScenarioCoordinator,
    IRp14ScenarioCoordinator,
    IUploadVerificationCoordinator,
    IUploadNavigationCoordinator
{
    private readonly IUploadPageCoordinator _pageCoordinator;
    private readonly IFileUploadCoordinator _fileUploadCoordinator;
    private readonly IRp14aScenarioCoordinator _scenarioCoordinator;
    private readonly IUploadVerificationCoordinator _verificationCoordinator;
    private readonly IUploadNavigationCoordinator _navigationCoordinator;
    private readonly IRp14ScenarioCoordinator _rp14ScenarioCoordinator;

    public UploadDocumentCoordinator(
        TestArtifacts testArtifacts,
        IUploadPageCoordinator pageCoordinator,
        IFileUploadCoordinator fileUploadCoordinator,
        IRp14aScenarioCoordinator scenarioCoordinator,
        IRp14ScenarioCoordinator rp14ScenarioCoordinator,
        IUploadVerificationCoordinator verificationCoordinator,
        IUploadNavigationCoordinator navigationCoordinator)
        : base(testArtifacts)
    {
        _pageCoordinator = pageCoordinator
            ?? throw new ArgumentNullException(nameof(pageCoordinator));

        _fileUploadCoordinator = fileUploadCoordinator
            ?? throw new ArgumentNullException(nameof(fileUploadCoordinator));

        _scenarioCoordinator = scenarioCoordinator
            ?? throw new ArgumentNullException(nameof(scenarioCoordinator));

        _rp14ScenarioCoordinator = rp14ScenarioCoordinator
            ?? throw new ArgumentNullException(nameof(rp14ScenarioCoordinator));

        _verificationCoordinator = verificationCoordinator
            ?? throw new ArgumentNullException(nameof(verificationCoordinator));

        _navigationCoordinator = navigationCoordinator
            ?? throw new ArgumentNullException(nameof(navigationCoordinator));
    }

    public Task VerifyUploadDocumentPageIsDisplayedAsync()
        => _pageCoordinator.VerifyUploadDocumentPageIsDisplayedAsync();

    public Task ExpandCommonIssuesWhenUploadingRP14AFormsAsync()
        => _pageCoordinator.ExpandCommonIssuesWhenUploadingRP14AFormsAsync();

    public Task UploadFileAsync(string filePath)
        => _fileUploadCoordinator.UploadFileAsync(filePath);

    public Task UploadUnsupportedFileAsync(string extension)
    {
        string filePath = TestFileFactory.CreateUnsupportedFile(TestArtifacts, extension);
        return UploadFileAsync(filePath);
    }

    public Task UploadXmlFileWithWrongContentAsync()
    {
        string filePath = TestFileFactory.CreateXmlWithWrongContent(TestArtifacts);
        return UploadFileAsync(filePath);
    }

    public Task UploadValidXmlFileAboveMaximumSizeAsync()
    {
        string filePath = TestFileFactory.CreateValidXmlFileAboveSize(TestArtifacts, 10);
        return UploadFileAsync(filePath);
    }

    public Task VerifyThatFileIsUploadedAsync()
        => _verificationCoordinator.VerifyThatFileIsUploadedAsync();

    public Task VerifyOnlyOneFileUploadedAsync()
        => _verificationCoordinator.VerifyOnlyOneFileUploadedAsync();

    public Task VerifyInvalidFileExtensionErrorAsync(UploadFileError uploadFileError)
        => _verificationCoordinator.VerifyInvalidFileExtensionErrorAsync(uploadFileError);

    public Task VerifyUploadDocumentContentSnapShotAsync()
        => _fileUploadCoordinator.VerifyUploadDocumentContentSnapShotAsync();

    public Task VerifyUploadCommonIssuesContentVisualSnapShotAsync()
        => _fileUploadCoordinator.VerifyUploadCommonIssuesContentVisualSnapShotAsync();

    public Task ClickOnContinueButtonAsync()
        => _navigationCoordinator.ClickOnContinueButtonAsync();

    public Task ClickOnBackButtonAsync()
        => _navigationCoordinator.ClickOnBackButtonAsync();

    public Task NavigateToFeedbackPageAsync()
        => _navigationCoordinator.NavigateToFeedbackPageAsync();

    public Task NavigateToSubmitPageAsync()
        => _navigationCoordinator.NavigateToSubmitPageAsync();

    // RP14A scenarios

    public Task UploadValidRp14aAsync()
        => _scenarioCoordinator.UploadValidRp14aAsync();

    public Task UploadRp14aWithCaseReferenceAsync(params string?[] caseReferences)
        => _scenarioCoordinator.UploadRp14aWithCaseReferenceAsync(caseReferences);

    public Task UploadRp14aWithEmployerNameAsync(params string?[] employerNames)
        => _scenarioCoordinator.UploadRp14aWithEmployerNameAsync(employerNames);

    public Task UploadRp14aWithEmployerNameLengthAsync(int length)
        => _scenarioCoordinator.UploadRp14aWithEmployerNameLengthAsync(length);

    public Task UploadRp14aWithEmployeeNameAsync(
        string? surname,
        string? forename,
        string? title = null)
        => _scenarioCoordinator.UploadRp14aWithEmployeeNameAsync(surname, forename, title);

    public Task UploadRp14aWithEmployeeBasicPayPerWeekAsync(string? basicPayPerWeek)
        => _scenarioCoordinator.UploadRp14aWithEmployeeBasicPayPerWeekAsync(basicPayPerWeek);

    public Task UploadRp14aWithArrearsOfPayOwedAsync(string? arrearsOfPay)
        => _scenarioCoordinator.UploadRp14aWithArrearsOfPayOwedAsync(arrearsOfPay);

    public Task UploadRp14aWithInvalidArrearsOfPayOwedAsync(int count)
        => _scenarioCoordinator.UploadRp14aWithInvalidArrearsOfPayOwedAsync(count);

    public Task UploadRp14aWithNationalInsuranceNumberAsync(string? insuranceNumber, int occurrenceIndex)
        => _scenarioCoordinator.UploadRp14aWithNationalInsuranceNumberAsync(insuranceNumber, occurrenceIndex);

    public Task UploadRp14aWithMoneyOwedToEmployerAsync(string? moneyOwed)
        => _scenarioCoordinator.UploadRp14aWithMoneyOwedToEmployerAsync(moneyOwed);

    public Task UploadRp14aWithEmploymentDatesAsync(DateOnly? startDate, DateOnly? endDate)
        => _scenarioCoordinator.UploadRp14aWithEmploymentDatesAsync(startDate, endDate);

    public Task UploadRp14aWithArrearsDatesAsync(DateOnly? startDate, DateOnly? endDate)
        => _scenarioCoordinator.UploadRp14aWithArrearsDatesAsync(startDate, endDate);

    public Task UploadRp14aWithHolidayContractedEntitlementDaysAsync(string? entitlementDays)
        => _scenarioCoordinator.UploadRp14aWithHolidayContractedEntitlementDaysAsync(entitlementDays);

    public Task UploadRp14aWithHolidayDaysCarriedForwardAsync(string? daysCarriedForward)
        => _scenarioCoordinator.UploadRp14aWithHolidayDaysCarriedForwardAsync(daysCarriedForward);

    public Task UploadRp14aWithHolidayDaysTakenAsync(string? holidayDaysTaken)
        => _scenarioCoordinator.UploadRp14aWithHolidayDaysTakenAsync(holidayDaysTaken);

    public Task UploadRp14aWithHolidayOwedAsync(string? holidayOwed)
        => _scenarioCoordinator.UploadRp14aWithHolidayOwedAsync(holidayOwed);

    public Task UploadRp14aWithHolidayNotPaidDatesAsync(DateOnly? startDate, DateOnly? endDate)
        => _scenarioCoordinator.UploadRp14aWithHolidayNotPaidDatesAsync(startDate, endDate);

    public Task UploadRp14aWithMissingEmployeeSurnamesAsync(int employeeCount)
        => _scenarioCoordinator.UploadRp14aWithMissingEmployeeSurnamesAsync(employeeCount);

    public Task UploadRp14aWithInvalidHolidayOwedForEmployeesAsync(int employeeCount, params string[] invalidValues)
        => _scenarioCoordinator.UploadRp14aWithInvalidHolidayOwedForEmployeesAsync(employeeCount, invalidValues);

    public Task UploadRp14aWithNationalInsuranceNumberForEmployeesAsync(int employeeCount, string? nationalInsuranceNumber)
        => _scenarioCoordinator.UploadRp14aWithNationalInsuranceNumberForEmployeesAsync(employeeCount, nationalInsuranceNumber);

    public Task UploadRp14aWithMoneyOwedToEmployerForEmployeesAsync(int employeeCount, string? moneyOwed)
        => _scenarioCoordinator.UploadRp14aWithMoneyOwedToEmployerForEmployeesAsync(employeeCount, moneyOwed);

    public Task UploadRp14aWithEmployeeBasicPayPerWeekForEmployeesAsync(int employeeCount, string? basicPayPerWeek)
        => _scenarioCoordinator.UploadRp14aWithEmployeeBasicPayPerWeekForEmployeesAsync(employeeCount, basicPayPerWeek);

    public Task UploadComplexRp14aScenarioAsync(
        string? caseReference = null,
        string? employerName = null,
        string? surname = null,
        string? forename = null,
        string? title = null,
        string? nationalInsuranceNumber = null,
        string? moneyOwedToEmployer = null,
        string? arrearsAmount = null,
        string? basicPayPerWeek = null,
        string? holidayOwed = null,
        DateOnly? employmentStartDate = null,
        DateOnly? employmentEndDate = null)
        => _scenarioCoordinator.UploadComplexRp14aScenarioAsync(
            caseReference,
            employerName,
            surname,
            forename,
            title,
            nationalInsuranceNumber,
            moneyOwedToEmployer,
            arrearsAmount,
            basicPayPerWeek,
            holidayOwed,
            employmentStartDate,
            employmentEndDate);

    // RP14 scenarios

    public Task UploadValidRp14Async()
        => _rp14ScenarioCoordinator.UploadValidRp14Async();

    public Task UploadRp14WithCaseReferenceAsync(string? caseReference)
        => _rp14ScenarioCoordinator.UploadRp14WithCaseReferenceAsync(caseReference);

    public Task UploadRp14WithBusinessNameAsync(string? businessName)
        => _rp14ScenarioCoordinator.UploadRp14WithBusinessNameAsync(businessName);

    public Task UploadRp14WithCompanyNumberAsync(string? companyNumber)
        => _rp14ScenarioCoordinator.UploadRp14WithCompanyNumberAsync(companyNumber);

    public Task UploadRp14WithIncorporationDateAsync(DateOnly? incorporationDate)
        => _rp14ScenarioCoordinator.UploadRp14WithIncorporationDateAsync(incorporationDate);

    public Task UploadRp14WithPayeAsync(string? district, string? reference)
        => _rp14ScenarioCoordinator.UploadRp14WithPayeAsync(district, reference);

    public Task UploadRp14WithNatureOfBusinessAsync(string? natureOfBusiness)
        => _rp14ScenarioCoordinator.UploadRp14WithNatureOfBusinessAsync(natureOfBusiness);

    public Task UploadRp14WithStandardIndustrialClassificationAsync(string? standardIndustrialClassification)
        => _rp14ScenarioCoordinator.UploadRp14WithStandardIndustrialClassificationAsync(standardIndustrialClassification);

    public Task UploadRp14WithDirectorAsync(
        int directorNumber,
        string? surname,
        string? initials,
        string? nino)
        => _rp14ScenarioCoordinator.UploadRp14WithDirectorAsync(directorNumber, surname, initials, nino);

    public Task UploadRp14WithDirectorsAsync(
        int directorCount,
        string? surname,
        string? initials,
        string? nino)
        => _rp14ScenarioCoordinator.UploadRp14WithDirectorsAsync(directorCount, surname, initials, nino);

    public Task UploadRp14WithDirectorSurnamesAsync(int directorCount, string surname)
        => _rp14ScenarioCoordinator.UploadRp14WithDirectorSurnamesAsync(directorCount, surname);

    public Task UploadRp14WithShareholderAsync(
        int shareholderNumber,
        string? fullName,
        string? numberOfShares,
        string? percentage)
        => _rp14ScenarioCoordinator.UploadRp14WithShareholderAsync(shareholderNumber, fullName, numberOfShares, percentage);

    public Task UploadRp14WithShareholdersAsync(
        int shareholderCount,
        string? fullName,
        string? numberOfShares,
        string? percentage)
        => _rp14ScenarioCoordinator.UploadRp14WithShareholdersAsync(shareholderCount, fullName, numberOfShares, percentage);

    public Task UploadRp14WithNoOfEmployeesAsync(string? noOfEmployees)
        => _rp14ScenarioCoordinator.UploadRp14WithNoOfEmployeesAsync(noOfEmployees);

    public Task UploadRp14WithInsolvencyDetailsAsync(DateOnly? insolvencyDate, string? insolvencyType)
        => _rp14ScenarioCoordinator.UploadRp14WithInsolvencyDetailsAsync(insolvencyDate, insolvencyType);

    public Task UploadRp14WithTransferDetailsAsync(
        string? transferType,
        string? transferToName,
        DateOnly? transferDate,
        DateOnly? negotiationDate)
        => _rp14ScenarioCoordinator.UploadRp14WithTransferDetailsAsync(transferType, transferToName, transferDate, negotiationDate);

    public Task UploadRp14WithIpDetailsAsync(
        string? registrationNumber,
        string? firmName,
        string? ipName,
        string? emailAddress,
        string? telephoneNumber = null)
        => _rp14ScenarioCoordinator.UploadRp14WithIpDetailsAsync(registrationNumber, firmName, ipName, emailAddress, telephoneNumber);

    public Task UploadRp14WithIpNameAsync(string? ipName)
        => _rp14ScenarioCoordinator.UploadRp14WithIpNameAsync(ipName);

    public Task UploadRp14WithIpRegistrationNumberAsync(string? registrationNumber)
        => _rp14ScenarioCoordinator.UploadRp14WithIpRegistrationNumberAsync(registrationNumber);

    public Task UploadRp14WithIpFirmNameAsync(string? firmName)
        => _rp14ScenarioCoordinator.UploadRp14WithIpFirmNameAsync(firmName);

    public Task UploadRp14WithIpEmailAddressAsync(string? emailAddress)
        => _rp14ScenarioCoordinator.UploadRp14WithIpEmailAddressAsync(emailAddress);

    public Task UploadRp14WithIpTelephoneNumberAsync(string? telephoneNumber)
        => _rp14ScenarioCoordinator.UploadRp14WithIpTelephoneNumberAsync(telephoneNumber);

    public Task UploadRp14WithAssociatedCompanyNamesAsync(int associatedCompanyCount, string companyName)
        => _rp14ScenarioCoordinator.UploadRp14WithAssociatedCompanyNamesAsync(associatedCompanyCount, companyName);

    public Task UploadRp14WithAssociatedCompanyReasonsAsync(int associatedCompanyCount, string reason)
        => _rp14ScenarioCoordinator.UploadRp14WithAssociatedCompanyReasonsAsync(associatedCompanyCount, reason);

    public Task UploadRp14WithAssociatedCompanyNumbersAsync(int associatedCompanyCount, string companyNumber)
        => _rp14ScenarioCoordinator.UploadRp14WithAssociatedCompanyNumbersAsync(associatedCompanyCount, companyNumber);

    public Task UploadRp14WithEmploymentContinuityEmployerNameAsync(string? employerName)
        => _rp14ScenarioCoordinator.UploadRp14WithEmploymentContinuityEmployerNameAsync(employerName);

    public Task UploadRp14WithTransferToNameAsync(string? transferToName)
        => _rp14ScenarioCoordinator.UploadRp14WithTransferToNameAsync(transferToName);

    public Task UploadRp14WithPayRecordsContactNameAsync(string? name)
        => _rp14ScenarioCoordinator.UploadRp14WithPayRecordsContactNameAsync(name);

    public Task UploadRp14WithPayRecordsContactPhoneNumberAsync(string? phoneNumber)
        => _rp14ScenarioCoordinator.UploadRp14WithPayRecordsContactPhoneNumberAsync(phoneNumber);

    public Task UploadRp14WithPayRecordsContactEmailAddressAsync(string? emailAddress)
        => _rp14ScenarioCoordinator.UploadRp14WithPayRecordsContactEmailAddressAsync(emailAddress);

    public Task UploadRp14WithCompanyAddressLine1Async(string? addressLine)
        => _rp14ScenarioCoordinator.UploadRp14WithCompanyAddressLine1Async(addressLine);

    public Task UploadRp14WithCompanyAddressFieldAsync(Rp14AddressField field, string? value)
        => _rp14ScenarioCoordinator.UploadRp14WithCompanyAddressFieldAsync(field, value);

    public Task UploadRp14WithCompanyAddressLineCountAsync(int lineCount)
        => _rp14ScenarioCoordinator.UploadRp14WithCompanyAddressLineCountAsync(lineCount);

    public Task UploadRp14WithCompanyAddressLinesCountAsync(int lineCount)
        => _rp14ScenarioCoordinator.UploadRp14WithCompanyAddressLinesCountAsync(lineCount);
}
