using GovUk.Forms.HostApp.UI.Test.Config;
using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Extensions;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using System.Reflection;

namespace GovUk.Forms.HostApp.UI.Test.Hook;

[Binding]
public sealed class ReqnrollHook : BaseTestConfig
{
    private const string AddVideoTag = "addVideo";
    private const string StepScreenshotsTag = "stepScreenshots";

    private readonly ScenarioContext _scenarioContext;
    private readonly FeatureContext _featureContext;
    private readonly IReqnrollOutputHelper _output;
    private readonly IPlaywrightDriver _driver;

    private bool _shouldRecordVideo;
    private bool _shouldCaptureStepScreenshots;

    public ReqnrollHook(
        ScenarioContext scenarioContext,
        IReqnrollOutputHelper output,
        IPlaywrightDriver driver,
        FeatureContext featureContext)
    {
        _scenarioContext = scenarioContext ?? throw new ArgumentNullException(nameof(scenarioContext));
        _output = output ?? throw new ArgumentNullException(nameof(output));
        _driver = driver ?? throw new ArgumentNullException(nameof(driver));
        _featureContext = featureContext ?? throw new ArgumentNullException(nameof(featureContext));
    }

    [BeforeScenario(Order = 0)]
    public void ValidateScenario()
    {
        TestValidator.ValidateScenario(_scenarioContext);
        TestArtifactsSetup(_scenarioContext);

        _shouldRecordVideo = HasTag(AddVideoTag);
        _shouldCaptureStepScreenshots = HasTag(StepScreenshotsTag);
    }

    [BeforeScenario(Order = 1)]
    public async Task BeforeScenarioAsync()
    {
        BrowserNewContextOptions contextOptions = BuildContextOptions();

        await _driver.InitialiseAsync(contextOptions);

        await BrowserSetupAsync(_scenarioContext, _driver.Page, _driver.Context);
    }

    [AfterScenario(Order = 0)]
    public async Task AfterScenarioAsync()
    {
        IBrowserContext context = _driver.Context;
        IPage page = _driver.Page;

        IVideo? video = page.Video;
        bool failed = _scenarioContext.ScenarioExecutionStatus == ScenarioExecutionStatus.TestError;

        try
        {
            await BrowserTearDownAsync(_scenarioContext, _output, context, page);

            if (!page.IsClosed)
            {
                await page.CloseAsync();
            }

            if (_shouldRecordVideo && failed && video is not null && TestArtifacts is not null)
            {
                await video.SaveToArtifactsAsync(
                    TestArtifacts,
                    _output,
                    $"{TestName}_failure");
            }
        }
        catch (Exception ex)
        {
            _output.WriteLine($"[AfterScenario Error] {ex}");
            throw;
        }
        finally
        {
            await _driver.CloseAsync();
        }
    }

    [AfterStep]
    public async Task AfterStepAsync()
    {
        if (!_shouldCaptureStepScreenshots || TestArtifacts is null)
        {
            return;
        }

        try
        {
            if (_driver.Page.IsClosed)
            {
                return;
            }

            StepInfo step = _scenarioContext.StepContext.StepInfo;

            string stepType = step.StepDefinitionType.ToString();
            string stepText = SanitizeFileName(step.Text);
            string screenshotName = $"{stepType}_{stepText}_{DateTime.UtcNow:HHmmssfff}";
            string path = TestArtifacts.GetScreenshotPath(screenshotName);

            await _driver.Page.TakeScreenshotAsync(_output, path);
            _output.AddAttachmentAsLink(path);
        }
        catch (Exception ex)
        {
            _output.WriteLine($"[AfterStep Screenshot Error] {ex.Message}");
        }
    }

    [BeforeTestRun]
    public static void EnforceArchitecture()
    {
        TestValidator.VerifyStepDefinitionsUseOnlyCoordinators(
            Assembly.GetExecutingAssembly());
    }

    private BrowserNewContextOptions BuildContextOptions()
    {
        BrowserNewContextOptions options = new() { IgnoreHTTPSErrors = true };

        if (_shouldRecordVideo && TestArtifacts is not null)
        {
            options.RecordVideoDir = TestArtifacts.VideoDirectory;
            options.RecordVideoSize = new RecordVideoSize
            {
                Width = 1280,
                Height = 720
            };
        }

        return options;
    }

    private bool HasTag(string tag)
    {
        IEnumerable<string> tags =
            _scenarioContext.ScenarioInfo.Tags
                .Concat(_featureContext.FeatureInfo.Tags ?? []);

        return tags.Any(t => string.Equals(t, tag, StringComparison.OrdinalIgnoreCase));
    }
}