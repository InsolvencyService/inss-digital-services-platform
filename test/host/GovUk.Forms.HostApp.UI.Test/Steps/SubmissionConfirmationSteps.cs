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

    [When("I agree to the declaration")]
    [Given("I have agreed to the declaration")]
    public async Task GivenIHaveAgreedToTheDeclaration()
    {
        await _declarationCoordinator.NavigateToUploadAFilePageAsync();
    }

    [Then("I retrieve the second submission confirmation email")]
    [Given("I retrieve the first submission confirmation email")]
    [Given("I retrieve the submission confirmation email")]
    public async Task GivenIRetrieveTheRP14ASubmissionConfirmationEmail()
    {
        await _submissionConfirmationCoordinator.RetrieveSubmissionConfirmationEmailAsync();
    }

    [Then("the second submission confirmation email contains the submitted details")]
    [Given("the first submission confirmation email contains the submitted details")]
    [Given("the submission confirmation email contains the submitted details")]
    public async Task GivenTheSubmissionConfirmationEmailContainsTheSubmittedRP14ADetails()
    {
        await _submissionConfirmationCoordinator.VerifySubmissionConfirmationEmailContainsRP14ADetailsAsync();
    }

    [Then("the second submission confirmation email contains the submitted RP14 details")]
    [Given("the first submission confirmation email contains the submitted RP14 details")]
    public async Task GivenTheFirstSubmissionConfirmationEmailContainsTheSubmittedRPDetails()
    {
        await _submissionConfirmationCoordinator.VerifySubmissionConfirmationEmailContainsRP14DetailsAsync();
    }

    [When("I select Upload another form")]
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

    [Then("I should be able to submit the new RP14A form successfully")]
    [Then("I will be able to upload a new RP14A form")]
    public async Task ThenIWillBeAbleToUploadANewRPAForm()
    {
        await _checkYourAnswersCoordinator.VerifyCheckYourAnswersPageIsDisplayedAsync();
        await _checkYourAnswersCoordinator.ClickOnSubmitButtonAsync();
        await _submissionConfirmationCoordinator.VerifySubmissionConfirmationPageIsDisplayedAsync();
    }

    [Given("I am on the RP14 submission confirmation page")]
    public async Task GivenIAmOnTheRP14SubmissionConfirmationPage()
    {
        await _commonCoordinator.VerifySubmissionConfirmationPageIsDisplayedForRp14Async();
    }

    [Given("I have selected Upload another RP14 form")]
    public async Task GivenIHaveSelectedUploadAnotherRP14Form()
    {
        await GivenIAmOnTheRP14SubmissionConfirmationPage();
        await WhenISelectToUploadAnotherForm();
        await ThenIWillBeTakenToTheDeclarationPage();
    }

    [When("I upload another RP14 form")]
    public async Task WhenIUploadAnotherRP14Form()
    {
        await _commonCoordinator.VerifyThatCheckYourAnswersPageIsDisplayedForRp14Async();
    }

    [Then("I will be able to upload a new RP14 form")]
    public async Task ThenIWillBeAbleToUploadANewRP14Form()
    {
        await _checkYourAnswersCoordinator.VerifyCheckYourAnswersPageIsDisplayedAsync();
        await _checkYourAnswersCoordinator.ClickOnSubmitButtonAsync();
        await _submissionConfirmationCoordinator.VerifySubmissionConfirmationPageIsDisplayedAsync();
    }
}
