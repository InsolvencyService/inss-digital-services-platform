using GovUk.Forms.HostApp.UI.Test.Coordinators;
using GovUk.Forms.HostApp.UI.Test.Factories;
using GovUk.Forms.HostApp.UI.Test.Models;

namespace GovUk.Forms.HostApp.UI.Test.Steps;

[Binding]
public sealed class ComonSteps
{
    private readonly CommonCoordinator _commonCoordinator;

    public ComonSteps(CommonCoordinator commonCoordinator)
    {
        _commonCoordinator = commonCoordinator;
    }

    [Given("I am on the upload page")]
    public async Task GivenIAmOnTheUploadPage()
    {
        await _commonCoordinator.VerifyThatUploadDocumentPageIsDisplayedAsync();
    }

    [Given(@"I am on the upload page as a ""(.*)"" user")]
    public async Task GivenIAmOnTheUploadPageAsUser(string userType)
    {
        TestUser user = UserFactory.GetUser(userType);

        await _commonCoordinator.VerifyThatUploadDocumentPageIsDisplayedAsync(user);
    }

    [Then("I should be able to log out successfully")]
    public async Task ThenIShouldBeAbleToLogOutSuccessfully()
    {
        await _commonCoordinator.SignOutAndVerifyAsync();
    }

}
