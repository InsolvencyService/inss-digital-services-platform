using GovUk.Forms.HostApp.UI.Tests.Config;
using GovUk.Forms.HostApp.UI.Tests.Config.Driver;
using GovUk.Forms.HostApp.UI.Tests.Extensions;
using GovUk.Forms.HostApp.UI.Tests.Helpers;
using System.Reflection;

namespace GovUk.Forms.HostApp.UI.Tests.Hook;

[Binding]
public sealed class ReqnrollHook : BaseTestConfig
{
    private readonly ScenarioContext _scenarioContext;
    private readonly IReqnrollOutputHelper _output;
    private readonly IPlaywrightDriver _driver;
    private readonly TestArtifacts _testArtifacts;

    public ReqnrollHook(
        ScenarioContext scenarioContext,
        IReqnrollOutputHelper output,
        IPlaywrightDriver driver,
        TestArtifacts testArtifacts)
    {
        _scenarioContext = scenarioContext;
        _output = output;
        _driver = driver;
        _testArtifacts = testArtifacts;
    }

    [BeforeScenario(Order = 0)]
    public void ValidateScenario()
    {
        TestValidator.ValidateScenario(_scenarioContext);

        TestName = SanitizeFileName(_scenarioContext.ScenarioInfo.Title);
        TestArtifacts = _testArtifacts;
        TestOutputDir = _testArtifacts.Folder;
    }

    [BeforeScenario(Order = 1)]
    public async Task BeforeScenario()
    {
        await _driver.InitialiseAsync();
        await BrowserSetupAsync(_scenarioContext, _driver.Page);

        _scenarioContext.ScenarioContainer.RegisterInstanceAs(_driver.Page);
        _scenarioContext.ScenarioContainer.RegisterInstanceAs(_driver.Context);
    }

    [AfterScenario(Order = 0)]
    public async Task AfterScenario()
    {
        IBrowserContext context = _scenarioContext.ScenarioContainer.Resolve<IBrowserContext>();
        IPage page = _scenarioContext.ScenarioContainer.Resolve<IPage>();

        await BrowserTearDownAsync(_scenarioContext, _output, context, page);
        await page.CloseAsync();
        await context.CloseAsync();
        if (_scenarioContext.ScenarioExecutionStatus == ScenarioExecutionStatus.TestError)
        {
            await SaveVideoAsync(_output, page);
        }
        await _driver.Browser.CloseAsync();
    }

    [AfterStep]
    public async Task AfterStepAsync(ScenarioContext scenarioContext)
    {
        try
        {
            StepInfo step = scenarioContext.StepContext.StepInfo;

            string stepType = step.StepDefinitionType.ToString();
            string stepText = Sanitize(step.Text);

            string fileName = $"{stepType}_{stepText}_{DateTime.UtcNow:HHmmssfff}";

            string path = await _driver.TakeScreenshotAsync(fileName);
            _output.AddAttachmentAsLink(path);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AfterStep Screenshot Error] {ex.Message}");
        }
    }

    [BeforeTestRun]
    public static void EnforceArchitecture()
    {
        TestValidator.VerifyStepDefinitionsUseOnlyCoordinators(
            Assembly.GetExecutingAssembly());
    }
}