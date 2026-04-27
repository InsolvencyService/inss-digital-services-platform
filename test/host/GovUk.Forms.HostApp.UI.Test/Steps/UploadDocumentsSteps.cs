using GovUk.Forms.HostApp.UI.Test.Coordinators;
using GovUk.Forms.HostApp.UI.Test.Support;

namespace GovUk.Forms.HostApp.UI.Test.Steps;

[Scope(Feature = "Upload Documents")]
[Binding]
public class UploadDocumentsSteps
{
    private readonly UploadDocumentCoordinator _uploadDocumentCoordinator;
    private readonly CommonCoordinator _commonCoordinator;
    private readonly ScenarioContext _scenarioContext;

    public UploadDocumentsSteps(
        UploadDocumentCoordinator uploadDocumentCoordinator,
        CommonCoordinator commonCoordinator,
        ScenarioContext scenarioContext)
    {
        _uploadDocumentCoordinator = uploadDocumentCoordinator;
        _commonCoordinator = commonCoordinator;
        _scenarioContext = scenarioContext;
    }

    [Given("I am on the upload page")]
    public async Task GivenIAmOnTheUploadPage()
    {
        await _commonCoordinator.VerifyThatUploadDocumentPageIsDisplayedAsync();
    }

    [When("I upload a valid RP14A file")]
    public async Task WhenIUploadAValidFile()
    {
        await _uploadDocumentCoordinator.UploadValidRp14aAsync();
    }

    [When("I upload the same file again")]
    public async Task WhenIUploadTheSameFileAgain()
    {
        string filePath = _scenarioContext.Get<string>(ScenarioConstant.UploadedFilePath);
        await _uploadDocumentCoordinator.UploadFileAsync(filePath);
    }

    [When("I expand the common issues section")]
    public async Task WhenIExpandTheCommonIssuesSection()
    {
        await _uploadDocumentCoordinator.ExpandCommonIssuesWhenUploadingRP14AFormsAsync();
    }

    [Then("the uploaded file should appear in the file list")]
    public async Task ThenTheUploadedFileShouldAppearInTheFileList()
    {
        await _uploadDocumentCoordinator.VerifyThatFileIsUploadedAsync();
    }

    [Then("the file list should contain only one instance of that file")]
    public async Task ThenTheFileListShouldContainOnlyOneInstanceOfThatFile()
    {
        await _uploadDocumentCoordinator.VerifyOnlyOneFileUploadedAsync();
    }

    [Then("the upload document page should match the visual snapshot")]
    public async Task ThenTheUploadDocumentPageShouldMatchTheVisualSnapshot()
    {
        string screenshotPath = await _uploadDocumentCoordinator.CaptureUploadDocumentPageVisualAsync();
        await VerifyFile(screenshotPath)
            .UseDirectory(ScenarioConstant.SnapShots)
            .UseFileName(ScenarioConstant.UploadPage);
    }


    [Then("the common issues section should display the correct content")]
    public async Task ThenTheCommonIssuesSectionShouldDisplayTheCorrectContent()
    {
        string screenshotPath = await _uploadDocumentCoordinator.CaptureUploadDocumentPageVisualAsync();
        await VerifyFile(screenshotPath)
            .UseDirectory(ScenarioConstant.SnapShots)
            .UseFileName(ScenarioConstant.UploadPageWithCommonIssuesSection);
    }

}
