using Bogus;
using GovUk.Forms.HostApp.UI.Test.Coordinators;
using GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;
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
}
