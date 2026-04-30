
using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Support;

namespace GovUk.Forms.HostApp.UI.Test.Pages.Upload;

public class UploadErrorsPage : BasePage, IUploadErrorsPage
{

    private readonly IPlaywrightDriver _playwrightDriver;

    public UploadErrorsPage(IPlaywrightDriver playwrightDriver)
    {
        _playwrightDriver = playwrightDriver;
    }
    private new IPage Page => _playwrightDriver.Page;
    private ILocator PageHeading => Page.GetByRole(AriaRole.Heading, new() { Name = "Your form has errors" });
    private ILocator CaseReferenceText => Page.GetByRole(AriaRole.Term, new() { Name = "Case reference" });
    private ILocator UploadedFileName => Page.Locator("p.govuk-body strong");
    private ILocator ContinueButton => Page.GetByRole(AriaRole.Button, new() { Name = "Continue" });
    private ILocator ErrorRows => Page.Locator(".govuk-summary-list__row");

    private ILocator GetByExactText(string text) => Page.GetByText(text, new PageGetByTextOptions { Exact = true });
    protected override async Task PageContentLoadedAsync()
    {
        await Page.WaitForLoadStateAsync(LoadState.Load,
           new() { Timeout = ScenarioConstant.ElementTimeout });
        await Expect(PageHeading).ToBeVisibleAsync();
        await Expect(ContinueButton).ToBeVisibleAsync();
    }

    public async Task VerifyThatCaseReferenceErrorsAreDisplayedAsync(string errorMessage)
    {
        await Expect(CaseReferenceText).ToBeVisibleAsync();
        await Expect(GetByExactText(errorMessage)).ToBeVisibleAsync();
    }

    public async Task VerifyUploadedFileNameAsync(string expectedFileName)
    {
        await Expect(UploadedFileName).ToHaveTextAsync(expectedFileName);
    }

    public async Task VerifyErrorSummaryAsync(UploadErrorSummary expected)
    {
        await PageContentLoadedAsync();

        ILocator row = ErrorRows
            .Filter(new() { HasTextString = expected.ErrorType })
            .Filter(new() { HasTextString = expected.ErrorMessage });

        await Expect(row).ToHaveCountAsync(1);
        await Expect(row).ToContainTextAsync(expected.Category);
        await Expect(row).ToContainTextAsync(expected.ErrorType);
        await Expect(row).ToContainTextAsync(expected.ErrorMessage);

        if (!string.IsNullOrWhiteSpace(expected.HintText))
        {
            await Expect(row.Locator(".govuk-hint")).ToHaveTextAsync(expected.HintText);
        }

        await Expect(
            row.GetByRole(AriaRole.Link, new() { Name = expected.ActionText })
        ).ToBeVisibleAsync();
    }

    public async Task ClickOnViewDetailsAsync(UploadErrorSummary expected)
    {
        ILocator row = ErrorRows.Filter(new()
        {
            HasTextString = expected.ErrorType
        });

        await row.GetByRole(AriaRole.Link, new() { Name = expected.ActionText }).ClickAsync();
    }

    public async Task ClickContinueAsync()
    {
        await ContinueButton.ClickAsync();
    }
}
