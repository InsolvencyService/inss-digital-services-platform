namespace GovUk.Forms.HostApp.UI.Tests.Pages;

public abstract class BasePage
{

    public virtual async Task WaitForPageToLoadAsync()
    {
        await PageContentLoadedAsync();
    }
    protected abstract Task PageContentLoadedAsync();
}
