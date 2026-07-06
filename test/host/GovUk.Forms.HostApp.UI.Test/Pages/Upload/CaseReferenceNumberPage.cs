using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Pages.Common;
using GovUk.Forms.HostApp.UI.Test.Support;
using System.Text.RegularExpressions;

namespace GovUk.Forms.HostApp.UI.Test.Pages.Upload;

public partial class CaseReferenceNumberPage : BasePage, ICaseReferenceNumberPage
{
    [GeneratedRegex(@".*/ip-upload/redundancy-payment/check-case-reference(\?.*)?$")]
    private static partial Regex CheckCaseReferenceUrlRegex();

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
    private ILocator ErrorSummaryTitle => Page.Locator(UploadLocators.Selectors.ErrorSummaryTitle);
    private ILocator CaseReferenceFieldError => Page.Locator(UploadLocators.Selectors.CaseReferenceFieldError);
    private ILocator ErrorInputGroup => Page.Locator(UploadLocators.Selectors.ErrorGroupForm);
    private ILocator MainContent => Page.Locator(UploadLocators.Selectors.MainContent);

    protected override async Task PageContentLoadedAsync()
    {
        await Page.WaitForLoadStateAsync(LoadState.Load, new() { Timeout = ScenarioConstant.ElementTimeout });
        await Expect(PageHeading).ToBeVisibleAsync();
        await Expect(CaseReferenceInput).ToBeVisibleAsync();
        await Expect(ContinueButton).ToBeVisibleAsync();
    }

    public async Task EnterCaseReferenceNumberAsync(string caseReference)
    {
        await PageContentLoadedAsync();
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

    public async Task VerifyValidationErrorsAsync(string errorMessage)
    {
        await Expect(ErrorSummary).ToBeVisibleAsync();
        await Expect(ErrorSummaryTitle).ToHaveTextAsync(UploadLocators.Labels.ThereIsAProblem);
        await Expect(ErrorSummary.GetByRole(AriaRole.Link, new() { Name = errorMessage })).ToBeVisibleAsync();
        await Expect(ErrorInputGroup).ToBeVisibleAsync();
        await Expect(CaseReferenceFieldError).ToContainTextAsync(errorMessage);
        await Expect(Page).ToHaveURLAsync(CheckCaseReferenceUrlRegex(), new PageAssertionsToHaveURLOptions { IgnoreCase = true });
    }

    public async Task VerifyAriaSnapshotAsync()
    {
        await WaitForPageToLoadAsync();

        await Expect(MainContent).ToMatchAriaSnapshotAsync("""
            - heading "Enter the 10 character case reference number" [level=1]
            - text: For example, 'CN12345678'. This must match the case reference number in your uploaded file.
            - textbox "Enter the 10 character case reference number": /.*/
            - button "Continue"
            """);
    }
}
