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


    public async Task VerifyUploadContentAriaSnapshotAsync()
    {
        await WaitForPageToLoadAsync();

        await Expect(MainContent)
            .ToMatchAriaSnapshotAsync("""
              - main:
                - heading "Upload redundancy payment forms (RP14/A)" [level=1]
                - text: Upload a file The uploaded file must be XML ending with '.xml'. (Other formats e.g. PDF, XLS are NOT supported).
                - button "Upload a file , No file chosen Choose file or drop file": No file chosen , Choose file or drop file
                - heading "Errors during the upload process" [level=2]
                - group: Common issues when uploading RP14/A forms
                - paragraph: "If you follow this guidance and your document will still not upload, email us at:"
                - paragraph: RPS.Stakeholder@insolvency.gov.uk
                - button "Continue"
              """);
    }

    public async Task VerifyCommonIssuesWhenUploadingAriaSnapshotAsync()
    {
        await WaitForPageToLoadAsync();

        await Expect(MainContent)
                       .ToMatchAriaSnapshotAsync("""
                   - heading "Upload redundancy payment forms (RP14/A)" [level=1]
                   - text: Upload a file The uploaded file must be XML ending with '.xml'. (Other formats e.g. PDF, XLS are NOT supported).
                   - button "Upload a file , No file chosen Choose file or drop file": No file chosen , Choose file or drop file
                   - heading "Errors during the upload process" [level=2]
                   - group:
                     - text: Common issues when uploading RP14/A forms
                     - paragraph: Even if you have followed all the steps in the upload instructions, you might see an error message. Using IP software to upload the data will minimise the risk of errors.
                     - paragraph: "Common issues when uploading RP14/A forms:"
                     - heading "File size" [level=3]
                     - paragraph: The file size must be less than 10MB. If your file is bigger than this, you need to separate it into smaller files so it can be uploaded.
                     - heading "Case and company details" [level=3]
                     - paragraph: "These can cause delays in processing claims. To avoid this, check that:"
                     - list:
                       - listitem: the case reference number (CN) is present and correct, and that there are no spaces before or after it. The employer name and case reference must be entered on each line of the spreadsheet
                       - listitem: the case reference number (CN) is present and correct, and that there are no spaces before or after it. The employer name and case reference must be entered on each line of the spreadsheet
                       - listitem: the correct Company Registration Number (CRN) is used, check the CRN matches the case
                       - listitem: National Insurance numbers are all valid and correct, as temporary or dummy numbers will not be accepted by the platform
                     - heading "Formatting problems" [level=3]
                     - paragraph: "These can include:"
                     - list:
                       - listitem:
                         - text: "Empty or blank rows at the end of the file. To remove any empty records:"
                         - list:
                           - listitem: Highlight the blank row.
                           - listitem: Right click.
                           - listitem: Delete the row.
                       - listitem:
                         - text: Too many decimal places. You might need to select each cell individually to make sure figures do not have more than 2 decimal places, as it’s not always visible.
                         - list:
                           - listitem: In the RP14, you will need to check the Shareholder percentage field
                           - listitem: In the RP14A, you will need to check 1). basic pay per week, 2). arrears of pay owed and 3). number of days holiday owed
                           - listitem: Incorrect characters. Only numbers must be put in the numerical fields. If you include any special characters, like £, or any text, the upload will fail.
                       - listitem: Incorrect date entries. Date fields must be formatted as DD/MM/YYYY. If you do not know the date and the field is not mandatory, leave it blank.
                       - listitem: Overtyped drop-down data. When entering data into a cell with a drop-down box, you must select one of the options provided using the selector, and don’t overtype.
                       - listitem: Ignoring the smart tag. Do not enter any data after the blue smart tag on the spreadsheet, as it will be lost.
                   - paragraph: "If you follow this guidance and your document will still not upload, email us at:"
                   - paragraph: RPS.Stakeholder@insolvency.gov.uk
                   - button "Continue"
                   """);
    }
}
