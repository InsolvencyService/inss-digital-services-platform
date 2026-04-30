namespace GovUk.Forms.HostApp.UI.Test.Pages.Login;

public interface IStartPage
{
    Task ClickOnStartNowAsync();
    Task<IPage> ClickOnFeedbackAsync();
    Task<string> GetHeadingTextAsync();
    Task WaitForPageToLoadAsync();
    Task ClickOnFooterLinkAsync(string linkText);
}
