using GovUk.Forms.HostApp.UI.Tests.Config;
using GovUk.Forms.HostApp.UI.Tests.Config.Driver;
using GovUk.Forms.HostApp.UI.Tests.Helpers;
using System.Reflection;
namespace GovUk.Forms.HostApp.UI.Tests.Hook;

[Binding]
public sealed class ReqnrollHook : BaseTestConfig
{
    private readonly ScenarioContext _scenarioContext;
    private readonly IReqnrollOutputHelper _reqnrollOutputHelper;
    private readonly IPlaywrightDriver _playwrightDriver;


    public ReqnrollHook(ScenarioContext scenarioContext,
        IReqnrollOutputHelper reqnrollOutputHelper,
        IPlaywrightDriver playwrightDriver)
    {
        _scenarioContext = scenarioContext;
        _reqnrollOutputHelper = reqnrollOutputHelper;
        _playwrightDriver = playwrightDriver;
    }

    [BeforeScenario(Order = 0)]
    public async Task ValidateScenario()
    {
        TestValidator.ValidateScenario(_scenarioContext);
        await _playwrightDriver.InitialiseAsync();
        await BrowserSetupAsync(_scenarioContext, _playwrightDriver.Page);
        _scenarioContext.ScenarioContainer.RegisterInstanceAs(_playwrightDriver.Page);
        _scenarioContext.ScenarioContainer.RegisterInstanceAs(_playwrightDriver.Context);

    }

    [AfterScenario(Order = 0)]
    public async Task AfterScenario()
    {
        IBrowserContext context = _scenarioContext.ScenarioContainer.Resolve<IBrowserContext>();
        IPage page = _scenarioContext.ScenarioContainer.Resolve<IPage>();

        await BrowserTearDownAsync(_scenarioContext, _reqnrollOutputHelper, context, page);
        await _playwrightDriver.Browser.CloseAsync();
    }



    [BeforeTestRun]
    public static void EnforceArchitecture()
    {
        TestValidator.VerifyStepDefinitionsUseOnlyCoordinators(
            Assembly.GetExecutingAssembly());
    }



}