using GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Models.TestData;
using GovUk.Forms.HostApp.UI.Test.Steps.Base;
using GovUk.Forms.HostApp.UI.Test.Support;
using GovUk.Forms.HostApp.UI.Test.Tags;

namespace GovUk.Forms.HostApp.UI.Test.Steps.Validation;

[Scope(Feature = "Case Validation")]
[Binding]
public sealed class CaseValidationSteps : ValidationStepsBase
{
    private const string CaseReferenceKey = "CaseReference";
    private const string ErrorSummaryKey = "ErrorSummary";
    private const string CaseCategory = "Case";
    private const string CaseReferenceErrorType = "Case reference";

    public CaseValidationSteps(
        UploadDocumentCoordinator uploadDocumentCoordinator,
        UploadErrorDetailsCoordinator uploadErrorDetailsCoordinator,
        ScenarioContext scenarioContext)
        : base(uploadDocumentCoordinator, uploadErrorDetailsCoordinator, scenarioContext)
    {
    }

    [Given("the RP14A contains an employee row with no case reference")]
    public async Task GivenTheRp14aContainsAnEmployeeRowWithNoCaseReference()
    {
        await UploadDocumentCoordinator.UploadRp14aWithCaseReferenceAsync(string.Empty);
    }

    [Given("the RP14A contains a case reference {string}")]
    public async Task GivenTheRp14aContainsACaseReference(string caseReference)
    {
        await UploadDocumentCoordinator.UploadRp14aWithCaseReferenceAsync(caseReference);

        ScenarioContext.Set(caseReference, CaseReferenceKey);
    }

    [Given("the RP14 XML contains case reference {string}")]
    public async Task GivenTheRp14XmlContainsCaseReference(string caseReference)
    {
        await UploadDocumentCoordinator.UploadRp14WithCaseReferenceAsync(caseReference);
    }

    [Given(@"the RP14A contains (.*) invalid case references")]
    public async Task GivenTheRp14AContainsInvalidCaseReferences(int count)
    {
        List<string> invalidCaseReferences = [];

        // Create the requested number of invalid case references.
        // If the count is greater than the number of available test values,
        // reuse the values from the beginning of the list.
        for (int i = 0; i < count; i++)
        {
            invalidCaseReferences.Add(
                InvalidCaseReferences.All[
                    i % InvalidCaseReferences.All.Count]);
        }

        await UploadDocumentCoordinator.UploadRp14aWithCaseReferenceAsync(invalidCaseReferences.ToArray());

        ScenarioContext.Set(invalidCaseReferences, CaseReferenceKey);
    }

    [Given("the RP14A contains {int} employees with no case reference")]
    public async Task GivenTheRp14aContainsEmployeesWithNoCaseReference(int count)
    {
        string[] emptyCaseReferences = Enumerable.Repeat(string.Empty, count).ToArray();
        await UploadDocumentCoordinator.UploadRp14aWithCaseReferenceAsync(emptyCaseReferences);
    }

    [Given("the RP14A contains {int} employees with a case reference that is too long")]
    public async Task GivenTheRp14aContainsEmployeesWithCaseReferenceTooLong(int count)
    {
        await UploadDocumentCoordinator.UploadRp14aWithTooLongCaseReferencesAsync(count);
    }

    [Then("I should see the validation error {string}")]
    public async Task ThenIShouldSeeTheValidationError(string errorMessage)
    {
        UploadErrorSummary expectedError = BuildCaseErrorSummary(
            errorMessage,
            hintText: null);

        await UploadErrorDetailsCoordinator
            .VerifyErrorSummaryIsDisplayedAsync(expectedError);

        ScenarioContext.Set(expectedError, ErrorSummaryKey);
    }

    [Then("I should see the validation error {string} with the hint {string}")]
    public async Task ThenIShouldSeeTheValidationErrorWithTheHint(
        string errorMessage,
        string hintText)
    {
        UploadErrorSummary expectedError = BuildCaseErrorSummary(
            errorMessage,
            hintText);

        await UploadErrorDetailsCoordinator
            .VerifyErrorSummaryIsDisplayedAsync(expectedError);

        ScenarioContext.Set(expectedError, ErrorSummaryKey);
    }

    [Then("I should be able to view case reference error details on a table")]
    public async Task ThenIShouldBeAbleToViewCaseReferenceErrorDetailsOnATable()
    {
        UploadErrorSummary errorSummary = GetErrorSummaryFromContext();

        AffectedEmployee affectedEmployee = BuildAffectedEmployee(
            cellValue: "Not entered");

        await UploadErrorDetailsCoordinator.VerifyErrorDetailsAsync(
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

        await UploadErrorDetailsCoordinator.VerifyErrorDetailsAsync(
            errorSummary,
            affectedEmployee,
            ErrorDetailsHeaderType.CaseReference);
    }

    [Then("I should see the following RP14 validation errors")]
    public async Task ThenIShouldSeeTheFollowingRPValidationErrors(DataTable dataTable)
    {
        Error error = dataTable.CreateInstance<Error>();

        UploadErrorSummary expectedError = BuildCaseErrorSummary(error.Message, error.Hint, false);
        await UploadErrorDetailsCoordinator.VerifyErrorSummaryIsDisplayedAsync(expectedError);
    }

    [Then("I should see the following case reference validation errors")]
    public async Task ThenIShouldSeeTheFollowingCaseReferenceValidationErrors(DataTable dataTable)
    {
        Error error = dataTable.CreateInstance<Error>();
        UploadErrorSummary expectedError = BuildCaseErrorSummary(error.Message, error.Hint);
        await UploadErrorDetailsCoordinator.VerifyErrorSummaryIsDisplayedAsync(expectedError);

        ScenarioContext.Set(expectedError, ErrorSummaryKey);
    }

    [Then("I should be able to view case reference error details for multiple employees")]
    public async Task ThenIShouldBeAbleToViewCaseReferenceErrorDetailsForMultipleEmployees()
    {
        UploadErrorSummary errorSummary = GetErrorSummaryFromContext();

        List<AffectedEmployee> affectedEmployees =
         ScenarioContext.Get<List<AffectedEmployee>>(AffectedEmployeesKey);

        await UploadErrorDetailsCoordinator.VerifyErrorDetailsAsync(
            errorSummary,
            affectedEmployees,
            ErrorDetailsHeaderType.CaseReference);
    }



    private static UploadErrorSummary BuildCaseErrorSummary(
        string errorMessage,
        string? hintText,
        bool actionText = true)
    {
        return new UploadErrorSummary(
            Category: CaseCategory,
            ErrorType: CaseReferenceErrorType,
            ErrorMessage: errorMessage,
            HintText: hintText,
            ActionText: actionText ? "View details" : string.Empty);
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
            return ScenarioContext.Get<UploadErrorSummary>(ErrorSummaryKey);
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
            string caseReference = ScenarioContext.Get<string>(CaseReferenceKey);

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
