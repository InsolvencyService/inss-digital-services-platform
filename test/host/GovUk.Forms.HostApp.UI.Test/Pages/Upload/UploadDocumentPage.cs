
using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Pages.Common;
using GovUk.Forms.HostApp.UI.Test.Support;

namespace GovUk.Forms.HostApp.UI.Test.Pages.Upload;

public class UploadDocumentPage : BasePage, IUploadDocumentPage
{

    private readonly IPlaywrightDriver _playwrightDriver;
    private readonly ICommonPage _commonPage;

    public UploadDocumentPage(IPlaywrightDriver playwrightDriver, ICommonPage commonPage)
    {
        _playwrightDriver = playwrightDriver;
        _commonPage = commonPage;
    }

    private new IPage Page => _playwrightDriver.Page;

    private ILocator UploadRedundancyPaymentForms => Page.GetByRole(AriaRole.Heading, new() { Name = SharedLoactors.UploadRedundancyPaymentForms });
    private ILocator BackButton => Page.GetByRole(AriaRole.Link, new() { Name = SharedLoactors.BackButton, Exact = true });
    private ILocator GiveFeedbackLink => Page.GetByRole(AriaRole.Link, new() { Name = SharedLoactors.FeedbackLink });
    private ILocator GOVUKLink => Page.GetByRole(AriaRole.Img, new() { Name = SharedLoactors.GOVUKLink });
    private ILocator UploadFileText => Page.GetByText(UploadLocators.Labels.UploadFile, new() { Exact = true });
    private ILocator NoFileChosenText => Page.GetByText(UploadLocators.Labels.NoFileChosen, new() { Exact = true });
    private ILocator CommonIssuesWhenUploadingRP14AForms => Page.GetByText(UploadLocators.Labels.CommonIssuesWhenUploadingRP14AForms, new() { Exact = true });
    private ILocator RPSStakeholderEmail => Page.GetByText(UploadLocators.Labels.RPSStakeholderEmail, new() { Exact = true });
    private ILocator GuidanceText => Page.GetByText(UploadLocators.Labels.Guidance, new() { Exact = true });
    private ILocator ContinueButton => Page.GetByRole(AriaRole.Button, new() { Name = SharedLoactors.ContinueButton });
    private ILocator FileUploadInput => Page.Locator(UploadLocators.Selectors.FileInput);
    private ILocator BetaText => Page.GetByText(SharedLoactors.Beta, new() { Exact = true });
    private ILocator UploadedFileStatus => Page.Locator(UploadLocators.Selectors.UploadStatus);

    protected override async Task PageContentLoadedAsync()
    {
        await Page.WaitForLoadStateAsync(LoadState.Load);
        await Expect(BetaText).ToBeVisibleAsync();
        await Expect(UploadRedundancyPaymentForms).ToBeVisibleAsync();
        await Expect(BackButton).ToBeVisibleAsync();
        await Expect(GiveFeedbackLink).ToBeVisibleAsync();
        await Expect(GOVUKLink).ToBeVisibleAsync();
        await Expect(UploadFileText).ToBeVisibleAsync();
        await Expect(NoFileChosenText).ToBeVisibleAsync();
        await Expect(CommonIssuesWhenUploadingRP14AForms).ToBeVisibleAsync();
        await Expect(RPSStakeholderEmail).ToBeVisibleAsync();
        await Expect(GuidanceText).ToBeVisibleAsync();
        await Expect(ContinueButton).ToBeVisibleAsync();
    }


    public async Task ClickOnContinueButtonAsync()
    {
        await ContinueButton.ClickAsync();
    }
    public async Task ClickOnBackButtonAsync()
    {
        await BackButton.ClickAsync();
    }

    public async Task<IPage> ClickOnGiveFeedbackLinkAsync()
    {
        await WaitForPageToLoadAsync();
        IPage page = await _commonPage.OpenNewTabAndVerifyAsync(
            Page.Context,
            () => GiveFeedbackLink.ClickAsync(),
            expectedUrlPart: PartialPageUris.DirectorConductReportingServicePage);

        return page;
    }

    public async Task ExpandCommonIssuesWhenUploadingRP14AFormsAsync()
    {
        await CommonIssuesWhenUploadingRP14AForms.ClickAsync();
    }

    public async Task UploadFileAsync(string filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath);
        await FileUploadInput.SetInputFilesAsync(filePath);
    }

    public async Task<string> GetUploadedFileNameAsync()
    {
        return (await UploadedFileStatus.InnerTextAsync()).Trim();
    }

    public async Task<IReadOnlyList<string>> GetUploadedFileNamesAsync()
    {
        return await UploadedFileStatus.AllInnerTextsAsync();
    }

    public async Task<string> CapturePageVisualAsync(string name)
    {
        await PageContentLoadedAsync();
        return await _commonPage.CaptureVisualAsync(Page, name);

    }
}
