namespace GovUk.Forms.HostApp.UI.Test.Pages;

public class DirectorConductReportingServicePage : BasePage
{
    private ILocator PageLogo => Page.GetByRole(AriaRole.Img, new() { Name = "The Insolvency Service logo" });

    protected override async Task PageContentLoadedAsync()
    {
        await Expect(Page).ToHaveTitleAsync("Customer feedback: Insolvency Practitioner Upload Service (IPUS)");
        await Expect(PageLogo).ToBeVisibleAsync();
    }

    public async Task VerifyThatDirectorConductReportingServicePageIsDisplayedAsync()
    {
        await PageContentLoadedAsync();
    }
    public void SetPage(IPage page)
    {
        AttachTo(page);
    }
}
