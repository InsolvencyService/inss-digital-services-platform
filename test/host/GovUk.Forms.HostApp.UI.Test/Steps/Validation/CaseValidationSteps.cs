using GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Models.TestData;
using GovUk.Forms.HostApp.UI.Test.Support;
using GovUk.Forms.HostApp.UI.Test.Tags;

namespace GovUk.Forms.HostApp.UI.Test.Steps.Validation;

[Scope(Feature = "Case Validation")]
[Binding]
public sealed class CaseValidationSteps
{
    private const string CaseReferenceKey = "CaseReference";
    private const string ErrorSummaryKey = "ErrorSummary";
    private const string CaseCategory = "Case";
    private const string CaseReferenceErrorType = "Case reference";

    private readonly UploadDocumentCoordinator _uploadDocumentCoordinator;
    private readonly UploadErrorDetailsCoordinator _uploadErrorDetailsCoordinator;
    private readonly ScenarioContext _scenarioContext;

    public CaseValidationSteps(
        UploadDocumentCoordinator uploadDocumentCoordinator,
        UploadErrorDetailsCoordinator uploadErrorDetailsCoordinator,
        ScenarioContext scenarioContext)
    {
        _uploadDocumentCoordinator = uploadDocumentCoordinator;
        _uploadErrorDetailsCoordinator = uploadErrorDetailsCoordinator;
        _scenarioContext = scenarioContext;
    }

    [Given("the RP14A contains an employee row with no case reference")]
    public async Task GivenTheRp14aContainsAnEmployeeRowWithNoCaseReference()
    {
        await _uploadDocumentCoordinator.UploadRp14aWithCaseReferenceAsync(string.Empty);
    }

    [Given("the RP14A contains a case reference {string}")]
    public async Task GivenTheRp14aContainsACaseReference(string caseReference)
    {
        await _uploadDocumentCoordinator.UploadRp14aWithCaseReferenceAsync(caseReference);

        _scenarioContext.Set(caseReference, CaseReferenceKey);
    }

    [When("I attempt to submit the RP14A")]
    public async Task WhenIAttemptToSubmitTheRp14a()
    {
        await _uploadDocumentCoordinator.NavigateToSubmitPageAsync();
    }

    [Then("I should see the validation error {string}")]
    public async Task ThenIShouldSeeTheValidationError(string errorMessage)
    {
        UploadErrorSummary expectedError = BuildCaseErrorSummary(
            errorMessage,
            hintText: null);

        await _uploadErrorDetailsCoordinator
            .VerifyErrorSummaryIsDisplayedAsync(expectedError);

        _scenarioContext.Set(expectedError, ErrorSummaryKey);
    }

    [Then("I should see the validation error {string} with the hint {string}")]
    public async Task ThenIShouldSeeTheValidationErrorWithTheHint(
        string errorMessage,
        string hintText)
    {
        UploadErrorSummary expectedError = BuildCaseErrorSummary(
            errorMessage,
            hintText);

        await _uploadErrorDetailsCoordinator
            .VerifyErrorSummaryIsDisplayedAsync(expectedError);

        _scenarioContext.Set(expectedError, ErrorSummaryKey);
    }

    [Then("I should be able to view case reference error details on a table")]
    public async Task ThenIShouldBeAbleToViewCaseReferenceErrorDetailsOnATable()
    {
        UploadErrorSummary errorSummary = GetErrorSummaryFromContext();

        AffectedEmployee affectedEmployee = BuildAffectedEmployee(
            cellValue: string.Empty);

        await _uploadErrorDetailsCoordinator.VerifyErrorDetailsAsync(
            errorSummary,
            affectedEmployee,
            ErrorDetailsHeaderType.CaseReference);
    }

    [Then("I should be able to view case reference error details")]
    public async Task ThenIShouldBeAbleToViewCaseReferenceErrorDetails()
    {
        UploadErrorSummary errorSummary = GetErrorSummaryFromContext();
        string caseReference = GetCaseReferenceFromContext();

        AffectedEmployee affectedEmployee = BuildAffectedEmployee(caseReference);

        await _uploadErrorDetailsCoordinator.VerifyErrorDetailsAsync(
            errorSummary,
            affectedEmployee,
            ErrorDetailsHeaderType.CaseReference);
    }

    private static UploadErrorSummary BuildCaseErrorSummary(
        string errorMessage,
        string? hintText)
    {
        return new UploadErrorSummary(
            Category: CaseCategory,
            ErrorType: CaseReferenceErrorType,
            ErrorMessage: errorMessage,
            HintText: hintText);
    }

    private static AffectedEmployee BuildAffectedEmployee(string cellValue)
    {
        return new AffectedEmployee
        {
            Forename = ScenarioConstant.Forename,
            Surname = ScenarioConstant.Surname,
            DateOfBirth = TestFactory.UiDateOfBirth(),
            NiNumber = ScenarioConstant.NationalInsuranceNumber,
            CellValue = cellValue
        };
    }

    private UploadErrorSummary GetErrorSummaryFromContext()
    {
        try
        {
            return _scenarioContext.Get<UploadErrorSummary>(ErrorSummaryKey);
        }
        catch (KeyNotFoundException ex)
        {
            throw new InvalidOperationException(
                "Error summary was not found in ScenarioContext. " +
                "Ensure the validation error summary step runs before viewing error details.",
                ex);
        }
    }

    private string GetCaseReferenceFromContext()
    {
        try
        {
            string caseReference = _scenarioContext.Get<string>(CaseReferenceKey);

            if (string.IsNullOrWhiteSpace(caseReference))
            {
                throw new InvalidOperationException(
                    "Case reference was found in ScenarioContext but it was null or empty.");
            }

            return caseReference;
        }
        catch (KeyNotFoundException ex)
        {
            throw new InvalidOperationException(
                $"Case reference was not found in ScenarioContext using key '{CaseReferenceKey}'. " +
                "Ensure the Given step with a case reference runs before this step.",
                ex);
        }
    }
}
