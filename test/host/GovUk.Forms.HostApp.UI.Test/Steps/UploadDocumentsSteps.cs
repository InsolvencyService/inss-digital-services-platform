using GovUk.Forms.HostApp.UI.Test.Coordinators;
using GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;
using GovUk.Forms.HostApp.UI.Test.Models.TestData;
using GovUk.Forms.HostApp.UI.Test.Support;

namespace GovUk.Forms.HostApp.UI.Test.Steps;

[Scope(Feature = "Upload Documents")]
[Binding]
public class UploadDocumentsSteps
{
    private readonly UploadDocumentCoordinator _uploadDocumentCoordinator;
    private readonly ScenarioContext _scenarioContext;
    private readonly CheckYourAnswersCoordinator _checkYourAnswersCoordinator;

    public UploadDocumentsSteps(
        UploadDocumentCoordinator uploadDocumentCoordinator,
        ScenarioContext scenarioContext,
        CheckYourAnswersCoordinator checkYourAnswersCoordinator)
    {
        _uploadDocumentCoordinator = uploadDocumentCoordinator;
        _scenarioContext = scenarioContext;
        _checkYourAnswersCoordinator = checkYourAnswersCoordinator;
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

    [Given(@"an empty RP14A XML file with a size of 0 bytes")]
    public void GivenAnEmptyRp14aXmlFileWithASizeOf0Bytes()
    {
        string filePath = _uploadDocumentCoordinator.PrepareEmptyRp14aFile();
        _scenarioContext.Set(filePath, ScenarioConstant.UploadedFilePath);
    }

    [When(@"I upload the file")]
    public async Task WhenIUploadTheFile()
    {
        string filePath = _scenarioContext.Get<string>(ScenarioConstant.UploadedFilePath);
        await _uploadDocumentCoordinator.UploadFileAsync(filePath);
        await _uploadDocumentCoordinator.ClickOnContinueButtonAsync();
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

    [When("I upload a valid RP14 XML file")]
    public async Task WhenIUploadAValidRP14XMLFile()
    {
        await _uploadDocumentCoordinator.UploadValidRp14Async();
    }

    [Then("the file list should contain only one instance of that file")]
    public async Task ThenTheFileListShouldContainOnlyOneInstanceOfThatFile()
    {
        await _uploadDocumentCoordinator.VerifyOnlyOneFileUploadedAsync();
    }

    [Then("the upload document page should match the visual snapshot")]
    public async Task ThenTheUploadDocumentPageShouldMatchTheVisualSnapshot()
    {
        await _uploadDocumentCoordinator.VerifyUploadDocumentContentSnapShotAsync();
    }


    [Then("the common issues section should display the correct content")]
    public async Task ThenTheCommonIssuesSectionShouldDisplayTheCorrectContent()
    {
        await _uploadDocumentCoordinator.VerifyUploadCommonIssuesContentVisualSnapShotAsync();
    }

    [Then("I should see the upload error message {string}")]
    [Then("I should see the file upload error {string}")]
    [Then("I should see the error message {string}")]
    public async Task ThenIShouldSeeTheFileUploadError(string errorMessage)
    {
        await _uploadDocumentCoordinator.VerifyInvalidFileExtensionErrorAsync(
        new UploadFileError(
        SummaryTitle: "There is a problem",
        ErrorMessage: errorMessage));
    }

    [Then("the file should not be uploaded")]
    [Then("the upload should be rejected")]
    public async Task ThenTheFileShouldNotBeUploaded()
    {
        await _uploadDocumentCoordinator.VerifyUploadDocumentPageIsDisplayedAsync();
    }

    [Then("no validation errors should be displayed")]
    public async Task ThenNoValidationErrorsShouldBeDisplayed()
    {
        await _uploadDocumentCoordinator.VerifyOnlyOneFileUploadedAsync();
        await _uploadDocumentCoordinator.ClickOnContinueButtonAsync();
        await _checkYourAnswersCoordinator.VerifyCheckYourAnswersPageIsDisplayedAsync();
    }

}
