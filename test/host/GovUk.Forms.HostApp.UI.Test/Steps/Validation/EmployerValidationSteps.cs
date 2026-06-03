using GovUk.Forms.HostApp.UI.Test.Coordinators;
using GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Models.TestData;
using GovUk.Forms.HostApp.UI.Test.Steps.Base;
using GovUk.Forms.HostApp.UI.Test.Support;
using GovUk.Forms.HostApp.UI.Test.Tags;

namespace GovUk.Forms.HostApp.UI.Test.Steps.Validation;

[Scope(Feature = "Employer Validation")]
[Binding]
public class EmployerValidationSteps : ValidationStepsBase
{
    private const string EmployerNames = "EmployerNames";
    private readonly CheckYourAnswersCoordinator _uploadDocumentSummaryCoordinator;

    public EmployerValidationSteps(
        UploadDocumentCoordinator uploadDocumentCoordinator,
        UploadErrorDetailsCoordinator uploadErrorDetailsCoordinator,
        ScenarioContext scenarioContext,
        CheckYourAnswersCoordinator uploadDocumentSummaryCoordinator)
        : base(uploadDocumentCoordinator, uploadErrorDetailsCoordinator, scenarioContext)
    {
        _uploadDocumentSummaryCoordinator = uploadDocumentSummaryCoordinator;
    }

    [Given("I have uploaded an RP14A file with employer name of length {int}")]
    public async Task GivenIHaveUploadedAnRP14AFileWithEmployerNameOfLength(int length)
    {
        string employerName = LengthHelper.AtMax(length);

        await UploadDocumentCoordinator.UploadRp14aWithEmployerNameAsync(employerName);

        ScenarioContext.Set(employerName, "EmployerName");
    }

    [Given("the RP14A contains multiple validation issues")]
    public async Task GivenTheRp14aContainsMultipleValidationIssues()
    {
        const string invalidBasicPayPerWeek = "12.345";
        const string invalidHolidayOwed = "-1";

        ScenarioContext.Set(
            invalidBasicPayPerWeek,
            InvalidBasicPayPerWeekKey);

        ScenarioContext.Set(
            invalidHolidayOwed,
            InvalidHolidayOwedKey);

        await UploadDocumentCoordinator.UploadComplexRp14aScenarioAsync(
            caseReference: string.Empty,
            employerName: LengthHelper.AtMax(100),
            surname: string.Empty,
            forename: ScenarioConstant.Forename,
            title: ScenarioConstant.Title,
            basicPayPerWeek: invalidBasicPayPerWeek,
            holidayOwed: invalidHolidayOwed);
    }

    [Given(@"I have uploaded an RP14A file with (.*) employer names of length (.*)")]
    public async Task GivenIHaveUploadedAnRp14AFileWithEmployerNamesOfLength(int count, int length)
    {
        string[] employerNames = Enumerable
            .Range(1, count)
            .Select(_ => LengthHelper.AtMax(length))
            .ToArray();

        await UploadDocumentCoordinator.UploadRp14aWithEmployerNameAsync(employerNames);

        ScenarioContext.Set(employerNames, EmployerNames);
    }

    [Given("the RP14A contains an employee with:")]
    public async Task GivenTheRP14AContainsAnEmployeeWith(DataTable dataTable)
    {
        EmployeeValidationData employee =
            dataTable.CreateInstance<EmployeeValidationData>();

        await UploadDocumentCoordinator.UploadComplexRp14aScenarioAsync(
           surname: employee.Surname,
           forename: ScenarioConstant.Forename,
           title: ScenarioConstant.Title,
           nationalInsuranceNumber: employee.NationalInsuranceNumber,
           moneyOwedToEmployer: employee.MoneyOwedToEmployer,
           basicPayPerWeek: employee.BasicPayPerWeek);
    }

    [When("I submit the RP14A file")]
    public async Task WhenISubmitTheRPAFile()
    {
        await UploadDocumentCoordinator.NavigateToSubmitPageAsync();
    }

