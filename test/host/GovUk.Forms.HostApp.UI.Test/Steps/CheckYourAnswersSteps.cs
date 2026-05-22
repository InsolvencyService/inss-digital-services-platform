using GovUk.Forms.HostApp.UI.Test.Coordinators;
using GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;

namespace GovUk.Forms.HostApp.UI.Test.Steps;

[Binding]
public class CheckYourAnswersSteps
{
    private readonly CommonCoordinator _commonCoordinator;
    private readonly CheckYourAnswersCoordinator _checkYourAnswersCoordinator;
    private readonly UploadDocumentCoordinator _uploadDocumentCoordinator;

    public CheckYourAnswersSteps(
        CommonCoordinator commonCoordinator,
        CheckYourAnswersCoordinator checkYourAnswersCoordinator,
        UploadDocumentCoordinator uploadDocumentCoordinator)
    {
        _commonCoordinator = commonCoordinator;
        _checkYourAnswersCoordinator = checkYourAnswersCoordinator;
        _uploadDocumentCoordinator = uploadDocumentCoordinator;
    }

    [Given("I am reviewing my uploaded RP14A document")]
    public async Task GivenIAmReviewingMyUploadedRP14ADocument()
    {
        await GivenIAmUploadingAnRPAForm();
        await WhenIContinueToTheCheckYourAnswersPage();
        await _checkYourAnswersCoordinator.VerifyCheckYourAnswersPageIsDisplayedAsync();
    }

    [Given("I am uploading an RP14A form")]
    public async Task GivenIAmUploadingAnRPAForm()
    {
        await _commonCoordinator.UploadAValidRP14AAndVerifyAsync();
    }

    [When("I continue to the check your answers page")]
    public async Task WhenIContinueToTheCheckYourAnswersPage()
    {
        await _uploadDocumentCoordinator.ClickOnContinueButtonAsync();
    }

    [When("I choose to change the uploaded document")]
    public async Task WhenIChooseToChangeTheUploadedDocument()
    {
        await _checkYourAnswersCoordinator.ChangeUploadedDocumentAsync();
    }

    [When("I navigate back from the review page")]
    public async Task WhenINavigateBackFromTheReviewPage()
    {
        await _checkYourAnswersCoordinator.NavigateBackAsync();
    }

    [When("I submit the RP14A form")]
    public async Task WhenISubmitTheRPAForm()
    {
        await _checkYourAnswersCoordinator.ClickOnSubmitButtonAsync();
    }

    [Then("I should be able to review my uploaded document")]
    public async Task ThenIShouldBeAbleToReviewMyUploadedDocument()
    {
        await _checkYourAnswersCoordinator.VerifyCheckYourAnswersPageIsDisplayedAsync();
    }

    [Then("I should be returned to the document upload page")]
    public async Task ThenIShouldBeReturnedToTheDocumentUploadPage()
    {
        await _uploadDocumentCoordinator.VerifyUploadDocumentPageIsDisplayedAsync();
    }

    [Then("the RP14A form should be successfully submitted")]
    public async Task ThenTheRPAFormShouldBeSuccessfullySubmitted()
    {
        await _checkYourAnswersCoordinator.VerifySubmitCompletedPageIsDisplayedAsync();
    }
}
