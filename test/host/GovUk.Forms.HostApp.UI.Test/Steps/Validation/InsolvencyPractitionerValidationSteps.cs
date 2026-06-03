using GovUk.Forms.HostApp.UI.Test.Coordinators;
using GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Steps.Base;

namespace GovUk.Forms.HostApp.UI.Test.Steps.Validation;

[Scope(Feature = "Insolvency Practitioner Validation")]
[Binding]
public class InsolvencyPractitionerValidationSteps : ValidationStepsBase
{
    private readonly CheckYourAnswersCoordinator _checkYourAnswersCoordinator;
    public InsolvencyPractitionerValidationSteps(
        UploadDocumentCoordinator uploadDocumentCoordinator,
        UploadErrorDetailsCoordinator uploadErrorDetailsCoordinator,
        ScenarioContext scenarioContext,
        CheckYourAnswersCoordinator checkYourAnswersCoordinator)
        : base(uploadDocumentCoordinator, uploadErrorDetailsCoordinator, scenarioContext)
    {
        _checkYourAnswersCoordinator = checkYourAnswersCoordinator;
    }

    [Given("the RP14 XML contains an insolvency practitioner name of length {int}")]
    public async Task GivenTheRP14XMLContainsAnInsolvencyPractitionerNameOfLength(int length)
    {
        string ipName = LengthHelper.AtMax(length);
        await UploadDocumentCoordinator.UploadRp14WithIpNameAsync(ipName);
    }

    [Given("the RP14 XML contains an insolvency practitioner registration number of length {int}")]
    public async Task GivenTheRP14XMLContainsAnInsolvencyPractitionerRegistrationNumberOfLength(int length)
    {
        string registrationNumber = new('1', length);
        await UploadDocumentCoordinator.UploadRp14WithIpRegistrationNumberAsync(registrationNumber);
    }

    [Given("the RP14 XML contains an insolvency practitioner firm name of length {int}")]
    public async Task GivenTheRPXMLContainsAnInsolvencyPractitionerFirmNameOfLength(int length)
    {
        string firmName = LengthHelper.AtMax(length);
        await UploadDocumentCoordinator.UploadRp14WithIpFirmNameAsync(firmName);
    }

    [Given("the RP14 XML contains an insolvency practitioner email of length {int}")]
    public async Task GivenTheRp14XmlContainsAnInsolvencyPractitionerEmailOfLength(int length)
    {
        string emailAddress = CreateEmailAddressOfLength(length);
        await UploadDocumentCoordinator.UploadRp14WithIpEmailAddressAsync(emailAddress);
    }

    [Given("the RP14 XML contains an insolvency practitioner phone number of length {int}")]
    public async Task GivenTheRp14XmlContainsAnInsolvencyPractitionerPhoneNumberOfLength(int length)
    {
        string phoneNumber = new('1', length);
        await UploadDocumentCoordinator.UploadRp14WithIpTelephoneNumberAsync(phoneNumber);
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

        return $"{new string('a', length - domain.Length)}{domain}";
    }

}
