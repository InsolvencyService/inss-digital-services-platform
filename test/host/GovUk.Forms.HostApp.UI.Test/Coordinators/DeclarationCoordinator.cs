using GovUk.Forms.HostApp.UI.Test.Pages.Declaration;
using GovUk.Forms.HostApp.UI.Test.Support;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators;

public class DeclarationCoordinator(
    IDeclarationPage declarationPage,
    ISection187Page section187Page) : BaseCoordinator
{

    public async Task VerifyDeclarationPageIsDisplayedAsync()
    {
        await declarationPage.WaitForPageToLoadAsync();
    }

    public async Task OpenSection187Async()
    {
        IPage page = await declarationPage.ClickOnSection187LinkAsync();
        section187Page.AttachTo(page);
    }

    public async Task VerifySection187PageContentAsync()
    {
        await section187Page.VerifyThatSection187PageIsDisplayedAsync();
    }
    public async Task ReturnToStartPageAsync()
    {
        await declarationPage.ClickOnBackButtonAsync();
    }
    public async Task ClickAgreeAndContinueButtonAsync()
    {
        await declarationPage.ClickOnAgreeAndContinueButtonAsync();
    }
    public async Task<string> CaptureDeclarationPageVisualAsync()
    {
        return await CapturePageVisualAsync(declarationPage.CapturePageVisualAsync, ScenarioConstant.DeclarationPage);
    }
}
