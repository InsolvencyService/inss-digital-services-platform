using Xunit;

namespace GovUk.Forms.HostApp.PageTest;

public class AboutYouPageTests(TestWebApplicationFactory factory) : PageTestBase(factory)
{
    [Fact]
    public async Task LaunchingHomePage_HasSectionLinks()
    {
        await GotToPage("about-you");
        await ExpectLinkToExist("Your Details");
    }
}