using GovUk.Forms.HostApp.UI.Test.Builders;
using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Pages.Common;
using GovUk.Forms.HostApp.UI.Test.Support;
using static GovUk.Forms.HostApp.UI.Test.Models.TestData;

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
        IPlaywrightDriver playwrightDriver) : base(testArtifacts)
    {
        _pageCoordinator = pageCoordinator;
        _fileUploadCoordinator = fileUploadCoordinator;
        _scenarioCoordinator = scenarioCoordinator;
        _verificationCoordinator = verificationCoordinator;
        _navigationCoordinator = navigationCoordinator;
        _commonPage = commonPage;
        _playwrightDriver = playwrightDriver;
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
        => await _fileUploadCoordinator.UploadValidRp14aAsync();

    public async Task UploadRp14aAsync(Rp14aScenarioBuilder scenarioBuilder)
        => await _fileUploadCoordinator.UploadRp14aAsync(scenarioBuilder);

    public async Task UploadRp14aWithCaseReferenceAsync(string? caseReference)
        => await _scenarioCoordinator.UploadRp14aWithCaseReferenceAsync(caseReference);

    public async Task UploadRp14aWithEmployerNameAsync(string? employerName)
        => await _scenarioCoordinator.UploadRp14aWithEmployerNameAsync(employerName);

    public async Task UploadRp14aWithEmployeeNameAsync(
        string? surname,
        string? forename,
        string title = "Ms")
        => await _scenarioCoordinator.UploadRp14aWithEmployeeNameAsync(
            surname,
            forename,
            title);

    public async Task UploadRp14aWithEmployerNameLengthAsync(int length)
        => await _scenarioCoordinator.UploadRp14aWithEmployerNameLengthAsync(length);

    public async Task UploadRp14aWithArrearsOfPayOwedAsync(string? arrearsOfPay)
        => await _scenarioCoordinator.UploadRp14aWithArrearsOfPayOwedAsync(arrearsOfPay);

    public async Task UploadRp14aWithInvalidArrearsOfPayOwedAsync(int count)
        => await _scenarioCoordinator.UploadRp14aWithInvalidArrearsOfPayOwedAsync(count);

    public async Task UploadRp14aWithNationalInsuranceNumberAsync(string? insuranceNumber)
        => await _scenarioCoordinator.UploadRp14aWithNationalInsuranceNumberAsync(insuranceNumber);

    public async Task UploadRp14aWithMoneyOwedToEmployerAsync(string? moneyOwed)
        => await _scenarioCoordinator.UploadRp14aWithMoneyOwedToEmployerAsync(moneyOwed);

    public async Task UploadRp14aWithEmploymentDatesAsync(
        string? startDate,
        string? endDate)
        => await _scenarioCoordinator.UploadRp14aWithEmploymentDatesAsync(
            startDate,
            endDate);

    public async Task UploadRp14aWithArrearsDatesAsync(
        string? startDate,
        string? endDate)
        => await _scenarioCoordinator.UploadRp14aWithArrearsDatesAsync(
            startDate,
            endDate);

    public async Task UploadComplexRp14aScenarioAsync(
        string employerName,
        string surname,
        string forename,
        string arrearsAmount,
        string employmentStartDate,
        string employmentEndDate)
        => await _scenarioCoordinator.UploadComplexRp14aScenarioAsync(
            employerName,
            surname,
            forename,
            arrearsAmount,
            employmentStartDate,
            employmentEndDate);

    public async Task VerifyThatFileIsUploadedAsync()
        => await _verificationCoordinator.VerifyThatFileIsUploadedAsync();

    public async Task VerifyOnlyOneFileUploadedAsync()
        => await _verificationCoordinator.VerifyOnlyOneFileUploadedAsync();

    public async Task VerifyInvalidFileExtensionErrorAsync(UploadFileError uploadFileError)
        => await _verificationCoordinator.VerifyInvalidFileExtensionErrorAsync(uploadFileError);

    public async Task ClickOnContinueButtonAsync()
        => await _navigationCoordinator.ClickOnContinueButtonAsync();

    public async Task ClickOnBackButtonAsync()
        => await _navigationCoordinator.ClickOnBackButtonAsync();

    public async Task NavigateToFeedbackPageAsync()
        => await _navigationCoordinator.NavigateToFeedbackPageAsync();

    public async Task NavigateToSubmitPageAsync()
        => await _navigationCoordinator.NavigateToSubmitPageAsync();
}
