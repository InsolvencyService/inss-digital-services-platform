using Xunit;

namespace GovUk.Forms.HostApp.PageTest;

public class IPUploadPageTests(TestWebApplicationFactory factory) : PageTestBase(factory)
{
    [Fact]
    public async Task LaunchingIPUpload_NavigatesToDeclarationPage()
    {
        await GotToPage("ip-upload");
        await ExpectHeading("Upload redundancy payment forms(RP14/A)"); 
    }
}