using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Models.TestData;
using GovUk.Forms.HostApp.UI.Test.Pages.Common;
using GovUk.Forms.HostApp.UI.Test.Pages.Login;
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
    private ILocator MainContent => Page.Locator(StartPageLocators.Selectors.MainContent);

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
        await Page.WaitForLoadStateAsync(LoadState.Load, new() { Timeout = ScenarioConstant.ElementTimeout });
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
        IReadOnlyList<string> texts = await UploadedFileStatus.AllInnerTextsAsync();
        return texts.Select(t => t.Trim()).ToList();
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


    public async Task VerifyUploadContentAriaSnapshotAsync()
    {
        await WaitForPageToLoadAsync();
        await Page.WaitForTimeoutAsync(ScenarioConstant.WaitForVisual);
        string html = await MainContent.InnerHTMLAsync();

        await Verify(html)
        .UseDirectory("Snapshots/UploadDocument")
        .UseFileName("UploadingRP14AForms")
        .ScrubLinesContaining("__RequestVerificationToken")
        .ScrubLinesWithReplace(ScrubSessionGuid)
        .DisableRequireUniquePrefix();
    }

    public async Task VerifyCommonIssuesWhenUploadingAriaSnapshotAsync()
    {
        await WaitForPageToLoadAsync();

        await VerifyUploadPageHeaderAriaSnapshotAsync();
        await VerifyCommonIssuesIntroAriaSnapshotAsync();
        await Page.WaitForTimeoutAsync(ScenarioConstant.WaitForVisual);

        string html = await MainContent.InnerHTMLAsync();

        await Verify(html)
            .UseDirectory("Snapshots/UploadDocument")
            .UseFileName("CommonIssuesWhenUploadingRP14AForms")
            .ScrubLinesContaining("__RequestVerificationToken")
            .ScrubLinesWithReplace(ScrubSessionGuid)
            .DisableRequireUniquePrefix();
    }

    private static string ScrubSessionGuid(string line)
    {
        const string marker = "id=\"Id_Value\" name=\"Id.Value\" type=\"hidden\" value=\"";
        int start = line.IndexOf(marker, StringComparison.Ordinal);
        if (start == -1)
        {
            return line;
        }
        start += marker.Length;
        int end = line.IndexOf('"', start);
        return end == -1 ? line : line[..start] + "SCRUBBED_GUID" + line[end..];
    }

    private async Task VerifyUploadPageHeaderAriaSnapshotAsync()
    {
        await Expect(MainContent)
            .ToMatchAriaSnapshotAsync("""
        - main:
          - heading "Upload redundancy payment forms (RP14/A)" [level=1]
          - text: Upload a file The uploaded file must be XML ending with '.xml'. (Other formats e.g. PDF, XLS are NOT supported).
          - button "Upload a file , No file chosen Choose file or drop file": No file chosen , Choose file or drop file
          - heading "Errors during the upload process" [level=2]
        """);
    }

    private async Task VerifyCommonIssuesIntroAriaSnapshotAsync()
    {
        await Expect(MainContent)
            .ToMatchAriaSnapshotAsync("""
        - main:
          - group:
            - text: Common issues when uploading RP14/A forms
            - paragraph: Even if you have followed all the steps in the upload instructions, you might see an error message. Using IP software to upload the data will minimise the risk of errors.
            - paragraph: "Common issues when uploading RP14/A forms:"
            - heading "File size" [level=3]
            - paragraph: The file size must be less than 10MB. If your file is bigger than this, you need to separate it into smaller files so it can be uploaded.
        """);
    }
}