    [Then(@"the submission should be {string}")]
    public async Task ThenSubmissionShouldBe(string outcome)
    {
        ScenarioContext.Set(outcome, ScenarioConstant.SubmissionOutcome);

        switch (outcome.ToLowerInvariant())
        {
            case "accepted":
                await _uploadDocumentSummaryCoordinator
                    .VerifyCheckYourAnswersPageIsDisplayedAsync();
                return;

            case "rejected":
                await UploadErrorDetailsCoordinator
                    .VerifyUploadErrorPageIsDisplayedAsync();
                return;

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(outcome),
                    outcome,
                    "Outcome must be 'accepted' or 'rejected'.");
        }
    }

    [Then("the error summary should {string} with {string}")]
    public async Task ThenTheErrorSummaryShouldWith(string summaryBehaviour, string detailsBehaviour)
    {
        string outcome =
            ScenarioContext.Get<string>(ScenarioConstant.SubmissionOutcome);

        if (outcome.Equals("accepted", StringComparison.OrdinalIgnoreCase))
        {
            await _uploadDocumentSummaryCoordinator.VerifyCheckYourAnswersPageIsDisplayedAsync();
            return;
        }

        UploadErrorSummary expectedError = new(
            Category: "Employer",
            ErrorType: "Employer name",
            ErrorMessage: summaryBehaviour,
            HintText: detailsBehaviour);

        ScenarioContext.Set(expectedError);
        ScenarioContext.Set(summaryBehaviour, ScenarioConstant.ErrorMessage);

        await UploadErrorDetailsCoordinator
            .VerifyErrorSummaryIsDisplayedAsync(expectedError);
    }

    [Then("I should be able to view error details")]
    public async Task ThenIShouldBeAbleToViewErrorDetails()
    {
        string outcome = ScenarioContext.Get<string>(ScenarioConstant.SubmissionOutcome);

        if (outcome.Equals("accepted", StringComparison.OrdinalIgnoreCase))
        {
            await _uploadDocumentSummaryCoordinator.VerifyCheckYourAnswersPageIsDisplayedAsync();
            return;
        }

        UploadErrorSummary expectedError =
            ScenarioContext.Get<UploadErrorSummary>();

        string employerName =
            ScenarioContext.Get<string>("EmployerName");

        AffectedEmployee affectedEmployee = new()
        {
            Forename = ScenarioConstant.Forename,
            Surname = ScenarioConstant.Surname,
            DateOfBirth = TestFactory.UiDateOfBirth(),
            NiNumber = ScenarioConstant.NationalInsuranceNumber,
            CellValue = employerName
        };

        await UploadErrorDetailsCoordinator
            .VerifyErrorDetailsAsync(
              expectedError,
              affectedEmployee,
             ErrorDetailsHeaderType.EmployerName);
    }


    [Then("I should see the following validation categories")]
    public async Task ThenIShouldSeeTheFollowingValidationCategories(
     DataTable dataTable)
    {
        List<string> expectedCategories = dataTable
            .Rows
            .Select(row => row["Category"])
            .ToList();

        foreach (string category in expectedCategories)
        {
            await UploadErrorDetailsCoordinator
                .VerifyValidationCategoryIsDisplayedAsync(category);
        }
    }

    [Then("I should see the following multiple validation errors")]
    public async Task ThenIShouldSeeTheFollowingMultipleValidationErrors(
     DataTable dataTable)
    {
        List<UploadErrorSummary> expectedErrors = dataTable
            .CreateSet<Error>()
            .Select(error => CreateErrorSummary(
                error.Type,
                error.Message,
                error.Hint))
            .ToList();

        foreach (UploadErrorSummary expectedError in expectedErrors)
        {
            await UploadErrorDetailsCoordinator
                .VerifyErrorSummaryIsDisplayedAsync(expectedError);
        }

        ScenarioContext.Set(expectedErrors, UploadErrorsContextKey);
    }

    [Then("I should be able to view error details for all validation categories")]
    public async Task ThenIShouldBeAbleToViewErrorDetailsForAllValidationCategories()
    {
        List<UploadErrorSummary> expectedErrors =
            ScenarioContext.Get<List<UploadErrorSummary>>(UploadErrorsContextKey);

        Dictionary<string, List<AffectedEmployee>> affectedEmployeesByError =
            ScenarioContext.Get<Dictionary<string, List<AffectedEmployee>>>(
                AffectedEmployeesByErrorTypeKey);

        foreach (UploadErrorSummary expectedError in expectedErrors)
        {
            if (!TryMapToHeaderType(expectedError, out ErrorDetailsHeaderType headerType))
            {
                continue;
            }

            string key = ErrorKey(expectedError);

            if (affectedEmployeesByError.TryGetValue(
                    key,
                    out List<AffectedEmployee>? affectedEmployees))
            {
                await UploadErrorDetailsCoordinator.VerifyErrorDetailsAsync(
                    expectedError,
                    affectedEmployees,
                    headerType);
            }
            else
            {
                await UploadErrorDetailsCoordinator.VerifyErrorDetailsHeaderOnlyAsync(
                    expectedError,
                    headerType);
            }

            await UploadErrorDetailsCoordinator
                .ClickBackAndVerifyUploadErrorPageIsDisplayedAsync();
        }
    }

    [Then("I should be able to view error details for multiple employees")]
    public async Task ThenIShouldBeAbleToViewErrorDetailsForMultipleEmployees()
    {
        UploadErrorSummary expectedError = ScenarioContext.Get<UploadErrorSummary>();
        List<AffectedEmployee> affectedEmployees =
            ScenarioContext.Get<List<AffectedEmployee>>(AffectedEmployeesKey);

        await UploadErrorDetailsCoordinator.VerifyErrorDetailsAsync(expectedError, affectedEmployees, ErrorDetailsHeaderType.EmployerName);
    }


    private static bool TryMapToHeaderType(UploadErrorSummary error, out ErrorDetailsHeaderType headerType)
    {
        switch (error.ErrorType.Trim())
        {
            case "Case reference":
                headerType = ErrorDetailsHeaderType.CaseReference;
                return true;

            case "Employer name":
                headerType = ErrorDetailsHeaderType.EmployerName;
                return true;

            case "Employee surname":
                headerType = ErrorDetailsHeaderType.EmployeeSurname;
                return true;

            case "Employee national insurance number":
                headerType = ErrorDetailsHeaderType.NationalInsuranceNumber;
                return true;

            case "Employee arrears of payment owed":
                headerType = ErrorDetailsHeaderType.ArrearsOfPayOwed;
                return true;

            case "Money owed to employer":
                headerType = ErrorDetailsHeaderType.MoneyOwedToEmployer;
                return true;

            case "Employee employment dates":
                headerType = ErrorDetailsHeaderType.EmploymentDates;
                return true;

            case "Employee arrears of payment dates":
                headerType = ErrorDetailsHeaderType.ArrearsOfPayDates;
                return true;

            case "Employee basic pay per week":
                headerType = ErrorDetailsHeaderType.BasicPayPerWeek;
                return true;

            case "Contracted holiday entitlement":
                headerType = ErrorDetailsHeaderType.HolidayContractedEntitlementDays;
                return true;

            case "Holiday carried forward":
                headerType = ErrorDetailsHeaderType.HolidayDaysCarriedForward;
                return true;

            case "Holiday days taken":
                headerType = ErrorDetailsHeaderType.HolidayDaysTaken;
                return true;

            case "Holiday owed":
                headerType = ErrorDetailsHeaderType.NoDaysHolidayOwed;
                return true;

            default:
                headerType = default;
                return false;
        }
    }
    private static string ErrorKey(UploadErrorSummary error)
    {
        ArgumentNullException.ThrowIfNull(error);

        return $"{error.ErrorType}|{error.ErrorMessage}";
    }

}
