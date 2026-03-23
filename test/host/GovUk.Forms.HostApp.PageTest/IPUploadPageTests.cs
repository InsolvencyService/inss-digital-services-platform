using Xunit;

namespace GovUk.Forms.HostApp.PageTest;

public class IPUploadPageTests(TestWebApplicationFactory factory) : PageTestBase(factory)
{
    [Fact]
    public async Task LaunchingHomePage_HasSectionLinks()
    {
        await GotToPage("ip-upload");
        await ExpectLinkToExist("IP Upload");
    }
}