using GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;
using GovUk.Forms.HostApp.UI.Test.Support;
using static GovUk.Forms.HostApp.UI.Test.Models.TestData;

namespace GovUk.Forms.HostApp.UI.Test.Steps;

[Scope(Feature = "Upload Documents")]
[Binding]
public class UploadDocumentsSteps
{
    private readonly UploadDocumentCoordinator _uploadDocumentCoordinator;
    private readonly ScenarioContext _scenarioContext;

    public UploadDocumentsSteps(
        UploadDocumentCoordinator uploadDocumentCoordinator,
        ScenarioContext scenarioContext)
    {
        _uploadDocumentCoordinator = uploadDocumentCoordinator;
        _scenarioContext = scenarioContext;
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

    [When("I proceed to the check answers page")]
    [When("I click the continue button without selecting a file")]
    public async Task WhenIClickTheContinueButtonWithoutSelectingAFile()
    {
        await _uploadDocumentCoordinator.ClickOnContinueButtonAsync();
    }

    [When(@"I upload an unsupported file with extension ""([^""]*)""")]
    public async Task WhenIUploadAnUnsupportedFileWithExtension(string extension)
    {
        await _uploadDocumentCoordinator.UploadUnsupportedFileAsync(extension);
    }

    [When(@"I upload an XML file with invalid RP14A content")]
    public async Task WhenIUploadAnXmlFileThatDoesNotMatchTheExpectedRp14AStructure()
    {
        await _uploadDocumentCoordinator
            .UploadXmlFileWithWrongContentAsync();
    }

    [When(@"I upload an XML file larger than the maximum allowed size")]
    public async Task WhenIUploadAnXmlFileLargerThanTheMaximumAllowedSize()
    {
        await _uploadDocumentCoordinator
            .UploadValidXmlFileAboveMaximumSizeAsync();
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

    [Then("I should see the upload error message {string}")]
    [Then("I should see the file upload error {string}")]
    public async Task ThenIShouldSeeTheFileUploadError(string errorMessage)
    {
        await _uploadDocumentCoordinator.VerifyInvalidFileExtensionErrorAsync(
        new UploadFileError(
        SummaryTitle: "There is a problem",
        ErrorMessage: errorMessage));
    }

    [Then("the file should not be uploaded")]
    public async Task ThenTheFileShouldNotBeUploaded()
    {
        await _uploadDocumentCoordinator.VerifyUploadDocumentPageIsDisplayedAsync();
    }

}
