using Bogus;
using GovUk.Forms.HostApp.UI.Test.Coordinators;
using GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Models.TestData;
using GovUk.Forms.HostApp.UI.Test.Steps.Base;

namespace GovUk.Forms.HostApp.UI.Test.Steps.Validation;

[Scope(Feature = "Director Validation")]
[Binding]
public sealed class DirectorValidationSteps : ValidationStepsBase
{
    private const string DirectorCategory = "Director national insurance number";

    private readonly CheckYourAnswersCoordinator _checkYourAnswersCoordinator;

    public DirectorValidationSteps(
        UploadDocumentCoordinator uploadDocumentCoordinator,
        UploadErrorDetailsCoordinator uploadErrorDetailsCoordinator,
        ScenarioContext scenarioContext,
        CheckYourAnswersCoordinator checkYourAnswersCoordinator)
        : base(uploadDocumentCoordinator, uploadErrorDetailsCoordinator, scenarioContext)
    {
        _checkYourAnswersCoordinator = checkYourAnswersCoordinator;
    }

    [Given("the RP14 XML contains director national insurance number {string}")]
    public async Task GivenTheRPXMLContainsDirectorNationalInsuranceNumber(string niNumber)
    {
        await UploadDocumentCoordinator.UploadRp14WithDirectorAsync(
            directorNumber: 1,
            surname: new Faker().Name.LastName(),
            initials: "J",
            nino: niNumber);
    }

    [Given("the RP14 XML contains {int} directors with national insurance number {string}")]
    public async Task GivenTheRp14XmlContainsDirectorsWithNationalInsuranceNumber(
    int directorCount,
    string niNumber)
    {
        await UploadDocumentCoordinator.UploadRp14WithDirectorsAsync(
            directorCount,
            surname: new Faker().Name.LastName(),
            initials: "TS",
            nino: niNumber);
    }

    [Given("the RP14 XML contains director initials of length {int}")]
    public async Task GivenTheRp14XmlContainsDirectorInitialsOfLength(int length)
    {

        await UploadDocumentCoordinator.UploadRp14WithDirectorAsync(
            directorNumber: 1,
            surname: new Faker().Name.LastName(),
            initials: LengthHelper.AtMax(length),
            nino: "KH557994B");
    }

    [Given("the RP14 XML contains director surname of length {int}")]
    public async Task GivenTheRp14XmlContainsDirectorSurnameOfLength(int length)
    {

        await UploadDocumentCoordinator.UploadRp14WithDirectorAsync(
            directorNumber: 1,
            surname: LengthHelper.AtMax(length),
            initials: "J",
            nino: "KH557994B");
    }

    [Given("the RP14 XML contains {int} director surnames exceeding the maximum length")]
    public async Task GivenTheRp14XmlContainsDirectorSurnamesExceedingTheMaximumLength(int count)
    {
        await UploadDocumentCoordinator.UploadRp14WithDirectorSurnamesAsync(count, LengthHelper.AtMax(101));
    }

    [Then("I should see the following director validation errors")]
    public async Task ThenIShouldSeeTheFollowingDirectorValidationErrors(DataTable dataTable)
    {
        Error error = dataTable.CreateInstance<Error>();
        UploadErrorSummary expectedError = new(
            Category: DirectorCategory,
            ErrorType: error.Type,
            ErrorMessage: error.Message,
            HintText: error.Hint,
            ActionText: null);

        await UploadErrorDetailsCoordinator.VerifyErrorSummaryIsDisplayedAsync(expectedError);
    }

    [Then("no invalid director national insurance number format error should be displayed")]
    public async Task ThenNoInvalidDirectorNationalInsuranceNumberFormatErrorShouldBeDisplayed()
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

    [Then("I should see the following director surnames validation errors")]
    public async Task ThenIShouldSeeTheFollowingDirectorSurnamesValidationErrors(DataTable dataTable)
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


}
