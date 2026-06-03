using GovUk.Forms.HostApp.UI.Test.Coordinators;
using GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;
namespace GovUk.Forms.HostApp.UI.Test.Steps;

[Scope(Feature = "Declaration Page")]
[Binding]
public class DeclarationSteps
{
    private readonly DeclarationCoordinator _declarationCoordinator;
    private readonly UploadDocumentCoordinator _uploadDocumentCoordinator;
    private readonly CommonCoordinator _commonCoordinator;
    private readonly StartPageCoordinator _startPageCoordinator;
    public DeclarationSteps(DeclarationCoordinator demoCoordinator,
        UploadDocumentCoordinator uploadDocumentCoordinator,
        CommonCoordinator commonCoordinator,
        StartPageCoordinator startPageCoordinator)
    {
        _declarationCoordinator = demoCoordinator;
        _uploadDocumentCoordinator = uploadDocumentCoordinator;
        _commonCoordinator = commonCoordinator;
        _startPageCoordinator = startPageCoordinator;
    }

    [Given("I am on the declaration page")]
    public async Task GivenIAmOnTheDeclarationPage()
    {
        await _commonCoordinator.VerifyThatDeclarationPageIsDisplayedAsync();
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

    [When("I am on the declaration page")]
    public async Task WhenIAmOnTheDeclarationPage()
    {
        await _declarationCoordinator.VerifyDeclarationPageIsDisplayedAsync();
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

    [Then("I will see the terms I need to agree to")]
    public async Task ThenIWillSeeTheTermsINeedToAgreeTo()
    {
        await _declarationCoordinator.VerifyDeclarationAriaSnapshotAsync();
    }

    [Then("I should have the ability to sign out")]
    [Then("I should be able to sign out directly from the file upload page")]
    public async Task ThenIShouldBeAbleToSignOutDirectlyFromTheFileUploadPage()
    {
        await _commonCoordinator.SignOutAndVerifyAsync();
    }
}
