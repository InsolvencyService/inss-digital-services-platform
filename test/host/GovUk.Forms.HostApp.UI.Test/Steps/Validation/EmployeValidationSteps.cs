using GovUk.Forms.HostApp.UI.Test.Coordinators;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Support;
using System.Globalization;

namespace GovUk.Forms.HostApp.UI.Test.Steps.Validation;

[Scope(Feature = "Employe Validation")]
[Binding]
public class EmployeValidationSteps
{
    private readonly UploadDocumentCoordinator _uploadDocumentCoordinator;
    private readonly UploadErrorDetailsCoordinator _uploadErrorDetailsCoordinator;
    private readonly UploadDocumentSummaryCoordinator _uploadDocumentSummaryCoordinator;
    private readonly ScenarioContext _scenarioContext;
    public EmployeValidationSteps(
        UploadDocumentCoordinator uploadDocumentCoordinator,
        UploadErrorDetailsCoordinator uploadErrorDetailsCoordinator,
        UploadDocumentSummaryCoordinator uploadDocumentSummaryCoordinator,
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
        switch (outcome)
        {
            case "accepted":
                await _uploadDocumentSummaryCoordinator.VerifySummaryPageIsDisplayedAsync();
                break;

            case "rejected":
                await _uploadErrorDetailsCoordinator.VerifyUploadErrorsPageIsDisplyedAsync();
                break;

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(outcome),
                    outcome,
                    "Outcome must be accepted or rejected.");
        }
    }

    [Then("the error summary should {string} with {string}")]
    public async Task ThenTheErrorSummaryShouldWith(string summaryBehaviour, string detailsBehaviour)
    {
        if (summaryBehaviour.StartsWith("not contain", StringComparison.OrdinalIgnoreCase))
        {
            await _uploadErrorDetailsCoordinator
                .VerifyErrorDetailsDoesNotContainAsync(summaryBehaviour);

            await _uploadErrorDetailsCoordinator
                .VerifyErrorDetailsDoesNotContainAsync(detailsBehaviour);

            return;
        }

        UploadErrorSummary expectedError = new(
            Category: "Employer",
            ErrorType: "Employer name",
            ErrorMessage: summaryBehaviour,
            HintText: detailsBehaviour
        );

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

        string errorMessage = _scenarioContext.Get<string>(ScenarioConstant.ErrorMessage);
        string employerName = _scenarioContext.Get<string>("EmployerName");

        UploadErrorSummary expectedError = new(
            Category: "Employer",
            ErrorType: "Employer name",
            ErrorMessage: errorMessage
        );

        await _uploadErrorDetailsCoordinator.OpenErrorDetailsAsync(expectedError);

        await _uploadErrorDetailsCoordinator.VerifyEmployerErrorSummaryIsDisplayedAsync();

        await _uploadErrorDetailsCoordinator.VerifyEmployeeErrorMessageSummaryAsync(
            new AffectedEmployee
            {
                Forename = ScenarioConstant.Forname,
                Surname = ScenarioConstant.Surname,

                DateOfBirth = DateTime
                .ParseExact(ScenarioConstant.DOB, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                .ToString("d/M/yyyy", CultureInfo.InvariantCulture),
                NiNumber = ScenarioConstant.NationalInsuranceNumber,
                CellValue = employerName
            });
    }


}
