using GovUk.Forms.HostApp.UI.Test.Coordinators;

namespace GovUk.Forms.HostApp.UI.Test.Steps;

[Binding]
public class NavigationSteps(NavigationCoordinator navigationCoordinator)
{
    [Given("I am on the IP Upload application")]
    public async Task GivenIAmOnTheIPUploadApplication()
    {
        await navigationCoordinator.VerifyOnApplicationAsync();
    }

    [When("I navigate to an invalid IP Upload URL")]
    public async Task WhenINavigateToAnInvalidIPUploadURL()
    {
        await navigationCoordinator.NavigateToInvalidUrlAsync();
    }

    [Then("the Page not found page should be displayed")]
    public async Task ThenThePageNotFoundPageShouldBeDisplayed()
    {
        await navigationCoordinator.VerifyPageNotFoundIsDisplayedAsync();
    }
}
