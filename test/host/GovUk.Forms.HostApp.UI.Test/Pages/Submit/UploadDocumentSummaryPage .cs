
using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Support;

namespace GovUk.Forms.HostApp.UI.Test.Pages.Submit;

public sealed class UploadDocumentSummaryPage(
    IPlaywrightDriver playwrightDriver) : BasePage, IUploadDocumentSummaryPage
{
    private new IPage Page => playwrightDriver.Page;

    private ILocator PageHeading =>
        Page.GetByRole(AriaRole.Heading, new()
        {
            Name = DocumentSummaryLocators.Labels.CheckYourAnswersBeforeSubmittingTheForm
        });

    private ILocator WeHaveMatchedMessage =>
        Page.GetByText(DocumentSummaryLocators.Labels.WeHaveMatchedToTheFollowingEmployer, new() { Exact = true });

    private ILocator InitialValidationMessage =>
        Page.GetByText(DocumentSummaryLocators.Labels.YourFormHasPassedInitialValidation, new() { Exact = true });

    private ILocator SubmitButton =>
        Page.GetByRole(AriaRole.Button, new()
        {
            Name = DocumentSummaryLocators.Labels.Submit
        });

    private ILocator CaseReferenceRow =>
        SummaryRow(DocumentSummaryLocators.Labels.CaseReferenceNumberKey);

    private ILocator ChangeCaseReferenceLink =>
        CaseReferenceRow.GetByRole(AriaRole.Link, new()
        {
            Name = DocumentSummaryLocators.Labels.Change
        });

    private ILocator EmployerNameRow =>
        SummaryRow(DocumentSummaryLocators.Labels.EmployerNameKey);

    private ILocator IsCorrectEmployerNameRow =>
        SummaryRow(DocumentSummaryLocators.Labels.IsThisTheCorrectEmployerNameKey);

    private ILocator UploadedDocumentRow =>
        SummaryRow(DocumentSummaryLocators.Labels.UploadedDocumentKey);

    private ILocator UploadedDocumentValue =>
        UploadedDocumentRow.Locator(DocumentSummaryLocators.Selectors.SummaryListOfValue);

    private ILocator ChangeUploadedDocumentLink =>
        UploadedDocumentRow.GetByRole(AriaRole.Link, new()
        {
            Name = DocumentSummaryLocators.Labels.Change
        });

    private ILocator SummaryRow(string label) =>
        Page.Locator(DocumentSummaryLocators.Selectors.SummaryList)
            .Filter(new() { Has = Page.GetByText(label, new() { Exact = true }) });

    private ILocator SummaryValue(string label) =>
        SummaryRow(label).Locator(DocumentSummaryLocators.Selectors.SummaryListOfValue);

    protected override async Task PageContentLoadedAsync()
    {
        await Page.WaitForLoadStateAsync(
            LoadState.Load,
            new() { Timeout = ScenarioConstant.ElementTimeout });
        await Expect(PageHeading).ToBeVisibleAsync();
        await Expect(WeHaveMatchedMessage).ToBeVisibleAsync();
        await Expect(CaseReferenceRow).ToBeVisibleAsync();
        await Expect(ChangeCaseReferenceLink).ToBeVisibleAsync();
        await Expect(EmployerNameRow).ToBeVisibleAsync();
        await Expect(IsCorrectEmployerNameRow).ToBeVisibleAsync();
        await Expect(InitialValidationMessage).ToBeVisibleAsync();
        await Expect(UploadedDocumentRow).ToBeVisibleAsync();
        await Expect(SubmitButton).ToBeVisibleAsync();
    }

    public async Task VerifyUploadedDocumentAsync(string expectedFileName)
    {
        await Expect(UploadedDocumentValue).ToHaveTextAsync(expectedFileName);
    }

    public async Task VerifyCaseReferenceNumberAsync(string expectedValue)
    {
        await Expect(SummaryValue(DocumentSummaryLocators.Labels.CaseReferenceNumberKey))
            .ToHaveTextAsync(expectedValue);
    }

    public async Task VerifyEmployerNameAsync(string expectedValue)
    {
        await Expect(SummaryValue(DocumentSummaryLocators.Labels.EmployerNameKey))
            .ToHaveTextAsync(expectedValue);
    }

    public async Task VerifyIsCorrectEmployerNameAsync(string expectedValue)
    {
        await Expect(SummaryValue(DocumentSummaryLocators.Labels.IsThisTheCorrectEmployerNameKey))
            .ToHaveTextAsync(expectedValue);
    }

    public async Task ClickChangeAsync()
    {
        await ChangeUploadedDocumentLink.ClickAsync();
    }

    public async Task ClickChangeCaseReferenceAsync()
    {
        await ChangeCaseReferenceLink.ClickAsync();
    }

    public async Task ClickSubmitAsync()
    {
        await SubmitButton.ClickAsync();
    }
}
