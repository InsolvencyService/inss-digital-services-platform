using GovUk.Forms.HostApp.UI.Test.Coordinators;
using GovUk.Forms.HostApp.UI.Test.Support;
namespace GovUk.Forms.HostApp.UI.Test.Steps;

[Binding]
public class DeclarationSteps
{
    private readonly StartPageCoordinator _startPageCoordinator;
    private readonly DeclarationCoordinator _declarationCoordinator;
    private readonly SignInCoordinator _signInCoordinator;
    private readonly UploadDocumentCoordinator _uploadDocumentCoordinator;
    public DeclarationSteps(DeclarationCoordinator demoCoordinator,
        StartPageCoordinator startPageCoordinator,
        SignInCoordinator signInCoordinator,
        UploadDocumentCoordinator uploadDocumentCoordinator)
    {
        _declarationCoordinator = demoCoordinator;
        _startPageCoordinator = startPageCoordinator;
        _signInCoordinator = signInCoordinator;
        _uploadDocumentCoordinator = uploadDocumentCoordinator;
    }

    [Given("I am on the declaration page")]
    public async Task GivenIAmOnTheDeclarationPage()
    {
        await _startPageCoordinator.NavigateToLoginPageAsync();
        await _signInCoordinator.SignInToServiceAsync(ScenarioConstant.EmailAddress, ScenarioConstant.Password);
        await _declarationCoordinator.VerifyDeclarationPageIsDisplayedAsync();
    }

    [When("I choose to view section 187")]
    public async Task WhenIChooseToViewSection()
    {
        await _declarationCoordinator.OpenSection187Async();
    }

    [When("I choose to Agree and continue")]
    public async Task WhenIChooseToAgreeAndContinue()
    {
        await _declarationCoordinator.ClickAgreeAndContinueButtonAsync();
    }

    [When("I choose to return to the start page")]
    public async Task WhenIChooseToReturnToTheStartPage()
    {
        await _declarationCoordinator.ReturnToStartPageAsync();
    }

    [Then("I will be taken to the section 187 page")]
    public async Task ThenIWillBeTakenToThesectionPage()
    {
        await _declarationCoordinator.VerifySection187PageContentAsync();
    }

    [Then("the start page should be displayed")]
    public async Task ThenTheStartPageShouldBeDisplayed()
    {
        await _startPageCoordinator.VerifyStartPageIsDisplayedAsync();
    }

    [Then("I will be taken to the file upload page")]
    public async Task ThenIWillBeTakenToTheFileUploadPage()
    {
        await _uploadDocumentCoordinator.VerifyUploadDocumentPageIsDisplayedAsync();
    }



}
