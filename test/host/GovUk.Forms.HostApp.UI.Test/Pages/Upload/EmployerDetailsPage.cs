using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Pages.Common;
using GovUk.Forms.HostApp.UI.Test.Support;

namespace GovUk.Forms.HostApp.UI.Test.Pages.Upload;

public sealed class EmployerDetailsPage(
    IPlaywrightDriver playwrightDriver) : BasePage, IEmployerDetailsPage
{
    private new IPage Page => playwrightDriver.Page;

    private ILocator PageHeading =>
        Page.GetByRole(AriaRole.Heading, new()
        {
            Name = UploadLocators.Labels.EmployerDetailsHeading,
            Level = 1
        });

    private ILocator ConfirmHeading =>
        Page.GetByRole(AriaRole.Heading, new()
        {
            Name = UploadLocators.Labels.IsThisTheCorrectEmployerNameHeading,
            Level = 2
        });

    private ILocator YesRadio =>
        Page.GetByRole(AriaRole.Radio, new()
        {
            Name = UploadLocators.Labels.YesOption,
            Exact = true
        });

    private ILocator NoRadio =>
        Page.GetByRole(AriaRole.Radio, new()
        {
            Name = UploadLocators.Labels.NoOption,
            Exact = true
        });

    private ILocator ContinueButton =>
        Page.GetByRole(AriaRole.Button, new()
        {
            Name = SharedLocactors.ContinueButton
        });

    private ILocator BackLink =>
        Page.GetByRole(AriaRole.Link, new()
        {
            Name = SharedLocactors.BackButton,
            Exact = true
        });

    private ILocator MainContent =>
        Page.Locator("#main-content");

    private ILocator SummaryRow(string label) =>
        Page.Locator(".govuk-summary-list__row")
            .Filter(new() { Has = Page.GetByText(label, new() { Exact = true }) });

    private ILocator SummaryValue(string label) =>
        SummaryRow(label).Locator(".govuk-summary-list__value");

    protected override async Task PageContentLoadedAsync()
    {
        await Page.WaitForLoadStateAsync(
            LoadState.DOMContentLoaded,
            new() { Timeout = ScenarioConstant.ElementTimeout });

        await Expect(PageHeading).ToBeVisibleAsync();
        await Expect(ConfirmHeading).ToBeVisibleAsync();
        await Expect(YesRadio).ToBeVisibleAsync();
        await Expect(NoRadio).ToBeVisibleAsync();
        await Expect(ContinueButton).ToBeVisibleAsync();
    }

    public async Task<string> GetCaseReferenceNumberAsync()
    {
        return await GetSummaryValueAsync(UploadLocators.Labels.CaseReferenceNumberLabel);
    }

    public async Task<string> GetEmployerNameAsync()
    {
        return await GetSummaryValueAsync(UploadLocators.Labels.EmployerNameLabel);
    }

    public async Task SelectYesAsync()
    {
        await YesRadio.CheckAsync();
        await Expect(YesRadio).ToBeCheckedAsync();
    }

    public async Task SelectNoAsync()
    {
        await NoRadio.CheckAsync();
        await Expect(NoRadio).ToBeCheckedAsync();
    }

    public async Task ClickContinueAsync()
    {
        await ContinueButton.ClickAsync();

        await Page.WaitForLoadStateAsync(
            LoadState.DOMContentLoaded,
            new() { Timeout = ScenarioConstant.ElementTimeout });
    }

    public async Task ClickBackAsync()
    {
        await BackLink.ClickAsync();

        await Page.WaitForLoadStateAsync(
            LoadState.DOMContentLoaded,
            new() { Timeout = ScenarioConstant.ElementTimeout });
    }

    public async Task VerifyAriaSnapshotAsync()
    {
        await WaitForPageToLoadAsync();

        await Expect(MainContent).ToMatchAriaSnapshotAsync("""
            - heading "Employer details" [level=1]
            - paragraph: We have matched to the following employer:
            - term: Case reference number
            - term: Employer name
            - heading "Is this the correct employer name?" [level=2]
            - paragraph: This case reference number you have provided matches with this employer in our system.
            - radio "Yes"
            - radio "No"
            - button "Continue"
            """);
    }

    private async Task<string> GetSummaryValueAsync(string label)
    {
        ILocator value = SummaryValue(label);

        await Expect(value).ToBeVisibleAsync();

        string text = await value.InnerTextAsync();

        return text.Trim();
    }
}