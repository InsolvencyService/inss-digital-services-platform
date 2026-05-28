using GovUk.Forms.HostApp.UI.Test.Coordinators;

namespace GovUk.Forms.HostApp.UI.Test.Steps;

[Binding]
public class SubmissionConfirmationSteps
{
    private readonly CommonCoordinator _commonCoordinator;
    private readonly SubmissionConfirmationCoordinator _submissionConfirmationCoordinator;
    private readonly DeclarationCoordinator _declarationCoordinator;
    private readonly CheckYourAnswersCoordinator _checkYourAnswersCoordinator;
    public SubmissionConfirmationSteps(
        CommonCoordinator commonCoordinator,
        SubmissionConfirmationCoordinator submissionConfirmationCoordinator,
        DeclarationCoordinator declarationCoordinator,
        CheckYourAnswersCoordinator checkYourAnswersCoordinator)
    {
        _commonCoordinator = commonCoordinator;
        _submissionConfirmationCoordinator = submissionConfirmationCoordinator;
        _declarationCoordinator = declarationCoordinator;
        _checkYourAnswersCoordinator = checkYourAnswersCoordinator;
    }

    [Given("I am on the submission confirmation page")]
    public async Task GivenIAmOnTheSubmissionConfirmationPage()
    {
        await _commonCoordinator.VerifySubmissionComfirmationPageIsDisplayedAsync();
    }

    [Given("I have selected Upload another form")]
    public async Task GivenIHaveSelectedUploadAnotherForm()
    {
        await GivenIAmOnTheSubmissionConfirmationPage();
        await WhenISelectToUploadAnotherForm();
        await ThenIWillBeTakenToTheDeclarationPage();
    }

    [Given("I have agreed to the declaration")]
    public async Task GivenIHaveAgreedToTheDeclaration()
    {
        await _declarationCoordinator.NavigateToUploadAFilePageAsync();
    }

    [When("I select to Upload another form")]
    public async Task WhenISelectToUploadAnotherForm()
    {
        await _submissionConfirmationCoordinator.UploadAnotherFormAsync();
    }

    [When("I upload another RP14A form")]
    public async Task WhenIUploadAnotherRPAForm()
    {
        await _commonCoordinator.VerifyThatCheckYourAnswersPageIsDisplayedAsync();
    }

    [Then("I will be taken to the declaration page")]
    public async Task ThenIWillBeTakenToTheDeclarationPage()
    {
        await _declarationCoordinator.VerifyDeclarationPageIsDisplayedAsync();
    }

    [Then("I will be able to upload a new RP14A form")]
    public async Task ThenIWillBeAbleToUploadANewRPAForm()
    {
        await _checkYourAnswersCoordinator.VerifyCheckYourAnswersPageIsDisplayedAsync();
        await _checkYourAnswersCoordinator.ClickOnSubmitButtonAsync();
        await _submissionConfirmationCoordinator.VerifySubmissionConfirmationPageIsDisplayedAsync();
    }


}
