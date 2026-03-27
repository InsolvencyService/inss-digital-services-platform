using Xunit;

namespace GovUk.Forms.HostApp.PageTest;

public class AboutYouPageTests(TestWebApplicationFactory factory) : PageTestBase(factory)
{
    [Fact]
    public async Task LaunchingAboutYou_NavigatesToDeclarationPage()
    {
        await GotToPage("about-you");
        await ExpectQuestion("What is your full name?");
    }
}