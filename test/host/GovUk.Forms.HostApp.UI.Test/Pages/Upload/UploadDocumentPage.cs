using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Models.TestData;
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

    private ILocator BackButton => Page.GetByRole(AriaRole.Link, new() { Name = SharedLocactors.BackButton, Exact = true });
    private ILocator GiveFeedbackLink => Page.GetByRole(AriaRole.Link, new() { Name = SharedLocactors.FeedbackLink });
    private ILocator UploadFileText => Page.GetByText(UploadLocators.Labels.UploadFile, new() { Exact = true });
    private ILocator NoFileChosenText => Page.GetByText(UploadLocators.Labels.NoFileChosen, new() { Exact = true });
    private ILocator CommonIssuesWhenUploadingRP14AForms => Page.GetByText(UploadLocators.Labels.CommonIssuesWhenUploadingRP14AForms, new() { Exact = true });
    private ILocator RPSStakeholderEmail => Page.GetByText(UploadLocators.Labels.RPSStakeholderEmail, new() { Exact = true });
    private ILocator CommonUploadErrorsText => Page.GetByText(UploadLocators.Labels.CommonUploadErrors, new() { Exact = true });
    private ILocator ContinueButton => Page.GetByRole(AriaRole.Button, new() { Name = SharedLocactors.ContinueButton });
    private ILocator FileUploadInput => Page.Locator(UploadLocators.Selectors.FileInput);
    private ILocator UploadedFileStatus => Page.Locator(UploadLocators.Selectors.UploadStatus);
    private ILocator ErrorSummary => Page.Locator(UploadLocators.Selectors.ErrorSummary);
    private ILocator ErrorSummaryTitle => ErrorSummary.GetByRole(AriaRole.Heading, new() { Name = UploadLocators.Labels.ThereIsAProblem });
    private ILocator ErrorSummaryLink => ErrorSummary.GetByRole(AriaRole.Link);
    private ILocator UploadFileFormGroup => Page.Locator(UploadLocators.Selectors.ErrorGroupForm);
    private ILocator UploadFileErrorMessage => Page.Locator(UploadLocators.Selectors.ContentError);
    private ILocator UploadFileInput => Page.Locator(UploadLocators.Selectors.UploadForm);


    protected override async Task PageContentLoadedAsync()
    {
        await Page.WaitForLoadStateAsync(LoadState.Load,
            new() { Timeout = ScenarioConstant.ElementTimeout });
        await Expect(UploadFileText).ToBeVisibleAsync();
        await Expect(NoFileChosenText).ToBeVisibleAsync();
        await Expect(CommonIssuesWhenUploadingRP14AForms).ToBeVisibleAsync();
        await Expect(RPSStakeholderEmail).ToBeVisibleAsync();
        await Expect(CommonUploadErrorsText).ToBeVisibleAsync();
        await Expect(ContinueButton).ToBeVisibleAsync();
    }


    public async Task ClickOnContinueButtonAsync()
    {
        await ContinueButton.ClickAsync();
        await Page.WaitForLoadStateAsync(LoadState.Load, new() { Timeout = ScenarioConstant.ElementTimeout });
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
        string absolutePath = Path.GetFullPath(filePath);

        byte[] bytes = await File.ReadAllBytesAsync(absolutePath);

        await
            FileUploadInput
            .SetInputFilesAsync(new FilePayload
            {
                Name = Path.GetFileName(absolutePath),
                MimeType = "text/xml",
                Buffer = bytes
            });
    }

    public async Task<string> GetUploadedFileNameAsync()
    {
        return (await UploadedFileStatus.InnerTextAsync()).Trim();
    }

    public async Task<IReadOnlyList<string>> GetUploadedFileNamesAsync()
    {
        return await UploadedFileStatus.AllInnerTextsAsync();
    }

    public async Task VerifyUploadFileErrorAsync(UploadFileError expected)
    {
        await Expect(ErrorSummary).ToBeVisibleAsync();
        await Expect(ErrorSummaryTitle).ToBeVisibleAsync();

        await Expect(ErrorSummaryLink).ToHaveTextAsync(expected.ErrorMessage);
        await Expect(UploadFileFormGroup).ToBeVisibleAsync();
        await Expect(UploadFileErrorMessage).ToContainTextAsync(expected.ErrorMessage);
    }

    public async Task ClickErrorSummaryLinkAsync()
    {
        await ErrorSummaryLink.ClickAsync();
        await Expect(UploadFileInput).ToBeFocusedAsync();
    }
}
