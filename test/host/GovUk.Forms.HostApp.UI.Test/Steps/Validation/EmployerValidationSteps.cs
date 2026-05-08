using GovUk.Forms.HostApp.UI.Test.Coordinators;
using GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Support;

namespace GovUk.Forms.HostApp.UI.Test.Steps.Validation;

[Scope(Feature = "Employer Validation")]
[Binding]
public class EmployerValidationSteps
{
    private readonly UploadDocumentCoordinator _uploadDocumentCoordinator;
    private readonly UploadErrorDetailsCoordinator _uploadErrorDetailsCoordinator;
    private readonly CheckYourAnswersCoordinator _uploadDocumentSummaryCoordinator;
    private readonly ScenarioContext _scenarioContext;
    public EmployerValidationSteps(
        UploadDocumentCoordinator uploadDocumentCoordinator,
        UploadErrorDetailsCoordinator uploadErrorDetailsCoordinator,
        CheckYourAnswersCoordinator uploadDocumentSummaryCoordinator,
        ScenarioContext scenarioContext)
    {
        _uploadDocumentCoordinator = uploadDocumentCoordinator;
        _uploadErrorDetailsCoordinator = uploadErrorDetailsCoordinator;
        _uploadDocumentSummaryCoordinator = uploadDocumentSummaryCoordinator;
        _scenarioContext = scenarioContext;
    }

    [Given("I have uploaded an RP14A file with employer name of length {int}")]
    public async Task GivenIHaveUploadedAnRP14AFileWithEmployerNameOfLength(int length)
    {
        string employerName = LengthHelper.AtMax(length);

        await _uploadDocumentCoordinator.UploadRp14aWithEmployerNameAsync(employerName);

        _scenarioContext.Set(employerName, "EmployerName");
    }

    [When("I submit the RP14A file")]
    public async Task WhenISubmitTheRPAFile()
    {
        await _uploadDocumentCoordinator.NavigateToSubmitPageAsync();
    }

    [Then(@"the submission should be {string}")]
    public async Task ThenSubmissionShouldBe(string outcome)
    {
        _scenarioContext.Set(outcome, ScenarioConstant.SubmissionOutcome);

        switch (outcome.ToLowerInvariant())
        {
            case "accepted":
                await _uploadDocumentSummaryCoordinator
                    .VerifySummaryPageIsDisplayedAsync();
                return;

            case "rejected":
                await _uploadErrorDetailsCoordinator
                    .VerifyUploadErrorPageIsDisplayedAsync();
                return;

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(outcome),
                    outcome,
                    "Outcome must be 'accepted' or 'rejected'.");
        }
    }

    [Then("the error summary should {string} with {string}")]
    public async Task ThenTheErrorSummaryShouldWith(
    string summaryBehaviour,
    string detailsBehaviour)
    {
        string outcome =
            _scenarioContext.Get<string>(ScenarioConstant.SubmissionOutcome);

        if (outcome.Equals("accepted", StringComparison.OrdinalIgnoreCase))
        {
            await _uploadDocumentSummaryCoordinator.VerifySummaryPageIsDisplayedAsync();
            return;
        }

        UploadErrorSummary expectedError = new(
            Category: "Employer",
            ErrorType: "Employer name",
            ErrorMessage: summaryBehaviour,
            HintText: detailsBehaviour);

        _scenarioContext.Set(expectedError);
        _scenarioContext.Set(summaryBehaviour, ScenarioConstant.ErrorMessage);

        await _uploadErrorDetailsCoordinator
            .VerifyErrorSummaryIsDisplayedAsync(expectedError);
    }

    [Then("I should be able to view error details")]
    public async Task ThenIShouldBeAbleToViewErrorDetails()
    {
        string outcome = _scenarioContext.Get<string>(ScenarioConstant.SubmissionOutcome);

        if (outcome.Equals("accepted", StringComparison.OrdinalIgnoreCase))
        {
            await _uploadDocumentSummaryCoordinator.VerifySummaryPageIsDisplayedAsync();
            return;
        }

        UploadErrorSummary expectedError =
            _scenarioContext.Get<UploadErrorSummary>();

        string employerName =
            _scenarioContext.Get<string>("EmployerName");

        AffectedEmployee affectedEmployee = new()
        {
            Forename = ScenarioConstant.Forname,
            Surname = ScenarioConstant.Surname,
            DateOfBirth = TestData.UiDateOfBirth(),
            NiNumber = ScenarioConstant.NationalInsuranceNumber,
            CellValue = employerName
        };

        await _uploadErrorDetailsCoordinator
            .VerifyErrorDetailsAsync(
              expectedError,
              affectedEmployee,
             ErrorDetailsHeaderType.EmployerName);
    }


}
