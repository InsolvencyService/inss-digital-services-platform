namespace GovUk.Forms.HostApp.UI.Test.Pages.Declaration;

public interface IDeclarationPage
{
    Task WaitForPageToLoadAsync();
    Task<IPage> ClickOnSection187LinkAsync();
    Task ClickOnBackButtonAsync();
    Task ClickOnAgreeAndContinueButtonAsync();
    Task<string> CapturePageVisualAsync(string name);
}

