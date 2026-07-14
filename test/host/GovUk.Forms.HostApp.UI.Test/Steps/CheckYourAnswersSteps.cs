using GovUk.Forms.HostApp.UI.Test.Coordinators;
using GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;
using GovUk.Forms.HostApp.UI.Test.Factories;
using GovUk.Forms.HostApp.UI.Test.Models;

namespace GovUk.Forms.HostApp.UI.Test.Steps;

[Binding]
public class CheckYourAnswersSteps
{
    private const string UserTypeKey = "UserType";

    private readonly CommonCoordinator _commonCoordinator;
    private readonly CheckYourAnswersCoordinator _checkYourAnswersCoordinator;
    private readonly UploadDocumentCoordinator _uploadDocumentCoordinator;
    private readonly CaseReferenceCoordinator _caseReferenceCoordinator;
    private readonly StartPageCoordinator _startPageCoordinator;
    private readonly SignInCoordinator _signInCoordinator;
    private readonly DeclarationCoordinator _declarationCoordinator;
    private readonly ScenarioContext _scenarioContext;

    public CheckYourAnswersSteps(
        CommonCoordinator commonCoordinator,
        CheckYourAnswersCoordinator checkYourAnswersCoordinator,
        UploadDocumentCoordinator uploadDocumentCoordinator,
        CaseReferenceCoordinator caseReferenceCoordinator,
        StartPageCoordinator startPageCoordinator,
        SignInCoordinator signInCoordinator,
        DeclarationCoordinator declarationCoordinator,
        ScenarioContext scenarioContext)
    {
        _commonCoordinator = commonCoordinator;
        _checkYourAnswersCoordinator = checkYourAnswersCoordinator;
        _uploadDocumentCoordinator = uploadDocumentCoordinator;
        _caseReferenceCoordinator = caseReferenceCoordinator;
        _startPageCoordinator = startPageCoordinator;
        _signInCoordinator = signInCoordinator;
        _declarationCoordinator = declarationCoordinator;
        _scenarioContext = scenarioContext;
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

    [Then("I should be returned to the case reference page")]
    public async Task ThenIShouldBeReturnedToTheCaseReferencePage()
    {
        await _caseReferenceCoordinator.VerifyCaseReferencePageIsDisplayedAsync();
    }

    [Then("the RP14A form should be successfully submitted")]
    public async Task ThenTheRPAFormShouldBeSuccessfullySubmitted()
    {
        await _checkYourAnswersCoordinator.VerifySubmitCompletedPageIsDisplayedAsync();
    }

    [Given("I have successfully submitted an RP14 form")]
    public async Task GivenIHaveSuccessfullySubmittedAnRP14Form()
    {
        await _commonCoordinator.VerifySubmissionConfirmationPageIsDisplayedForRp14Async();
    }

    [Given("I am logged out")]
    public async Task GivenIAmLoggedOut()
    {
        await _commonCoordinator.SignOutAndVerifyAsync();
    }

    [When("I sign in")]
    public async Task WhenISignIn()
    {
        string userType = _scenarioContext.TryGetValue(UserTypeKey, out string? storedUserType)
            ? storedUserType!
            : "Default";

        TestUser user = UserFactory.GetUser(userType);

        await _startPageCoordinator.NavigateToLoginPageAsync();
        await _signInCoordinator.SignInAsync(user.Email, user.Password);
    }

    [Then("I should be taken to the declaration page")]
    public async Task ThenIShouldBeTakenToTheDeclarationPage()
    {
        await _declarationCoordinator.VerifyDeclarationPageIsDisplayedAsync();
    }

    [Then("I should be able to upload a new RP14 form")]
    public async Task ThenIShouldBeAbleToUploadANewRP14Form()
    {
        await _commonCoordinator.NavigateToUploadPageAndUploadRp14Async();
    }

    [Given("I have uploaded an RP14 document")]
    public async Task GivenIHaveUploadedAnRP14Document()
    {
        await _commonCoordinator.UploadAValidRP14AndVerifyAsync();
    }

    [Then("I should be able to view the uploaded RP14 document")]
    public async Task ThenIShouldBeAbleToViewTheUploadedRP14Document()
    {
        await _commonCoordinator.ViewPreviouslyUploadedRp14DocumentAsync();
    }
}
