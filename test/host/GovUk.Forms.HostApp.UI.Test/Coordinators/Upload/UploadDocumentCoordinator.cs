using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Factories;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Models.TestData;
using GovUk.Forms.HostApp.UI.Test.Pages.Common;
using GovUk.Forms.HostApp.UI.Test.Support;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;

public sealed class UploadDocumentCoordinator :
    BaseCoordinator,
    IUploadPageCoordinator,
    IFileUploadCoordinator,
    IRp14aScenarioCoordinator,
    IUploadVerificationCoordinator,
    IUploadNavigationCoordinator
{
    private readonly IUploadPageCoordinator _pageCoordinator;
    private readonly IFileUploadCoordinator _fileUploadCoordinator;
    private readonly IRp14aScenarioCoordinator _scenarioCoordinator;
    private readonly IUploadVerificationCoordinator _verificationCoordinator;
    private readonly IUploadNavigationCoordinator _navigationCoordinator;
    private readonly ICommonPage _commonPage;
    private readonly IPlaywrightDriver _playwrightDriver;

    public UploadDocumentCoordinator(
        TestArtifacts testArtifacts,
        IUploadPageCoordinator pageCoordinator,
        IFileUploadCoordinator fileUploadCoordinator,
        IRp14aScenarioCoordinator scenarioCoordinator,
        IUploadVerificationCoordinator verificationCoordinator,
        IUploadNavigationCoordinator navigationCoordinator,
        ICommonPage commonPage,
        IPlaywrightDriver playwrightDriver)
        : base(testArtifacts)
    {
        _pageCoordinator = pageCoordinator
            ?? throw new ArgumentNullException(nameof(pageCoordinator));

        _fileUploadCoordinator = fileUploadCoordinator
            ?? throw new ArgumentNullException(nameof(fileUploadCoordinator));

        _scenarioCoordinator = scenarioCoordinator
            ?? throw new ArgumentNullException(nameof(scenarioCoordinator));

        _verificationCoordinator = verificationCoordinator
            ?? throw new ArgumentNullException(nameof(verificationCoordinator));

        _navigationCoordinator = navigationCoordinator
            ?? throw new ArgumentNullException(nameof(navigationCoordinator));

        _commonPage = commonPage
            ?? throw new ArgumentNullException(nameof(commonPage));

        _playwrightDriver = playwrightDriver
            ?? throw new ArgumentNullException(nameof(playwrightDriver));
    }

    public async Task VerifyUploadDocumentPageIsDisplayedAsync()
        => await _pageCoordinator.VerifyUploadDocumentPageIsDisplayedAsync();

    public async Task ExpandCommonIssuesWhenUploadingRP14AFormsAsync()
        => await _pageCoordinator.ExpandCommonIssuesWhenUploadingRP14AFormsAsync();

    public async Task<string> CaptureUploadDocumentPageVisualAsync()
    {
        return await CapturePageVisualAsync(
            () => _commonPage.CaptureVisualAsync(_playwrightDriver.Page),
            ScenarioConstant.UploadPage);
    }

    public async Task UploadFileAsync(string filePath)
        => await _fileUploadCoordinator.UploadFileAsync(filePath);

    public async Task UploadValidRp14aAsync()
        => await _scenarioCoordinator.UploadValidRp14aAsync();

    public async Task UploadRp14aWithCaseReferenceAsync(string? caseReference)
        => await _scenarioCoordinator.UploadRp14aWithCaseReferenceAsync(caseReference);
    public async Task UploadRp14aWithEmployerNameAsync(string? employerName)
        => await _scenarioCoordinator.UploadRp14aWithEmployerNameAsync(employerName);

    public async Task UploadRp14aWithEmployerNameLengthAsync(int length)
        => await _scenarioCoordinator.UploadRp14aWithEmployerNameLengthAsync(length);

    public async Task UploadRp14aWithEmployeeNameAsync(
        string? surname,
        string? forename,
        string? title = null)
        => await _scenarioCoordinator.UploadRp14aWithEmployeeNameAsync(
            surname,
            forename,
            title);

    public async Task UploadRp14aWithEmployeeBasicPayPerWeekAsync(string? basicPayPerWeek)
        => await _scenarioCoordinator.UploadRp14aWithEmployeeBasicPayPerWeekAsync(basicPayPerWeek);
    public async Task UploadRp14aWithArrearsOfPayOwedAsync(string? arrearsOfPay)
        => await _scenarioCoordinator.UploadRp14aWithArrearsOfPayOwedAsync(arrearsOfPay);
    public async Task UploadRp14aWithInvalidArrearsOfPayOwedAsync(int count)
        => await _scenarioCoordinator.UploadRp14aWithInvalidArrearsOfPayOwedAsync(count);
    public async Task UploadRp14aWithNationalInsuranceNumberAsync(string? insuranceNumber, int occurrenceIndex)
        => await _scenarioCoordinator.UploadRp14aWithNationalInsuranceNumberAsync(insuranceNumber, occurrenceIndex);
    public async Task UploadRp14aWithMoneyOwedToEmployerAsync(string? moneyOwed)
        => await _scenarioCoordinator.UploadRp14aWithMoneyOwedToEmployerAsync(moneyOwed);
    public async Task UploadRp14aWithEmploymentDatesAsync(DateOnly? startDate, DateOnly? endDate)
        => await _scenarioCoordinator.UploadRp14aWithEmploymentDatesAsync(startDate, endDate);
    public async Task UploadRp14aWithArrearsDatesAsync(DateOnly? startDate, DateOnly? endDate)
        => await _scenarioCoordinator.UploadRp14aWithArrearsDatesAsync(startDate, endDate);

    public async Task UploadUnsupportedFileAsync(string extension)
    {
        string filePath = TestFileFactory.CreateUnsupportedFile(
            TestArtifacts,
            extension);

        await UploadFileAsync(filePath);
    }

    public async Task UploadXmlFileWithWrongContentAsync()
    {
        string filePath = TestFileFactory.CreateXmlWithWrongContent(
            TestArtifacts);

        await UploadFileAsync(filePath);
    }

    public async Task UploadValidXmlFileAtMaximumSizeAsync()
    {
        string filePath = TestFileFactory.CreateValidXmlFileAtSize(
            TestArtifacts,
            10);

        await UploadFileAsync(filePath);
    }

    public async Task UploadValidXmlFileAboveMaximumSizeAsync()
    {
        string filePath = TestFileFactory.CreateValidXmlFileAboveSize(
            TestArtifacts,
            10);

        await UploadFileAsync(filePath);
    }

    public async Task VerifyThatFileIsUploadedAsync()
        => await _verificationCoordinator.VerifyThatFileIsUploadedAsync();

    public async Task VerifyOnlyOneFileUploadedAsync()
        => await _verificationCoordinator.VerifyOnlyOneFileUploadedAsync();

    public async Task VerifyInvalidFileExtensionErrorAsync(
        UploadFileError uploadFileError)
        => await _verificationCoordinator.VerifyInvalidFileExtensionErrorAsync(
            uploadFileError);

    public async Task ClickOnContinueButtonAsync()
        => await _navigationCoordinator.ClickOnContinueButtonAsync();

    public async Task ClickOnBackButtonAsync()
        => await _navigationCoordinator.ClickOnBackButtonAsync();

    public async Task NavigateToFeedbackPageAsync()
        => await _navigationCoordinator.NavigateToFeedbackPageAsync();

    public async Task NavigateToSubmitPageAsync()
        => await _navigationCoordinator.NavigateToSubmitPageAsync();

    public async Task UploadRp14aWithHolidayContractedEntitlementDaysAsync(string? entitlementDays)
        => await _scenarioCoordinator.UploadRp14aWithHolidayContractedEntitlementDaysAsync(entitlementDays);
    public async Task UploadRp14aWithHolidayDaysCarriedForwardAsync(string? daysCarriedForward)
    => await _scenarioCoordinator.UploadRp14aWithHolidayDaysCarriedForwardAsync(daysCarriedForward);
    public async Task UploadRp14aWithHolidayDaysTakenAsync(string? holidayDaysTaken)
    => await _scenarioCoordinator.UploadRp14aWithHolidayDaysTakenAsync(holidayDaysTaken);
    public async Task UploadRp14aWithHolidayOwedAsync(string? holidayOwed)
    => await _scenarioCoordinator.UploadRp14aWithHolidayOwedAsync(holidayOwed);
    public async Task UploadRp14aWithHolidayNotPaidDatesAsync(DateOnly? startDate, DateOnly? endDate)
    => await _scenarioCoordinator.UploadRp14aWithHolidayNotPaidDatesAsync(startDate, endDate);
    public async Task UploadRp14aWithMissingEmployeeSurnamesAsync(int employeeCount) =>
        await _scenarioCoordinator.UploadRp14aWithMissingEmployeeSurnamesAsync(employeeCount);

    public async Task UploadRp14aWithInvalidHolidayOwedForEmployeesAsync(int employeeCount, params string[] invalidValues) =>
        await _scenarioCoordinator.UploadRp14aWithInvalidHolidayOwedForEmployeesAsync(employeeCount, invalidValues);

    public async Task UploadComplexRp14aScenarioAsync(
        string? caseReference = null,
        string? employerName = null,
        string? surname = null,
        string? forename = null,
        string? title = null,
        string? arrearsAmount = null,
        string? basicPayPerWeek = null,
        string? holidayOwed = null,
        DateOnly? employmentStartDate = null,
        DateOnly? employmentEndDate = null)
    {
        await _scenarioCoordinator.UploadComplexRp14aScenarioAsync(
            caseReference,
            employerName,
            surname,
            forename,
            title,
            arrearsAmount,
            basicPayPerWeek,
            holidayOwed,
            employmentStartDate,
            employmentEndDate);
    }
}
