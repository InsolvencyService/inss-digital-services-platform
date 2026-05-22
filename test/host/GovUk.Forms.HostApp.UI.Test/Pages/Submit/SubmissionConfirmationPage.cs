using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Support;

namespace GovUk.Forms.HostApp.UI.Test.Pages.Submit;

public partial class SubmissionConfirmationPage : BasePage, ISubmissionConfirmationPage
{
    private readonly IPlaywrightDriver _playwrightDriver;
    public SubmissionConfirmationPage(IPlaywrightDriver playwrightDriver)
    {
        _playwrightDriver = playwrightDriver;
    }

    private new IPage Page => _playwrightDriver.Page;

    private ILocator PageHeading => Page.GetByRole(AriaRole.Heading, new() { Name = DocumentSummaryLocators.Labels.ApplicationComplete });
    private ILocator ConfirmationPanel => Page.Locator(DocumentSummaryLocators.Selectors.ConfirmBodyCompleted);
    private ILocator UploadAnotherFormButton => Page.GetByRole(AriaRole.Button, new() { Name = DocumentSummaryLocators.Labels.UploadAnotherForm });
    private ILocator WhatHappensNextHeading => Page.GetByRole(AriaRole.Heading, new() { Name = "What happens next" });
    private ILocator WhatHappensNextParagraphs => Page.Locator("#main-content .govuk-body");

    protected override async Task PageContentLoadedAsync()
    {
        await Page.WaitForLoadStateAsync(
           LoadState.Load,
           new() { Timeout = ScenarioConstant.ElementTimeout });
        await Expect(PageHeading).ToBeVisibleAsync();
        await Expect(ConfirmationPanel).ToContainTextAsync(DocumentSummaryLocators.Labels.YourReferenceNumber);
        await Expect(UploadAnotherFormButton).ToBeVisibleAsync();
        await VerifyWhatHappensNextContentAsync();
    }

    public async Task ClickUploadAnotherFormButtonAsync()
    {
        await Expect(UploadAnotherFormButton).ToBeVisibleAsync();
        await UploadAnotherFormButton.ClickAsync();
    }
    public async Task VerifyWhatHappensNextContentAsync()
    {
        await Expect(WhatHappensNextHeading)
            .ToHaveTextAsync("What happens next");

        await Expect(WhatHappensNextParagraphs.Nth(0))
            .ToHaveTextAsync(
                "You will receive a confirmation email, the email will tell you if your form has been accepted or rejected.");

        await Expect(WhatHappensNextParagraphs.Nth(1))
            .ToHaveTextAsync(
                "If your form has been rejected the email will contain the reason for the rejection.");
    }

}
