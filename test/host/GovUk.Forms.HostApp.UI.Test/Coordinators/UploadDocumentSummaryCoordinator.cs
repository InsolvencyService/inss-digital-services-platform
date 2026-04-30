using GovUk.Forms.HostApp.UI.Test.Pages.Submit;
using GovUk.Forms.HostApp.UI.Test.Support;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators;

public class UploadDocumentSummaryCoordinator(
        IUploadDocumentSummaryPage summaryPage,
        ScenarioContext scenarioContext)
{
    public async Task VerifySummaryPageIsDisplayedAsync()
    {
        await summaryPage.VerifyPageIsDisplayedAsync();

        string uploadedFileName =
            scenarioContext.Get<string>(ScenarioConstant.UploadedFileName);

        await summaryPage.VerifyUploadedDocumentAsync(uploadedFileName);
    }

    public async Task SubmitAsync()
    {
        await summaryPage.ClickSubmitAsync();
    }

    public async Task ChangeUploadedDocumentAsync()
    {
        await summaryPage.ClickChangeAsync();
    }
}

