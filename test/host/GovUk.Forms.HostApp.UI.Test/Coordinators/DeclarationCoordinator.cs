using GovUk.Forms.HostApp.UI.Test.Pages.Declaration;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators;

public class DeclarationCoordinator(
    IDeclarationPage declarationPage,
    ISection187Page section187Page)
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
        await section187Page.VerifySection187PageContentAsync();
    }
}
