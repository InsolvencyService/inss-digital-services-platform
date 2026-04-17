using GovUk.Forms.HostApp.UI.Test.Coordinators;
namespace GovUk.Forms.HostApp.UI.Test.Steps;

[Binding]
public class Feature1Steps
{
    private readonly DeclarationCoordinator _demoCoordinator;
    public Feature1Steps(DeclarationCoordinator demoCoordinator)
    {
        _demoCoordinator = demoCoordinator;
    }

    [Given("I am on the declaration page")]
    public async Task GivenIAmOnTheDeclarationPage()
    {
        await _demoCoordinator.VerifyDeclarationPageIsDisplayedAsync();
    }

    [When("I choose to view section 187")]
    public async Task WhenIChooseToViewSection()
    {
        await _demoCoordinator.OpenSection187Async();
    }

    [Then("I will be taken to the section 187 page")]
    public async Task ThenIWillBeTakenToThesectionPage()
    {
        await _demoCoordinator.VerifySection187PageContentAsync();
    }

}
