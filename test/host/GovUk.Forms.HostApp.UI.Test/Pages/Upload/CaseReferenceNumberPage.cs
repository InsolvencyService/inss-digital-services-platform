using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Pages.Common;
using GovUk.Forms.HostApp.UI.Test.Support;

namespace GovUk.Forms.HostApp.UI.Test.Pages.Upload;

public class CaseReferenceNumberPage : BasePage, ICaseReferenceNumberPage
{
    private readonly IPlaywrightDriver _playwrightDriver;

    public CaseReferenceNumberPage(IPlaywrightDriver playwrightDriver)
    {
        _playwrightDriver = playwrightDriver;
    }

    private new IPage Page => _playwrightDriver.Page;

    private ILocator PageHeading => Page.GetByRole(AriaRole.Heading, new() { Name = UploadLocators.Labels.CaseReferenceNumberHeading });
    private ILocator CaseReferenceInput => Page.Locator(UploadLocators.Selectors.CaseReferenceNumberInput);
    private ILocator ContinueButton => Page.GetByRole(AriaRole.Button, new() { Name = SharedLocactors.ContinueButton });
    private ILocator BackLink => Page.GetByRole(AriaRole.Link, new() { Name = SharedLocactors.BackButton, Exact = true });
    private ILocator ErrorSummary => Page.Locator(UploadLocators.Selectors.ErrorSummary);
    private ILocator MainContent => Page.Locator("#main-content");

    protected override async Task PageContentLoadedAsync()
    {
        await Page.WaitForLoadStateAsync(LoadState.Load, new() { Timeout = ScenarioConstant.ElementTimeout });
        await Expect(PageHeading).ToBeVisibleAsync();
        await Expect(CaseReferenceInput).ToBeVisibleAsync();
        await Expect(ContinueButton).ToBeVisibleAsync();
    }

    public async Task EnterCaseReferenceNumberAsync(string caseReference)
    {
        await CaseReferenceInput.FillAsync(caseReference);
    }

    public async Task ClickContinueAsync()
    {
        await ContinueButton.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.Load, new() { Timeout = ScenarioConstant.ElementTimeout });
    }

    public async Task ClickBackAsync()
    {
        await BackLink.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.Load, new() { Timeout = ScenarioConstant.ElementTimeout });
    }

    public async Task VerifyErrorMessageAsync(string errorMessage)
    {
        await Expect(ErrorSummary).ToBeVisibleAsync();
        await Expect(ErrorSummary.GetByRole(AriaRole.Link, new() { Name = errorMessage })).ToBeVisibleAsync();
    }

    public async Task VerifyAriaSnapshotAsync()
    {
        await WaitForPageToLoadAsync();

        await Expect(MainContent).ToMatchAriaSnapshotAsync("""
            - heading "Whats the case reference number?" [level=1]
            - text: For example, 'CN123456K'. This must match the case reference number in your form.
            - textbox "Whats the case reference number?"
            - button "Continue"
            """);
    }
}
