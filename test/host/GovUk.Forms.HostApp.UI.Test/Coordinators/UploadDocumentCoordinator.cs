using GovUk.Forms.HostApp.UI.Test.Builders;
using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Extensions;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Pages.Common;
using GovUk.Forms.HostApp.UI.Test.Pages.Upload;
using GovUk.Forms.HostApp.UI.Test.Support;
using System.Globalization;
using static GovUk.Forms.HostApp.UI.Test.Models.TestData;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators;

public class UploadDocumentCoordinator(
    IUploadDocumentPage uploadDocumentPage,
    ScenarioContext scenarioContext,
    IReqnrollOutputHelper outputHelper,
    IAllureReportingHelper allure,
    IPlaywrightDriver playwrightDriver,
    ICommonPage commonPage,
    TestArtifacts testArtifacts) : BaseCoordinator(testArtifacts)
{
    public async Task VerifyUploadDocumentPageIsDisplayedAsync()
    {
        await allure.StepAsync("File upload page is displayed", async () =>
        {
            await uploadDocumentPage.WaitForPageToLoadAsync();

            await allure.AttachScreenshotAsync(
                playwrightDriver.Page,
                "Upload File Page");
        });
    }

    public async Task ClickOnContinueButtonAsync()
    {
        await uploadDocumentPage.ClickOnContinueButtonAsync();
    }

    public async Task ClickOnBackButtonAsync()
    {
        await uploadDocumentPage.ClickOnBackButtonAsync();
    }

    public async Task UploadFileAsync(string filePath)
    {
        await UploadFileInternalAsync(
            filePath,
            $"Upload file '{Path.GetFileName(filePath)}'");
    }

    public async Task UploadValidRp14aAsync()
    {
        string xml = new Rp14aBuilder().Build();

        await UploadRp14aAsync(xml, "Upload valid RP14A file");
    }

    public async Task UploadRp14aWithCaseReferenceAsync(string caseReference)
    {
        string xml = new Rp14aBuilder()
            .WithCaseReference(caseReference)
            .Build();

        await UploadRp14aAsync(
            xml,
            $"Upload RP14A file with case reference '{caseReference}'");
    }

    public async Task UploadRp14aWithEmployerNameAsync(string employerName)
    {
        string xml = new Rp14aBuilder()
            .WithCaseReference("CN10300112")
            .WithEmployerName(employerName)
            .Build();

        await UploadRp14aAsync(
            xml,
            $"Upload RP14A file with employer name '{employerName}'");
    }
    public async Task UploadRp14aWithEmployeeNameAsync(string surname, string forname, string title = "Ms")
    {
        string xml = new Rp14aBuilder()
            .WithCaseReference("CN11300112")
            .WithEmployeeName(surname, forname, title)
            .Build();

        await UploadRp14aAsync(
            xml,
            $"Upload RP14A file with employee surname '{surname}' forname '{forname}' and title '{title}'");
    }
    public async Task UploadRp14aWithEmployerNameLengthAsync(int length)
    {
        string xml = new Rp14aBuilder()
            .WithEmployerNameLength(length)
            .Build();

        await UploadRp14aAsync(
            xml,
            $"Upload RP14A file with employer name length {length}");
    }

    public async Task UploadAndSubmitRp14aWithEmployerNameLengthAsync(int length)
    {
        await UploadRp14aWithEmployerNameLengthAsync(length);
        await NavigateToSubmitPageAsync();
    }

    private async Task UploadRp14aAsync(string xml, string stepName)
    {
        string filePath = await Rp14aFileFactory.CreateAsync(xml);

        await UploadFileInternalAsync(filePath, stepName);
    }

    private async Task UploadFileInternalAsync(string filePath, string stepName)
    {
        await allure.StepAsync(stepName, async () =>
        {
            await uploadDocumentPage.UploadFileAsync(filePath);

            StoreUploadedFileInScenarioContext(filePath);
            AttachUploadedFile(filePath);

            await allure.AttachScreenshotAsync(
                playwrightDriver.Page,
                "After Upload File");
        });
    }
    public async Task UploadRp14aWithArrearsOfPayOwedAsync(string arrearsOfPay)
    {
        string xml = new Rp14aBuilder()
            .WithCaseReference("CN90300112")
            .WithArrearsOfPayPeriod(
                periodNumber: 1,
                startDate: "2020-01-10",
                endDate: "2020-01-11",
                amountOwed: arrearsOfPay,
                payType: "overtime")
            .Build();

        await UploadRp14aAsync(
            xml,
            $"Upload RP14A file with arrears of pay owed '{arrearsOfPay}'");
    }
    private void StoreUploadedFileInScenarioContext(string filePath)
    {
        string fileName = Path.GetFileName(filePath);

        scenarioContext.Set(fileName, ScenarioConstant.UploadedFileName);
        scenarioContext.Set(filePath, ScenarioConstant.UploadedFilePath);
    }

    private void AttachUploadedFile(string filePath)
    {
        string fileName = Path.GetFileName(filePath);

        outputHelper.WriteLine($"Uploading file: {fileName}");
        outputHelper.WriteLine($"Full path: {filePath}");

        outputHelper.AddAttachmentAsLink(filePath);

        allure.AttachFile(
            filePath,
            $"Uploaded RP14A File - {fileName}");
    }

    public async Task VerifyThatFileIsUploadedAsync()
    {
        string expectedFileName =
            scenarioContext.Get<string>(ScenarioConstant.UploadedFileName);

        string actualUploadedFileName =
            await uploadDocumentPage.GetUploadedFileNameAsync();

        Assert.That(
            actualUploadedFileName,
            Is.EqualTo(expectedFileName),
            $"Expected uploaded file '{expectedFileName}', but actual was '{actualUploadedFileName}'.");
    }

    public async Task VerifyOnlyOneFileUploadedAsync()
    {
        string expectedFileName =
            scenarioContext.Get<string>(ScenarioConstant.UploadedFileName);

        IReadOnlyList<string> uploadedFiles =
            await uploadDocumentPage.GetUploadedFileNamesAsync();

        int matchingFileCount = uploadedFiles.Count(file =>
            file.Equals(expectedFileName, StringComparison.OrdinalIgnoreCase));

        Assert.That(
            matchingFileCount,
            Is.EqualTo(1),
            $"Expected only one instance of '{expectedFileName}', but found {matchingFileCount}. " +
            $"Actual files: {string.Join(", ", uploadedFiles)}");
    }

    public async Task<string> CaptureUploadDocumentPageVisualAsync()
    {
        return await CapturePageVisualAsync(
            () => commonPage.CaptureVisualAsync(playwrightDriver.Page),
            ScenarioConstant.UploadPage);
    }

    public async Task ExpandCommonIssuesWhenUploadingRP14AFormsAsync()
    {
        await uploadDocumentPage.ExpandCommonIssuesWhenUploadingRP14AFormsAsync();
    }

    public async Task NavigateToFeedbackPageAsync()
    {
        IPage feedbackPage =
            await NavigateAsync(uploadDocumentPage.ClickOnGiveFeedbackLinkAsync);

        scenarioContext.Set(feedbackPage);
    }

    public async Task NavigateToSubmitPageAsync()
    {
        await allure.StepAsync("Navigate to submit page", async () =>
        {
            await uploadDocumentPage.ClickOnContinueButtonAsync();

            await allure.AttachScreenshotAsync(
                playwrightDriver.Page,
                "Submit Page");
        });
    }

    public async Task VerifyInvalidFileExtensionErrorAsync(UploadFileError uploadFileError)
    {
        await uploadDocumentPage.VerifyUploadFileErrorAsync(uploadFileError);
    }

    public async Task UploadRp14aWithInvalidArrearsOfPayOwedAsync(int count)
    {
        Rp14aBuilder builder = new Rp14aBuilder()
            .WithCaseReference("CN90370112");

        string[] invalidValues =
        [
            "15.3",
            "12.345",
            "-100"
        ];

        List<AffectedEmployee> affectedEmployees = [];

        for (int i = 1; i <= count; i++)
        {
            string invalidValue = invalidValues[(i - 1) % invalidValues.Length];

            builder.WithArrearsOfPayPeriod(
                periodNumber: i,
                startDate: $"2020-01-{9 + i:00}",
                endDate: $"2020-01-{10 + i:00}",
                amountOwed: invalidValue,
                payType: "overtime");

            affectedEmployees.Add(new AffectedEmployee
            {
                Surname = ScenarioConstant.Surname,
                Forename = ScenarioConstant.Forname,
                DateOfBirth = DateTime
                    .ParseExact(ScenarioConstant.DOB, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                    .ToString("d/M/yyyy", CultureInfo.InvariantCulture),
                NiNumber = ScenarioConstant.NationalInsuranceNumber,
                CellValue = invalidValue
            });
        }

        scenarioContext.Set(affectedEmployees, "AffectedEmployees");

        string xml = builder.Build();

        await UploadRp14aAsync(
            xml,
            $"Upload RP14A file with {count} invalid arrears of pay owed");
    }

    public async Task UploadRp14aWithMissingNationalInsuranceNumberAsync(string insuranceNumber)
    {
        string xml = new Rp14aBuilder()
            .WithCaseReference("CN90300132")
            .WithNationalInsuranceNumber(insuranceNumber)
            .Build();

        await UploadRp14aAsync(
            xml,
            "Upload RP14A file with missing national insurance number");
    }

    public async Task UploadRp14aWithMoneyOwedToEmployerAsync(string moneyOwed)
    {
        string xml = new Rp14aBuilder()
            .WithCaseReference("CN10300112")
            .WithMoneyOwedToEmployer(moneyOwed)
            .Build();

        await UploadRp14aAsync(
            xml,
            $"Upload RP14A file with money owed to employer '{moneyOwed}'");
    }
    public async Task UploadRp14aWithEmploymentDatesAsync(string startDate, string endDate)
    {
        string xml = new Rp14aBuilder()
            .WithCaseReference("CN90350142")
            .WithEmploymentDates(startDate, endDate)
            .Build();

        await UploadRp14aAsync(
            xml,
            $"Upload RP14A file with employment dates {startDate} to {endDate}");
    }

    public async Task UploadRp14aWithArrearsDatesAsync(string startDate, string endDate)
    {
        string xml = new Rp14aBuilder()
            .WithCaseReference("CN90370152")
            .WithArrearsOfPayPeriod(
                periodNumber: 1,
                startDate: startDate,
                endDate: endDate,
                amountOwed: "100",
                payType: "overtime")
            .Build();

        await UploadRp14aAsync(
            xml,
            $"Upload RP14A file with invalid arrears dates {startDate} - {endDate}");
    }
}
