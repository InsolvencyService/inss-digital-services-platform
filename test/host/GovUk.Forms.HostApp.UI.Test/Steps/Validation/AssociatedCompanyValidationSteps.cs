using GovUk.Forms.HostApp.UI.Test.Coordinators;
using GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Models.TestData;
using GovUk.Forms.HostApp.UI.Test.Steps.Base;

namespace GovUk.Forms.HostApp.UI.Test.Steps.Validation;

[Scope(Feature = "Associated Company Validation")]
[Binding]
public class AssociatedCompanyValidationSteps : ValidationStepsBase
{
    private readonly CheckYourAnswersCoordinator _checkYourAnswersCoordinator;
    public AssociatedCompanyValidationSteps(
        UploadDocumentCoordinator uploadDocumentCoordinator,
        UploadErrorDetailsCoordinator uploadErrorDetailsCoordinator,
        ScenarioContext scenarioContext,
        CheckYourAnswersCoordinator checkYourAnswersCoordinator)
        : base(uploadDocumentCoordinator, uploadErrorDetailsCoordinator, scenarioContext)
    {
        _checkYourAnswersCoordinator = checkYourAnswersCoordinator;
    }

    [Given("the RP14 XML contains {int} associated companies with name longer than {int} characters")]
    public async Task GivenTheRp14XmlContainsAssociatedCompaniesWithNameLongerThan60Characters(int associatedCompanyCount, int length)
    {
        string companyName = LengthHelper.OverMax(length);
        await UploadDocumentCoordinator.UploadRp14WithAssociatedCompanyNamesAsync(associatedCompanyCount, companyName);
    }

    [Given("the RP14 XML contains an associated company name of length {int}")]
    public async Task GivenTheRPXMLContainsAnAssociatedCompanyNameOfLength(int length)
    {
        string companyName = LengthHelper.AtMax(length);
        const int associatedCompanyCount = 1;

        await UploadDocumentCoordinator.UploadRp14WithAssociatedCompanyNamesAsync(associatedCompanyCount, companyName);
    }

    [Given("the RP14 XML contains {int} associated companies with {int} characters")]
    [Given("the RP14 XML contains {int} associated companies with reason for association of length {int}")]
    public async Task GivenTheRp14XmlContainsAssociatedCompaniesWithReasonForAssociationOfLength(int associatedCompanyCount, int length)
    {
        string reasonForAssociation = LengthHelper.AtMax(length);

        await UploadDocumentCoordinator.UploadRp14WithAssociatedCompanyReasonsAsync(associatedCompanyCount, reasonForAssociation);
    }

    [Given("the RP14 XML contains an associated company number of length {int}")]
    public async Task GivenTheRPXMLContainsAnAssociatedCompanyNumberOfLength(int length)
    {
        const int associatedCompanyCount = 1;
        string companyNumber = LengthHelper.AtMax(length);

        await UploadDocumentCoordinator.UploadRp14WithAssociatedCompanyNumbersAsync(associatedCompanyCount, companyNumber);
    }

    [Given("the RP14 XML contains {int} associated company numbers exceeding the maximum length")]
    public async Task GivenTheRPXMLContainsAssociatedCompanyNumbersExceedingTheMaximumLength(int count)
    {
        string companyNumber = LengthHelper.OverMax(9);
        await UploadDocumentCoordinator.UploadRp14WithAssociatedCompanyNumbersAsync(count, companyNumber);
    }

    [Given("the RP14 XML contains {int} associated companies with company number longer than {int} characters")]
    public async Task GivenTheRPXMLContainsAssociatedCompaniesWithCompanyNumberLongerThanCharacters(int companyCount, int length)
    {
        string companyNumber = LengthHelper.OverMax(length);
        await UploadDocumentCoordinator.UploadRp14WithAssociatedCompanyNumbersAsync(companyCount, companyNumber);
    }

    [Given("the RP14 XML contains an employment continuity employer name of length {int}")]
    public async Task GivenTheRPXMLContainsAnEmploymentContinuityEmployerNameOfLength(int length)
    {
        string employerName = LengthHelper.AtMax(length);
        await UploadDocumentCoordinator.UploadRp14WithEmploymentContinuityEmployerNameAsync(employerName);
    }

    [Given("the RP14 XML contains a transfer to name of length {int}")]
    public async Task GivenTheRPXMLContainsATransferToNameOfLength(int length)
    {
        string transferToName = LengthHelper.AtMax(length);
        await UploadDocumentCoordinator.UploadRp14WithTransferToNameAsync(transferToName);
    }

    [Then("I should see the following associated company validation errors")]
    public async Task ThenIShouldSeeTheFollowingAssociatedCompanyValidationErrors(DataTable dataTable)
    {
        Error error = dataTable.CreateInstance<Error>();
        UploadErrorSummary expectedError = new(
            Category: "",
            ErrorType: error.Type,
            ErrorMessage: error.Message,
            HintText: error.Hint,
            ActionText: null);

        await UploadErrorDetailsCoordinator.VerifyErrorSummaryIsDisplayedAsync(expectedError);
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
