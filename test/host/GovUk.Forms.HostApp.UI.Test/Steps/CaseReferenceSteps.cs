using GovUk.Forms.HostApp.UI.Test.Coordinators;
using GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;
using GovUk.Forms.HostApp.UI.Test.Factories;
using GovUk.Forms.HostApp.UI.Test.Models;
using GovUk.Forms.HostApp.UI.Test.Support;

namespace GovUk.Forms.HostApp.UI.Test.Steps;

[Binding]
public class CaseReferenceSteps(
    CaseReferenceCoordinator caseReferenceCoordinator,
    DeclarationCoordinator declarationCoordinator,
    CommonCoordinator commonCoordinator,
    UploadDocumentCoordinator uploadDocumentCoordinator,
    ScenarioContext scenarioContext)
{
    private const string UserTypeKey = "UserType";
    private const string EnteredCaseReferenceKey = "EnteredCaseReference";


    [Given(@"I am on the declaration page as a ""(.*)"" user")]
    public async Task GivenIAmOnTheDeclarationPageAsUser(string userType)
    {
        scenarioContext[UserTypeKey] = userType;
        TestUser user = UserFactory.GetUser(userType);
        await commonCoordinator.VerifyThatDeclarationPageIsDisplayedAsync(user);
    }

    [Given("I am on the case reference number page")]
    public async Task GivenIAmOnTheCaseReferenceNumberPage()
    {
        await declarationCoordinator.ClickAgreeAndContinueButtonAsync();
        await caseReferenceCoordinator.VerifyCaseReferencePageIsDisplayedAsync();
    }

    [Given("I am on the employer details page")]
    public async Task GivenIAmOnTheEmployerDetailsPage()
    {
        await GivenIAmOnTheCaseReferenceNumberPage();
        scenarioContext[EnteredCaseReferenceKey] = ScenarioConstant.ValidCaseReference;
        await caseReferenceCoordinator.EnterCaseReferenceAndContinueAsync(ScenarioConstant.ValidCaseReference);
        await caseReferenceCoordinator.VerifyEmployerDetailsPageIsDisplayedAsync();
    }

    [Given("I have entered a valid case reference")]
    public async Task GivenIHaveEnteredAValidCaseReference()
    {
        await GivenIAmOnTheCaseReferenceNumberPage();
        scenarioContext[EnteredCaseReferenceKey] = ScenarioConstant.ValidCaseReference;
        await caseReferenceCoordinator.EnterCaseReferenceNumberAsync(ScenarioConstant.ValidCaseReference);
    }

    [When("I click Agree and continue")]
    public async Task WhenIClickAgreeAndContinue()
    {
        await declarationCoordinator.ClickAgreeAndContinueButtonAsync();
    }

    [When("I enter a valid case reference number")]
    public async Task WhenIEnterAValidCaseReferenceNumber()
    {
        scenarioContext[EnteredCaseReferenceKey] = ScenarioConstant.ValidCaseReference;
        await caseReferenceCoordinator.EnterCaseReferenceNumberAsync(ScenarioConstant.ValidCaseReference);
    }

    [When(@"I enter ""(.*)"" as the case reference number")]
    public async Task WhenIEnterAsTheCaseReferenceNumber(string caseReference)
    {
        scenarioContext[EnteredCaseReferenceKey] = caseReference;
        await caseReferenceCoordinator.EnterCaseReferenceNumberAsync(caseReference);
    }

    [When("I enter a case reference number that has not been linked to an employer")]
    public async Task WhenIEnterACaseReferenceNotLinkedToAnEmployer()
    {
        await caseReferenceCoordinator.EnterCaseReferenceNumberAsync(ScenarioConstant.UnlinkedCaseReference);
    }

    [When("I proceed to the next page")]
    public async Task WhenIProceedToTheNextPage()
    {
        await caseReferenceCoordinator.ClickContinueAsync();
    }

    [When("I navigate to the employer details page")]
    public async Task WhenINavigateToTheEmployerDetailsPage()
    {
        await caseReferenceCoordinator.ClickContinueAsync();
        await caseReferenceCoordinator.VerifyEmployerDetailsPageIsDisplayedAsync();
    }

    [When("I go to the previous page")]
    public async Task WhenIGoToThePreviousPage()
    {
        await caseReferenceCoordinator.ClickBackAsync();
    }

    [When("I confirm that this is the correct employer name")]
    public async Task WhenIConfirmCorrectEmployerName()
    {
        await caseReferenceCoordinator.SelectYesAsync();
    }

    [When("I confirm that this is not the correct employer name")]
    public async Task WhenIDeclineCorrectEmployerName()
    {
        await caseReferenceCoordinator.SelectNoAsync();
    }

    // ── Then steps ─────────────────────────────────────────────────────────

    [Then("I will be taken to the case reference number page")]
    public async Task ThenIWillBeTakenToTheCaseReferenceNumberPage()
    {
        await caseReferenceCoordinator.VerifyCaseReferencePageIsDisplayedAsync();
    }

    [Then("I will be taken to the Employer Details page")]
    public async Task ThenIWillBeTakenToTheEmployerDetailsPage()
    {
        await caseReferenceCoordinator.VerifyEmployerDetailsPageIsDisplayedAsync();
    }

    [Then("I will be taken to the Upload a file page")]
    public async Task ThenIWillBeTakenToTheUploadAFilePage()
    {
        await uploadDocumentCoordinator.VerifyUploadDocumentPageIsDisplayedAsync();
    }

    [Then(@"I should see the case reference error ""(.*)""")]
    public async Task ThenIShouldSeeTheCaseReferenceError(string errorMessage)
    {
        await caseReferenceCoordinator.VerifyValidationErrorsAsync(errorMessage);
    }

    [Then("I will see the case reference number I entered")]
    public async Task ThenIWillSeeTheCaseReferenceNumberIEntered()
    {
        string expected = scenarioContext.TryGetValue(EnteredCaseReferenceKey, out string? stored)
            ? stored!
            : ScenarioConstant.ValidCaseReference;

        await caseReferenceCoordinator.VerifyCaseReferenceInSummaryAsync(expected);
    }

    [Then("I will see the name of the employer it relates to")]
    public async Task ThenIWillSeeTheNameOfTheEmployerItRelatesTo()
    {
        await caseReferenceCoordinator.VerifyEmployerNameIsDisplayedAsync();
    }

    [Then("I should see the case reference number")]
    public async Task ThenIShouldSeeTheCaseReferenceNumber()
    {
        string expected = scenarioContext.TryGetValue(EnteredCaseReferenceKey, out string? stored)
            ? stored!
            : ScenarioConstant.ValidCaseReference;

        await caseReferenceCoordinator.VerifyCaseReferenceInSummaryAsync(expected);
    }

    [Then("I should see the employer name")]
    public async Task ThenIShouldSeeTheEmployerName()
    {
        await caseReferenceCoordinator.VerifyEmployerNameIsDisplayedAsync();
    }
}
