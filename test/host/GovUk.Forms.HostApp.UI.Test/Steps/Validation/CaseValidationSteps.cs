using GovUk.Forms.HostApp.UI.Test.Coordinators;
using GovUk.Forms.HostApp.UI.Test.Models;

namespace GovUk.Forms.HostApp.UI.Test.Steps.Validation;

[Scope(Feature = "Case Validation")]
[Binding]
public class CaseValidationSteps
{
    private readonly UploadDocumentCoordinator _uploadDocumentCoordinator;
    private readonly UploadErrorDetailsCoordinator _uploadErrorDetailsCoordinator;
    public CaseValidationSteps(
        UploadDocumentCoordinator uploadDocumentCoordinator,
        UploadErrorDetailsCoordinator uploadErrorDetailsCoordinator)
    {
        _uploadDocumentCoordinator = uploadDocumentCoordinator;
        _uploadErrorDetailsCoordinator = uploadErrorDetailsCoordinator;
    }

    [Given("the RP14A contains an employee row with no case reference")]
    public async Task GivenTheRPAContainsAnEmployeeRowWithNoCaseReference()
    {
        await _uploadDocumentCoordinator.UploadRp14aWithCaseReferenceAsync(string.Empty);
    }

    [Given("the RP14A contains a case reference {string}")]
    public async Task GivenTheRP14AContainsACaseReference(string invalidCaseReference)
    {
        await _uploadDocumentCoordinator.UploadRp14aWithCaseReferenceAsync(invalidCaseReference);
    }

    [When("I attempt to submit the RP14A")]
    public async Task WhenIAttemptToSubmitTheRPA()
    {
        await _uploadDocumentCoordinator.NavigateToSubmitPageAsync();
    }

    [Then("I should see the validation error {string}")]
    public async Task ThenIShouldSeeTheValidationError(string errorMesage)
    {
        UploadErrorSummary expectedError = new(
         Category: "Case",
         ErrorType: "Case reference",
         ErrorMessage: errorMesage
        );
        await _uploadErrorDetailsCoordinator.VerifyErrorSummaryIsDisplayedAsync(expectedError);
    }

    [Then("I should see the validation error {string} with the hint {string}")]
    public async Task ThenIShouldSeeTheValidationErrorWithTheHint(string errorMessage, string hint)
    {
        UploadErrorSummary expectedError = new(
               Category: "Case",
               ErrorType: "Case reference",
               HintText: hint,
               ErrorMessage: errorMessage
        );
        await _uploadErrorDetailsCoordinator.VerifyErrorSummaryIsDisplayedAsync(expectedError);
    }

    [Then("I should be able to view error details")]
    public static async Task ThenIShouldBeAbleToViewErrorDetails()
    {

    }

}
