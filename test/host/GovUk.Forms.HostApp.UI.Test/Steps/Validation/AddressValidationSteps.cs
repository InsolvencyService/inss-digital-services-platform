using GovUk.Forms.HostApp.UI.Test.Coordinators;
using GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Steps.Base;
using GovUk.Forms.HostApp.UI.Test.Support;

namespace GovUk.Forms.HostApp.UI.Test.Steps.Validation;

[Scope(Feature = "AddressValidation")]
[Binding]
public class AddressValidationSteps : ValidationStepsBase
{
    private readonly CheckYourAnswersCoordinator _checkYourAnswersCoordinator;
    public AddressValidationSteps(
        UploadDocumentCoordinator uploadDocumentCoordinator,
        UploadErrorDetailsCoordinator uploadErrorDetailsCoordinator,
        ScenarioContext scenarioContext,
        CheckYourAnswersCoordinator checkYourAnswersCoordinator)
        : base(uploadDocumentCoordinator, uploadErrorDetailsCoordinator, scenarioContext)
    {
        _checkYourAnswersCoordinator = checkYourAnswersCoordinator;
    }

    [Given("the RP14 XML contains an address line of length {int}")]
    public async Task GivenTheRp14XmlContainsAnAddressLineOfLength(int length)
    {
        string addressLine = new('A', length);

        await UploadDocumentCoordinator.UploadRp14WithCompanyAddressLine1Async(addressLine);
    }

    [Given("the RP14 XML contains an address town of length {int}")]
    public async Task GivenTheRp14XmlContainsAnAddressTownOfLength(int length)
    {
        await UploadDocumentCoordinator.UploadRp14WithCompanyAddressFieldAsync(Rp14AddressField.Town, LengthHelper.AtMax(length));
    }

    [Given("the RP14 XML contains an address county of length {int}")]
    public async Task GivenTheRp14XmlContainsAnAddressCountyOfLength(int length)
    {
        await UploadDocumentCoordinator.UploadRp14WithCompanyAddressFieldAsync(Rp14AddressField.County, LengthHelper.AtMax(length));
    }

    [Given("the RP14 XML contains an address postcode of length {int}")]
    public async Task GivenTheRp14XmlContainsAnAddressPostcodeOfLength(int length)
    {
        await UploadDocumentCoordinator.UploadRp14WithCompanyAddressFieldAsync(Rp14AddressField.Postcode, LengthHelper.AtMax(length));
    }

    [Given("the RP14 XML contains {int} address lines")]
    public async Task GivenTheRp14XmlContainsAddressLines(int lineCount)
    {
        await UploadDocumentCoordinator.UploadRp14WithCompanyAddressLinesCountAsync(lineCount);
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
