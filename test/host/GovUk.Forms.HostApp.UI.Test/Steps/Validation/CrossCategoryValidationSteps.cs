using GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Models.TestData;
using GovUk.Forms.HostApp.UI.Test.Steps.Base;
using GovUk.Forms.HostApp.UI.Test.Support;
using System.Globalization;

namespace GovUk.Forms.HostApp.UI.Test.Steps.Validation;

[Scope(Feature = "Cross Category Validation")]
[Binding]
public sealed class CrossCategoryValidationSteps : ValidationStepsBase
{
    private const string ExpectedErrorsKey = "CrossCategoryExpectedErrors";

    public CrossCategoryValidationSteps(
        UploadDocumentCoordinator uploadDocumentCoordinator,
        UploadErrorDetailsCoordinator uploadErrorDetailsCoordinator,
        ScenarioContext scenarioContext)
        : base(uploadDocumentCoordinator, uploadErrorDetailsCoordinator, scenarioContext)
    {
    }

    [Given("the RP14 XML contains the following invalid values and errors")]
    public async Task GivenTheRp14XmlContainsTheFollowingInvalidValuesAndErrors(DataTable dataTable)
    {
        Dictionary<string, string> fieldValues = new(StringComparer.OrdinalIgnoreCase);
        List<Error> expectedErrors = [];

        foreach (DataTableRow row in dataTable.Rows)
        {
            fieldValues[row["Field"]] = row["Value"];

            expectedErrors.Add(new Error(
                Message: row["Message"],
                Hint: row["Hint"],
                Type: row["Type"]));
        }

        await UploadDocumentCoordinator.UploadRp14WithCrossCategoryErrorsAsync(
            caseReference: fieldValues.GetValueOrDefault("CaseReference", string.Empty),
            businessName: NullIfWhiteSpace(fieldValues.GetValueOrDefault("BusinessName", string.Empty)),
            directorNino: fieldValues.GetValueOrDefault("DirectorNationalInsuranceNumber", string.Empty),
            shareholderPercentage: fieldValues.GetValueOrDefault("ShareholderPercentage", string.Empty),
            payRecordsContactName: NullIfWhiteSpace(fieldValues.GetValueOrDefault("PayRecordsContactName", string.Empty)));

        ScenarioContext.Set(expectedErrors, ExpectedErrorsKey);
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
                ErrorMessage: error.Message.Replace(
                    "{validCaseReference}",
                    ScenarioConstant.ValidCaseReference),
                HintText: error.Hint,
                ActionText: null);

            await UploadErrorDetailsCoordinator.VerifyErrorSummaryIsDisplayedAsync(expectedError);
        }
    }

    [Then("I should see the expected validation errors")]
    public async Task ThenIShouldSeeTheExpectedValidationErrors()
    {
        List<Error> expectedErrors = ScenarioContext.Get<List<Error>>(ExpectedErrorsKey);

        foreach (Error error in expectedErrors)
        {
            UploadErrorSummary expectedError = new(
                Category: string.Empty,
                ErrorType: error.Type,
                ErrorMessage: error.Message.Replace(
                    "{validCaseReference}",
                    ScenarioConstant.ValidCaseReference),
                HintText: error.Hint,
                ActionText: null);

            await UploadErrorDetailsCoordinator.VerifyErrorSummaryIsDisplayedAsync(expectedError);
        }
    }

    private static string? NullIfWhiteSpace(string value) =>
        string.IsNullOrWhiteSpace(value) ? null : value;
}
