using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Config.Environments;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Pages.Login;
using GovUk.Forms.HostApp.UI.Test.Pages.Navigation;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators;

public class NavigationCoordinator(
    IStartPage startPage,
    IPageNotFoundPage pageNotFoundPage,
    IPlaywrightDriver playwrightDriver,
    TestArtifacts testArtifacts) : BaseCoordinator(testArtifacts)
{
    public async Task VerifyOnApplicationAsync()
    {
        await ExecuteStepAsync(
            "Verify IP Upload application is displayed",
            async () =>
            {
                await startPage.WaitForPageToLoadAsync();
                AddAllureLog("Navigation", "IP Upload application is displayed");
            });
    }

    public async Task NavigateToInvalidUrlAsync()
    {
        await ExecuteStepAsync(
            "Navigate to invalid IP Upload URL",
            async () =>
            {
                string baseUrl = EnvironmentConfigFactory.EnvironmentConfig.BaseUrl;
                string invalidUrl = $"{baseUrl.TrimEnd('/')}/this-page-does-not-exist";

                await playwrightDriver.Page.GotoAsync(
                    invalidUrl,
                    new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded });

                AddAllureLog("Navigation", $"Navigated to invalid URL: {invalidUrl}");
            });
    }

    public async Task VerifyPageNotFoundIsDisplayedAsync()
    {
        await ExecuteStepAsync(
            "Verify Page not found page is displayed",
            async () =>
            {
                await pageNotFoundPage.WaitForPageToLoadAsync();
                AddAllureLog("Navigation", "Page not found page displayed successfully");
            });
    }
}
