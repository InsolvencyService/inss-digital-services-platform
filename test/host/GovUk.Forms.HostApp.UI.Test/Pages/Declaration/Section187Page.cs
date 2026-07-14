namespace GovUk.Forms.HostApp.UI.Test.Pages.Declaration;


public class Section187Page : BasePage, ISection187Page
{
    protected override async Task PageContentLoadedAsync()
    {
        await Expect(Page).ToHaveTitleAsync("Employment Rights Act 1996");
    }

    public async Task VerifyThatSection187PageIsDisplayedAsync()
    {
        await PageContentLoadedAsync();
    }
}

