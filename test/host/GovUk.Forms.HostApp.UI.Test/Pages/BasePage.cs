namespace GovUk.Forms.HostApp.UI.Test.Pages;

public abstract class BasePage
{
    protected IPage Page { get; private set; } = null!;

    public void AttachTo(IPage page)
    {
        Page = page;
    }

    public virtual async Task WaitForPageToLoadAsync()
    {
        await PageContentLoadedAsync();
    }
    protected abstract Task PageContentLoadedAsync();
}
