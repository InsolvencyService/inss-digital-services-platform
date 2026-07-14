using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Pages.Common;
using GovUk.Forms.HostApp.UI.Test.Pages.Login;
using GovUk.Forms.HostApp.UI.Test.Support;

namespace GovUk.Forms.HostApp.UI.Test.Pages.Declaration;

public class DeclarationPage : BasePage, IDeclarationPage
{
    private readonly IPlaywrightDriver _playwrightDriver;
    private readonly ICommonPage _commonPage;

    public DeclarationPage(IPlaywrightDriver playwrightDriver, ICommonPage commonPage)
    {
        _playwrightDriver = playwrightDriver;
        _commonPage = commonPage;
    }

    protected new IPage Page => _playwrightDriver.Page;

    private ILocator DeclarationTitle => Page.GetByRole(AriaRole.Link, new() { Name = DeclarationLocators.Labels.DeclarationTitle });
    private ILocator Section187Link => Page.GetByRole(AriaRole.Link, new() { Name = DeclarationLocators.Labels.Section187Link });
    private ILocator BackButton => Page.GetByRole(AriaRole.Link, new() { Name = SharedLocactors.BackButton, Exact = true });
    private ILocator AgreeAndContinueButton => Page.GetByRole(AriaRole.Button, new() { Name = DeclarationLocators.Labels.AgreeAndContinueButton });
    protected override async Task PageContentLoadedAsync()
    {
        await Page.WaitForLoadStateAsync(LoadState.Load);
        await Expect(DeclarationTitle).ToBeVisibleAsync();
        await Expect(DeclarationTitle).ToHaveTextAsync(DeclarationLocators.Labels.DeclarationTitle);
        await Expect(Section187Link).ToBeVisibleAsync();
        await Expect(Section187Link).ToBeEnabledAsync();
        await Expect(AgreeAndContinueButton).ToBeVisibleAsync();
    }

    public async Task<IPage> ClickOnSection187LinkAsync()
    {
        await PageContentLoadedAsync();

        IPage page = await _commonPage.OpenNewTabAndVerifyAsync(
            Page.Context,
            () => Section187Link.ClickAsync(),
            expectedUrlPart: PartialPageUris.Section187Page);
        return page;
    }

    public async Task ClickOnBackButtonAsync()
    {
        await PageContentLoadedAsync();
        await BackButton.ClickAsync();
    }

    public async Task ClickOnAgreeAndContinueButtonAsync()
    {
        await PageContentLoadedAsync();
        await AgreeAndContinueButton.ClickAsync();
    }

    public async Task VerifyAriaSnapshotAsync()
    {
        await WaitForPageToLoadAsync();

        await Expect(Page.Locator(StartPageLocators.Selectors.MainContent))
            .ToMatchAriaSnapshotAsync("""
              - heading "Declaration" [level=1]
              - paragraph: This information is required under Section 187(1) of the Employment Rights Act 1996.
              - paragraph:
                - link "Section 187(1) (opens in new tab)":
                  - /url: https://www.legislation.gov.uk/ukpga/1996/18/section/187
              - paragraph: "I am the relevant officer* who has been appointed in connection with the employer’s insolvency, and I declare to the best of my knowledge and belief:"
              - list:
                - listitem: That in completing the required forms and submitting this statement, I am listing the amount of that debt which appears to have been owed to an employee on the appropriate date, and which appears to remain unpaid. In particular, I have checked whether the claimants were on the payroll and appear entitled to make a claim.
              - paragraph: "I also confirm that:"
              - list:
                - listitem: Should I become aware of any changes to the information already submitted, I will immediately inform the Redundancy Payments Service in writing on a new RP14/RP14A.
                - listitem: "Should I come to believe that there is relevant information, not provided for within the RP14/RP14A that may impact the claims, I will immediately inform the Redundancy Payments Service in writing to: RPS.Stakeholder@insolvency.gov.uk"
                - listitem: I am aware that the RPS reserves the right to conduct an assessment of compliance and, in the process, may request relevant records or other documents for review under Section 190(1)(b) of the Employment Rights Act 1996.
              - paragraph: "*Administrator, Liquidator, Trustee, Interim Trustee, Receiver or Manager"
              - button "Agree and continue"
              """);
    }
}
