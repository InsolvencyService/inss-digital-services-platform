using GovUk.Forms.HostApp.UI.Tests.Coordinators;
namespace GovUk.Forms.HostApp.UI.Tests.Steps;

[Binding]
public class Feature1Steps
{
    private readonly DemoCoordinator _demoCoordinator;
    //private readonly IDeclarationPage _declarationPage;
    public Feature1Steps(DemoCoordinator demoCoordinator) //IDeclarationPage declarationPage)
    {
        _demoCoordinator = demoCoordinator;
        //_declarationPage = declarationPage;
    }

    [Given("I am on the declaration page")]
    public async Task GivenIAmOnTheDeclarationPage()
    {
        // await _declarationPage.WaitForPageToLoadAsync();
        await _demoCoordinator.VerifyDeclarationPageContentAsync();
        // Thread.Sleep(2000);
    }

    [When("I choose to view section 187")]
    public async Task WhenIChooseToViewSection()
    {
        await _demoCoordinator.OpenSection187Async();
    }

    [Then("I will be taken to the section 187 page")]
    public async Task ThenIWillBeTakenToThesectionPage()
    {
        Assert.Fail();
        await _demoCoordinator.VerifySection187PageContentAsync();
    }

}
