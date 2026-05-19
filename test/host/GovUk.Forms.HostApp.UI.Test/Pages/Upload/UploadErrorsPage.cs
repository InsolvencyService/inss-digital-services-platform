
using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Pages.Common;
using GovUk.Forms.HostApp.UI.Test.Support;
using System.Globalization;

namespace GovUk.Forms.HostApp.UI.Test.Pages.Upload;

public class UploadErrorsPage : BasePage, IUploadErrorsPage
{
    private readonly IPlaywrightDriver _playwrightDriver;
    public UploadErrorsPage(IPlaywrightDriver playwrightDriver)
    {
        _playwrightDriver = playwrightDriver ?? throw new ArgumentNullException(nameof(playwrightDriver));
    }

    private new IPage Page => _playwrightDriver.Page;

    private ILocator PageHeading => Page.GetByRole(AriaRole.Heading,
        new() { Name = UploadLocators.Labels.ErrorPageTitle });

    private ILocator CaseReferenceText => Page.GetByRole(AriaRole.Term,
        new() { Name = UploadLocators.Labels.CaseReference });
    private ILocator UploadedFileName => Page.Locator(UploadLocators.Selectors.UploadFileName);
    private ILocator ContinueButton => Page.GetByRole(AriaRole.Button,
        new() { Name = SharedLocactors.ContinueButton });

    private ILocator ErrorRows => Page.Locator(UploadLocators.Selectors.ErrorRowSelector);

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
        await Expect(Page.GetByText(errorMessage, new() { Exact = true })).ToBeVisibleAsync();
    }

    public async Task VerifyUploadedFileNameAsync(string expectedFileName)
    {
        await Expect(UploadedFileName).ToHaveTextAsync(expectedFileName);
    }

    public async Task VerifyErrorSummaryAsync(UploadErrorSummary expected)
    {
        await PageContentLoadedAsync();

        ILocator row = GetErrorRow(expected);

        await Expect(row).ToHaveCountAsync(1);

        // Verify all expected fields are present
        await VerifyRowContent(row, expected);

        // Verify the action link exists
        await Expect(
            row.GetByRole(AriaRole.Link, new() { Name = expected.ActionText })
        ).ToBeVisibleAsync();
    }

    public async Task ClickOnViewDetailsAsync(UploadErrorSummary expected)
    {
        ILocator row = GetErrorRow(expected);
        await row.GetByRole(AriaRole.Link, new() { Name = expected.ActionText }).ClickAsync();
    }

    public async Task ClickContinueAsync()
    {
        await ContinueButton.ClickAsync();
    }

    public async Task<int> GetErrorCountAsync(string errorKey)
    {
        if (string.IsNullOrWhiteSpace(errorKey))
        {
            throw new ArgumentException("Error key cannot be null or empty.", nameof(errorKey));
        }

        try
        {
            string text = await Page
                .GetByText(errorKey)
                .First
                .InnerTextAsync();

            string[] parts = text.Split(' ');

            if (parts.Length == 0 || !int.TryParse(parts[UploadLocators.Selectors.ErrorCountParseIndex],
                NumberStyles.None, CultureInfo.InvariantCulture, out int count))
            {
                throw new InvalidOperationException(
                    $"Could not parse error count from text: '{text}'");
            }

            return count;
        }
        catch (PlaywrightException ex)
        {
            throw new InvalidOperationException(
                $"Failed to retrieve error count for key '{errorKey}'", ex);
        }
    }

    public async Task VerifyErrorMessageAsync(string expectedError)
    {
        if (string.IsNullOrWhiteSpace(expectedError))
        {
            throw new ArgumentException("Error message cannot be null or empty.", nameof(expectedError));
        }

        await Expect(Page.GetByText(expectedError, new() { Exact = true }))
            .ToBeVisibleAsync();
    }

    public async Task VerifyHintAsync(string expectedHint)
    {
        if (string.IsNullOrWhiteSpace(expectedHint))
        {
            throw new ArgumentException("Hint text cannot be null or empty.", nameof(expectedHint));
        }

        await Expect(Page.GetByText(expectedHint, new() { Exact = true }))
            .ToBeVisibleAsync();
    }

    /// <summary>
    /// Gets a specific error row by matching error type and message.
    /// </summary>
    private ILocator GetErrorRow(UploadErrorSummary expected)
    {
        return ErrorRows
            .Filter(new() { HasTextString = expected.ErrorType })
            .Filter(new() { HasTextString = expected.ErrorMessage });
    }

    /// <summary>
    /// Verifies all content fields within an error row.
    /// </summary>
    private static async Task VerifyRowContent(ILocator row, UploadErrorSummary expected)
    {
        await Expect(row).ToContainTextAsync(expected.Category);
        await Expect(row).ToContainTextAsync(expected.ErrorType);
        await Expect(row).ToContainTextAsync(expected.ErrorMessage);

        if (!string.IsNullOrWhiteSpace(expected.HintText))
        {
            await Expect(row.Locator(UploadLocators.Selectors.HintSelector))
                .ToHaveTextAsync(expected.HintText);
        }
    }
}
