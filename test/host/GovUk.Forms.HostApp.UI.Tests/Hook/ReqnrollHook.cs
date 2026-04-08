using GovUk.Forms.HostApp.UI.Tests.Config;
using GovUk.Forms.HostApp.UI.Tests.Helpers;
using Reqnroll;
namespace GovUk.Forms.HostApp.UI.Tests.Hook;

[Binding]
public sealed class ReqnrollHook : BaseTestConfig
{
    private readonly ScenarioContext _scenarioContext;
    private readonly IReqnrollOutputHelper _reqnrollOutputHelper;


    public ReqnrollHook(ScenarioContext scenarioContext,
        IReqnrollOutputHelper reqnrollOutputHelper)
    {
        _scenarioContext = scenarioContext;
        _reqnrollOutputHelper = reqnrollOutputHelper;
    }

    [BeforeScenario(Order = 0)]
    public async Task ValidateScenario()
    {
        TestValidator.ValidateScenario(_scenarioContext);
        await BrowserSetupAsync(_scenarioContext);
        _scenarioContext.ScenarioContainer.RegisterInstanceAs(Page);
        _scenarioContext.ScenarioContainer.RegisterInstanceAs(Context);

    }

    [AfterScenario(Order = 0)]
    public async Task AfterScenario()
    {
        await BrowserTearDownAsync(_scenarioContext, _reqnrollOutputHelper);
    }

}