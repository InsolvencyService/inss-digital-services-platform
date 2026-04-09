using GovUk.Forms.HostApp.UI.Tests.Pages;

namespace GovUk.Forms.HostApp.UI.Tests.Steps;

[Binding]
public class Feature1Steps
{
    private readonly IStartPage _startPage;

    public Feature1Steps(IStartPage startPage)
    {
        _startPage = startPage;
    }

    [Given("I am on the start page")]
    public static void GivenIAmOnTheStartPage()
    {

    }

    [When("I choose Start now")]
    public static void WhenIChooseStartNow()
    {

    }

    [Then("I will be taken to the sign in page")]
    public static void ThenIWillBeTakenToTheSignInPage()
    {

    }

}
