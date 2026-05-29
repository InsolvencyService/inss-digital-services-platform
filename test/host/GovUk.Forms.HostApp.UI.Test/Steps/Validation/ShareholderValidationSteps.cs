using Bogus;
using GovUk.Forms.HostApp.UI.Test.Coordinators;
using GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Models.TestData;
using GovUk.Forms.HostApp.UI.Test.Steps.Base;

namespace GovUk.Forms.HostApp.UI.Test.Steps.Validation;

[Scope(Feature = "Shareholder Validation")]
[Binding]
public sealed class ShareholderValidationSteps : ValidationStepsBase
{
    private readonly CheckYourAnswersCoordinator _checkYourAnswersCoordinator;
    public ShareholderValidationSteps(
        UploadDocumentCoordinator uploadDocumentCoordinator,
        UploadErrorDetailsCoordinator uploadErrorDetailsCoordinator,
        ScenarioContext scenarioContext,
        CheckYourAnswersCoordinator checkYourAnswersCoordinator)
        : base(uploadDocumentCoordinator, uploadErrorDetailsCoordinator, scenarioContext)
    {
        _checkYourAnswersCoordinator = checkYourAnswersCoordinator;
    }

    [Given("the RP14 XML contains shareholder percentage {string}")]
    public async Task GivenTheRp14XmlContainsShareholderPercentage(string percentage)
    {
        await UploadDocumentCoordinator.UploadRp14WithShareholderAsync(
            shareholderNumber: 1,
            fullName: new Faker().Name.FullName(),
            numberOfShares: "40000",
            percentage: percentage);
    }

    [Given("the RP14 XML contains a shareholder name of length {int}")]
    public async Task GivenTheRp14XmlContainsAShareholderNameOfLength(int length)
    {
        string shareholderName = LengthHelper.AtMax(length);

        await UploadDocumentCoordinator.UploadRp14WithShareholderAsync(
            shareholderNumber: 1,
            fullName: shareholderName,
            numberOfShares: "40000",
            percentage: "15");
    }

    [Given("the RP14 XML contains {int} shareholder names of length {int}")]
    public async Task GivenTheRp14XmlContainsShareholderNamesOfLength(
     int shareholderCount,
     int length)
    {
        string shareholderName = LengthHelper.AtMax(length);

        await UploadDocumentCoordinator.UploadRp14WithShareholdersAsync(
            shareholderCount,
            fullName: shareholderName,
            numberOfShares: "1000000",
            percentage: "20");
    }

    [Given("the RP14 XML contains {int} shareholder percentages {string}")]
    public async Task GivenTheRp14XmlContainsShareholderPercentages(int shareholderCount, string percentage)
    {
        await UploadDocumentCoordinator.UploadRp14WithShareholdersAsync(
            shareholderCount,
            fullName: new Faker().Name.FullName(),
            numberOfShares: "40000",
            percentage: percentage);
    }

    [Then("I should see the following shareholder validation errors")]
    public async Task ThenIShouldSeeTheFollowingShareholderValidationErrors(DataTable dataTable)
    {
        Error error = dataTable.CreateInstance<Error>();
        UploadErrorSummary expectedError = new(
            Category: string.Empty,
            ErrorType: error.Type,
            ErrorMessage: error.Message,
            HintText: error.Hint,
            ActionText: null);

        await UploadErrorDetailsCoordinator.VerifyErrorSummaryIsDisplayedAsync(expectedError);
    }

    [Then("no invalid shareholder percentage error should be displayed")]
    public async Task ThenNoInvalidShareholderPercentageErrorShouldBeDisplayed()
    {
        await _checkYourAnswersCoordinator.VerifyCheckYourAnswersPageIsDisplayedAsync();
    }

    [Then("the error summary should {string} with {string} With {string}")]
    public async Task ThenTheErrorSummaryShouldWithWith(string errorMessage, string hintText, string type)
    {
        if (errorMessage.Equals("none", StringComparison.OrdinalIgnoreCase))
        {
            await _checkYourAnswersCoordinator.VerifyCheckYourAnswersPageIsDisplayedAsync();
            return;
        }

        UploadErrorSummary expectedError = new(
           Category: string.Empty,
           ErrorType: type,
           ErrorMessage: errorMessage,
           HintText: hintText,
           ActionText: null);

        await UploadErrorDetailsCoordinator.VerifyErrorSummaryIsDisplayedAsync(expectedError);
    }
}
