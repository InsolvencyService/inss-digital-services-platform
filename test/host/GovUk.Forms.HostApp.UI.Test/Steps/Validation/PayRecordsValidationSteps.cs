using GovUk.Forms.HostApp.UI.Test.Coordinators;
using GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Models.TestData;
using GovUk.Forms.HostApp.UI.Test.Steps.Base;

namespace GovUk.Forms.HostApp.UI.Test.Steps.Validation;

[Scope(Feature = "Pay Records Validation")]
[Binding]
public class PayRecordsValidationSteps : ValidationStepsBase
{
    private readonly CheckYourAnswersCoordinator _checkYourAnswersCoordinator;
    public PayRecordsValidationSteps(
        UploadDocumentCoordinator uploadDocumentCoordinator,
        UploadErrorDetailsCoordinator uploadErrorDetailsCoordinator,
        ScenarioContext scenarioContext,
        CheckYourAnswersCoordinator checkYourAnswersCoordinator)
        : base(uploadDocumentCoordinator, uploadErrorDetailsCoordinator, scenarioContext)
    {
        _checkYourAnswersCoordinator = checkYourAnswersCoordinator;
    }

    [Given("the RP14 XML does not contain Pay records contact name")]
    public async Task GivenTheRPXMLDoesNotContainPayRecordsContactName()
    {
        await UploadDocumentCoordinator.UploadRp14WithPayRecordsContactNameAsync(null);
    }

    [Given("the RP14 XML contains a Pay records contact name of length {int}")]
    public async Task GivenTheRPXMLContainsAPayRecordsContactNameOfLength(int length)
    {
        await UploadDocumentCoordinator.UploadRp14WithPayRecordsContactNameAsync(LengthHelper.AtMax(length));
    }

    [Given("the RP14 XML contains a Pay records contact phone number of length {int}")]
    public async Task GivenTheRPXMLContainsAPayRecordsContactPhoneNumberOfLength(int lenght)
    {
        await UploadDocumentCoordinator.UploadRp14WithPayRecordsContactPhoneNumberAsync(LengthHelper.AtMax(lenght));
    }
    [Given("the RP14 XML contains a Pay records contact email address of length {int}")]
    public async Task GivenTheRp14XmlContainsAPayRecordsContactEmailAddressOfLength(int length)
    {
        string emailAddress = CreateEmailAddressOfLength(length);
        await UploadDocumentCoordinator.UploadRp14WithPayRecordsContactEmailAddressAsync(emailAddress);
    }

    [Then("I should see the following pay records contact validation errors")]
    public async Task ThenIShouldSeeTheFollowingPayRecordsContactValidationErrors(DataTable dataTable)
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


    private static string CreateEmailAddressOfLength(int length)
    {
        const string domain = "@test.com";

        if (length <= domain.Length)
        {
            throw new ArgumentException(
                $"Email length must be greater than {domain.Length}. Received: {length}",
                nameof(length));
        }

        return $"{new string('a', length - domain.Length)}{domain}";
    }

}
