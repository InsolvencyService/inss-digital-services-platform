using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Support;

namespace GovUk.Forms.HostApp.UI.Test.Pages.Submit;

public partial class SubmitCompletedPage : BasePage, ISubmitCompletedPage
{
    private readonly IPlaywrightDriver _playwrightDriver;
    public SubmitCompletedPage(IPlaywrightDriver playwrightDriver)
    {
        _playwrightDriver = playwrightDriver;
    }

    private new IPage Page => _playwrightDriver.Page;

    private ILocator PageHeading => Page.GetByRole(AriaRole.Heading, new() { Name = DocumentSummaryLocators.Labels.ApplicationComplete });
    private ILocator ConfirmationPanel => Page.Locator(DocumentSummaryLocators.Selectors.ConfirmBodyCompleted);

    protected override async Task PageContentLoadedAsync()
    {
        await Page.WaitForLoadStateAsync(
           LoadState.Load,
           new() { Timeout = ScenarioConstant.ElementTimeout });
        await Expect(PageHeading).ToBeVisibleAsync();
        await Expect(ConfirmationPanel).ToContainTextAsync(DocumentSummaryLocators.Labels.YourReferenceNumber);
    }
}
