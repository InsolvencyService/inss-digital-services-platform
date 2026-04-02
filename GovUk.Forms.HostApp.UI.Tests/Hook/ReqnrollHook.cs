using GovUk.Forms.HostApp.UI.Tests.Helpers;
using Reqnroll;

namespace GovUk.Forms.HostApp.UI.Tests.Hook;

[Binding]
public sealed class ReqnrollHook
{
    private readonly ScenarioContext _scenarioContext;

    public ReqnrollHook(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [BeforeScenario(Order = 0)]
    public void ValidateScenario()
    {
        TestValidator.ValidateScenario(_scenarioContext);
    }

}