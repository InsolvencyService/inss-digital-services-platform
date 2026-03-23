using Xunit;

namespace GovUk.Forms.HostApp.PageTest;

public class BusinessPageTests(TestWebApplicationFactory factory) : PageTestBase(factory)
{
    [Fact]
    public async Task LaunchingHomePage_HasSectionLinks()
    {
        await GotToPage("business");
        await ExpectLinkToExist("Employee Details");
        await ExpectLinkToExist("Creditors and Debtors");
    }
}