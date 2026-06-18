using GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Models.TestData;
using GovUk.Forms.HostApp.UI.Test.Steps.Base;
using System.Globalization;

namespace GovUk.Forms.HostApp.UI.Test.Steps.Validation;

[Scope(Feature = "Cross Category Validation")]
[Binding]
public sealed class CrossCategoryValidationSteps : ValidationStepsBase
{
    public CrossCategoryValidationSteps(
        UploadDocumentCoordinator uploadDocumentCoordinator,
        UploadErrorDetailsCoordinator uploadErrorDetailsCoordinator,
        ScenarioContext scenarioContext)
        : base(uploadDocumentCoordinator, uploadErrorDetailsCoordinator, scenarioContext)
    {
    }

    [Given("the RP14 XML contains the following invalid values")]
    public async Task GivenTheRp14XmlContainsTheFollowingInvalidValues(DataTable dataTable)
    {
        if (dataTable.Header.Contains("Type"))
        {
            Dictionary<string, int> counts = dataTable.Rows
                .ToDictionary(r => r["Type"], r => int.Parse(r["Count"], CultureInfo.InvariantCulture));

            await UploadDocumentCoordinator.UploadRp14WithRepeatedValidationErrorsAsync(
                directorNinoCount: counts.GetValueOrDefault("DirectorNationalInsuranceNumber"),
                shareholderPercentageCount: counts.GetValueOrDefault("ShareholderPercentage"),
                addressLineCount: counts.GetValueOrDefault("AddressLine"),
                businessNameCount: counts.GetValueOrDefault("BusinessName"));
        }
        else
        {
            DataTableRow row = dataTable.Rows[0];

            await UploadDocumentCoordinator.UploadRp14WithCrossCategoryErrorsAsync(
                caseReference: row["caseReference"],
                businessName: NullIfWhiteSpace(row["businessName"]),
                directorNino: row["directorNationalInsuranceNumber"],
                shareholderPercentage: row["shareholderPercentage"],
                payRecordsContactName: NullIfWhiteSpace(row["payRecordsContactName"]));
        }
    }

    [Then("I should see the following validation errors")]
    public async Task ThenIShouldSeeTheFollowingValidationErrors(DataTable dataTable)
    {
        foreach (Error error in dataTable.CreateSet<Error>())
        {
            UploadErrorSummary expectedError = new(
                Category: string.Empty,
                ErrorType: error.Type,
                ErrorMessage: error.Message,
                HintText: error.Hint,
                ActionText: null);

            await UploadErrorDetailsCoordinator.VerifyErrorSummaryIsDisplayedAsync(expectedError);
        }
    }

    private static string? NullIfWhiteSpace(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : value;
}
