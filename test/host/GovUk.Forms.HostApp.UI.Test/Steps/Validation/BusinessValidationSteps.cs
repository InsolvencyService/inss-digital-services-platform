using GovUk.Forms.HostApp.UI.Test.Coordinators;
using GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Steps.Base;

namespace GovUk.Forms.HostApp.UI.Test.Steps.Validation;

[Scope(Feature = "Business Validation")]
[Binding]
public class BusinessValidationSteps : ValidationStepsBase
{
    private const string BusinessCategory = "Business";
    private const string BusinessNameErrorType = "Name of business";

    private readonly CheckYourAnswersCoordinator _checkYourAnswersCoordinator;

    public BusinessValidationSteps(
        UploadDocumentCoordinator uploadDocumentCoordinator,
        UploadErrorDetailsCoordinator uploadErrorDetailsCoordinator,
        ScenarioContext scenarioContext,
        CheckYourAnswersCoordinator checkYourAnswersCoordinator)
        : base(uploadDocumentCoordinator, uploadErrorDetailsCoordinator, scenarioContext)
    {
        _checkYourAnswersCoordinator = checkYourAnswersCoordinator;
    }

    [Given("the RP14 XML contains no business name")]
    public async Task GivenTheRp14XmlContainsNoBusinessName()
    {
        await UploadDocumentCoordinator.UploadRp14WithBusinessNameAsync(null);
    }

    [Given("the RP14 XML contains a business name of length {int}")]
    public async Task GivenTheRp14XmlContainsABusinessNameOfLength(int length)
    {
        string businessName = LengthHelper.AtMax(length);
        await UploadDocumentCoordinator.UploadRp14WithBusinessNameAsync(businessName);
    }

    [Given("the RP14 XML contains a company number of length {int}")]
    public async Task GivenTheRPXMLContainsACompanyNumberOfLength(int length)
    {
        string companyNumber = LengthHelper.AtMax(length);

        await UploadDocumentCoordinator
            .UploadRp14WithCompanyNumberAsync(companyNumber);
    }

    [Given("the RP14 XML contains a standard industrial classification of length {int}")]
    public async Task GivenTheRp14XmlContainsAStandardIndustrialClassificationOfLength(int length)
    {
        string standardIndustrialClassification = LengthHelper.AtMax(length);

        await UploadDocumentCoordinator
            .UploadRp14WithStandardIndustrialClassificationAsync(
                standardIndustrialClassification);
    }

    [Then("I should see the validation error {string}")]
    public async Task ThenIShouldSeeTheValidationError(string errorMessage)
    {
        UploadErrorSummary expectedError = new(
            Category: "",
            ErrorType: BusinessNameErrorType,
            ErrorMessage: errorMessage,
            HintText: null,
            ActionText: null);

        await UploadErrorDetailsCoordinator
            .VerifyErrorSummaryIsDisplayedAsync(expectedError);

        await UploadErrorDetailsCoordinator
            .VerifyValidationCategoryIsDisplayedAsync(BusinessCategory);
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
            Category: "",
            ErrorType: type,
            ErrorMessage: errorMessage,
            HintText: hintText,
            ActionText: null);

        await UploadErrorDetailsCoordinator
            .VerifyErrorSummaryIsDisplayedAsync(expectedError);
    }

}
