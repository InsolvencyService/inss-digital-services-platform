using Xunit;

namespace GovUk.Forms.HostApp.PageTest;

public class YourDetailsFullNameTests(TestWebApplicationFactory factory) : PageTestBase(factory)
{
    [Fact]
    public async Task NavigatingToFirstPageInSectionOne_HasYourNameQuestionAndHint()
    {
        await GotToPage("about-you");
        await ClickLink("Your Details");
        await ExpectQuestion("What is your full name?");
        await ExpectHint("Enter your first name, middle name (if applicable), and last name");
    }
    
    [Fact]
    public async Task NotEnteringFullName_DisplaysError()
    {
        await GotToPage("about-you");
        await ClickLink("Your Details");
        await ClickButton("Save and continue");
        await ExpectErrorHeading("There is a problem");
        await ExpectLinkToExist("Enter your full name");
    }
}