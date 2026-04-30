
using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Support;

namespace GovUk.Forms.HostApp.UI.Test.Pages.Submit;

public class UploadDocumentSummaryPage : BasePage, IUploadDocumentSummaryPage
{
    private readonly IPlaywrightDriver _playwrightDriver;
    public UploadDocumentSummaryPage(IPlaywrightDriver playwrightDriver)
    {
        _playwrightDriver = playwrightDriver;
    }

    private new IPage Page => _playwrightDriver.Page;

    private ILocator PageHeading => Page.GetByRole(AriaRole.Heading, new() { Name = "Check your answers before submitting the form" });
    private ILocator InitialValidationMessage => Page.GetByText("Your form has passed initial validation.", new() { Exact = true });
    private ILocator SubmitButton => Page.GetByRole(AriaRole.Button, new() { Name = "Submit" });
    private ILocator UploadedDocumentRow => Page.Locator(".govuk-summary-list__row")
            .Filter(new() { HasTextString = "Upload document" });
    private ILocator UploadedDocumentValue => UploadedDocumentRow.Locator(".govuk-summary-list__value");
    private ILocator ChangeLink =>
        UploadedDocumentRow.GetByRole(AriaRole.Link, new()
        {
            Name = "Change"
        });

    protected override async Task PageContentLoadedAsync()
    {
        await Page.WaitForLoadStateAsync(
            LoadState.Load,
            new() { Timeout = ScenarioConstant.ElementTimeout });

        await VerifyPageIsDisplayedAsync();
    }

    public async Task VerifyPageIsDisplayedAsync()
    {
        await Expect(PageHeading).ToBeVisibleAsync();
        await Expect(InitialValidationMessage).ToBeVisibleAsync();
        await Expect(UploadedDocumentRow).ToBeVisibleAsync();
        await Expect(SubmitButton).ToBeVisibleAsync();
    }

    public async Task ClickOnSubmitButtonAsync()
    {
        await SubmitButton.ClickAsync();
    }
    public async Task VerifyUploadedDocumentAsync(string expectedFileName)
    {
        await Expect(UploadedDocumentValue).ToHaveTextAsync(expectedFileName);
    }

    public async Task ClickChangeAsync()
    {
        await ChangeLink.ClickAsync();
    }

    public async Task ClickSubmitAsync()
    {
        await SubmitButton.ClickAsync();
    }
}
