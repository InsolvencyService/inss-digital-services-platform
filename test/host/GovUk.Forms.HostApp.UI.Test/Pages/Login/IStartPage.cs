namespace GovUk.Forms.HostApp.UI.Test.Pages.Login;

public interface IStartPage
{
    Task ClickOnStartNowAsync();
    Task<IPage> ClickOnFeedbackAsync();
    Task<string> GetHeadingTextAsync();
    Task WaitForPageToLoadAsync();
    Task<string> CaptureStartPageVisualAsync(string name);
    Task ClickOnFooterLinkAsync(string linkText);
}
