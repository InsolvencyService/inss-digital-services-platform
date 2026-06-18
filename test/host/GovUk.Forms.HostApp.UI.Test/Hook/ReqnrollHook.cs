using Allure.Net.Commons;
using GovUk.Forms.HostApp.UI.Test.Config;
using GovUk.Forms.HostApp.UI.Test.Config.Driver;
using GovUk.Forms.HostApp.UI.Test.Config.Environments;
using GovUk.Forms.HostApp.UI.Test.Extensions;
using GovUk.Forms.HostApp.UI.Test.Helpers;
using GovUk.Forms.HostApp.UI.Test.Pages.Common;
using System.Reflection;

namespace GovUk.Forms.HostApp.UI.Test.Hook;

[Binding]
public sealed class ReqnrollHook : BaseTestConfig
{
    private const string AddVideoTag = "addVideo";
    private const string AddScreencastTag = "addScreencast";
    private const string StepScreenshotsTag = "stepScreenshots";

    private readonly ScenarioContext _scenarioContext;
    private readonly FeatureContext _featureContext;
    private readonly IReqnrollOutputHelper _output;
    private readonly IPlaywrightDriver _driver;
    private readonly IAllureReportingHelper _allureReportingHelper;
    private readonly ICommonPage _commonPage;

    private bool _shouldRecordVideo;
    private bool _shouldRecordScreencast;
    private bool _shouldCaptureStepScreenshots;

    public ReqnrollHook(
        ScenarioContext scenarioContext,
        IReqnrollOutputHelper output,
        IPlaywrightDriver driver,
        FeatureContext featureContext,
        IAllureReportingHelper allureReportingHelper,
        ICommonPage commonPage)
    {
        _scenarioContext = scenarioContext ?? throw new ArgumentNullException(nameof(scenarioContext));
        _output = output ?? throw new ArgumentNullException(nameof(output));
        _driver = driver ?? throw new ArgumentNullException(nameof(driver));
        _featureContext = featureContext ?? throw new ArgumentNullException(nameof(featureContext));
        _allureReportingHelper = allureReportingHelper ?? throw new ArgumentNullException(nameof(allureReportingHelper));
        _commonPage = commonPage ?? throw new ArgumentNullException(nameof(commonPage));
    }

    [BeforeScenario(Order = 0)]
    public void ValidateScenario()
    {
        TestValidator.ValidateScenario(_scenarioContext);
        TestArtifactsSetup(_scenarioContext);

        _shouldRecordVideo = HasTag(AddVideoTag);
        _shouldRecordScreencast = HasTag(AddScreencastTag);
        _shouldCaptureStepScreenshots = HasTag(StepScreenshotsTag);

        AllureReportSetup();

        if (_shouldRecordVideo && _shouldRecordScreencast)
        {
            throw new InvalidOperationException(
                "Use either @addVideo or @addScreencast, not both in the same scenario.");
        }
    }

    [BeforeScenario(Order = 2)]
    public async Task BeforeScenarioAsync()
    {
        BrowserNewContextOptions contextOptions = BuildContextOptions();

        await _driver.InitialiseAsync(contextOptions);

        await BrowserSetupAsync(
            _scenarioContext,
            _driver.Page,
            _driver.Context);

        if (_shouldRecordScreencast && TestArtifacts is not null)
        {
            string screencastPath = TestArtifacts.FilePath($"{TestName}_screencast.webm");

            ScreencastHelper screencast = new(_driver.Page, screencastPath);
            _scenarioContext.SetScreencast(screencast);

            await screencast.StartAsync();
        }
    }

    [AfterStep]
    public async Task AfterStepAsync()
    {
        bool failed = _scenarioContext.TestError is not null;

        if (!_shouldCaptureStepScreenshots && !failed)
        {
            return;
        }

        if (TestArtifacts is null || _driver.Page.IsClosed)
        {
            return;
        }

        try
        {
            StepInfo step = _scenarioContext.StepContext.StepInfo;

            string stepType = step.StepDefinitionType.ToString();
            string stepText = SanitizeFileName(step.Text);
            string screenshotName = $"{stepType}_{stepText}_{DateTime.UtcNow:HHmmssfff}";
            string path = TestArtifacts.GetScreenshotPath(screenshotName);

            await _driver.Page.TakeScreenshotAsync(_output, path);

            _output.AddAttachmentAsLink(path);

            _allureReportingHelper.AttachFile(
                failed ? "Failure Step Screenshot" : screenshotName,
                path,
                "image/png");
        }
        catch (Exception ex)
        {
            _output.WriteLine($"[AfterStep Screenshot Error] {ex}");

            _allureReportingHelper.AttachText(
                "AfterStep Screenshot Error",
                ex.ToString());
        }
    }

    [AfterScenario(Order = 0)]
    public async Task SignOutAfterScenarioAsync()
    {
        try
        {
            if (!_driver.Page.IsClosed)
            {
                await _commonPage.SignOutAsync(_driver.Page);
            }
        }
        catch (PlaywrightException ex)
        {
            _output.WriteLine($"[SignOut Hook] Sign-out skipped: {ex.Message}");
        }
    }

    [AfterScenario(Order = 1)]
    public async Task AfterScenarioAsync()
    {
        IVideo? video = null;

        bool failed =
            _scenarioContext.ScenarioExecutionStatus == ScenarioExecutionStatus.TestError;

        try
        {
            if (!_driver.Page.IsClosed)
            {
                video = _driver.Page.Video;
            }

            await StopAndAttachScreencastAsync();

            if (failed)
            {
                _allureReportingHelper.AttachText(
                    "Scenario Failure",
                    _scenarioContext.TestError?.ToString() ?? "Scenario failed.");
            }

            await BrowserTearDownAsync(
                _scenarioContext,
                _output,
                _driver.Context,
                _driver.Page);

            AttachTraceToAllure();
        }
        catch (Exception ex)
        {
            _output.WriteLine($"[AfterScenario Error] {ex}");

            _allureReportingHelper.AttachText(
                "AfterScenario Error",
                ex.ToString());

            throw;
        }
        finally
        {
            await _driver.CloseAsync();

            if (_shouldRecordVideo && failed && video is not null && TestArtifacts is not null)
            {
                string? videoPath = await video.SaveRecordedVideoToArtifactsAsync(
                    TestArtifacts,
                    _output,
                    $"{TestName}_failure");

                if (!string.IsNullOrWhiteSpace(videoPath))
                {
                    _allureReportingHelper.AttachFile(
                        "Failure Video",
                        videoPath,
                        "video/webm");
                }
            }
        }
    }

    [BeforeTestRun]
    public static void EnforceArchitecture()
    {
        TestValidator.VerifyStepDefinitionsUseOnlyCoordinators(
            Assembly.GetExecutingAssembly());

        Dictionary<string, string> properties = new()
        {
            ["Browser"] = TestConfigReader.Settings.BrowserSettings.BrowserName,
            ["Environment"] = EnvironmentConfigFactory.CurrentEnvironment.ToString(),
            ["BaseUrl"] = EnvironmentConfigFactory.EnvironmentConfig.BaseUrl,
        };

        AllureReportingHelper.WriteEnvironmentProperties(properties);
    }

    private async Task StopAndAttachScreencastAsync()
    {
        ScreencastHelper? screencast = _scenarioContext.GetScreencast();

        if (screencast is null || TestArtifacts is null)
        {
            return;
        }

        await screencast.StopAsync();

        string screencastPath = TestArtifacts.FilePath($"{TestName}_screencast.webm");

        if (!File.Exists(screencastPath))
        {
            return;
        }

        _output.AddAttachmentAsLink(screencastPath);

        _allureReportingHelper.AttachFile(
            "Screencast",
            screencastPath,
            "video/webm");
    }

    private void AttachTraceToAllure()
    {
        if (TestArtifacts is null)
        {
            return;
        }

        string tracePath = TestArtifacts.FilePath($"{TestName}_trace.zip");

        if (!File.Exists(tracePath))
        {
            return;
        }

        _output.AddAttachmentAsLink(tracePath);

        _allureReportingHelper.AttachFile(
            "Playwright Trace",
            tracePath,
            "application/zip");
    }

    private BrowserNewContextOptions BuildContextOptions()
    {
        BrowserNewContextOptions options = new()
        {
            IgnoreHTTPSErrors = true
        };

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

        return tags.Any(t =>
            string.Equals(t, tag, StringComparison.OrdinalIgnoreCase));
    }

    private void AllureReportSetup()
    {
        string[] scenarioTags = _scenarioContext.ScenarioInfo.Tags;
        string[] featureTags = _featureContext.FeatureInfo.Tags ?? [];

        AllureApi.SetTestName(_scenarioContext.ScenarioInfo.Title);

        AllureApi.AddLabel("epic", GetTagValue(featureTags, "epic") ?? "IP Upload");
        AllureApi.AddLabel("feature", _featureContext.FeatureInfo.Title);
        AllureApi.AddLabel("story", _scenarioContext.ScenarioInfo.Title);
        AllureApi.AddLabel("suite", _featureContext.FeatureInfo.Title);

        AllureApi.SetSeverity(MapSeverity(GetTagValue(scenarioTags, "severity")));

        _allureReportingHelper.AddParameter("Feature", _featureContext.FeatureInfo.Title);
        _allureReportingHelper.AddParameter("Scenario", _scenarioContext.ScenarioInfo.Title);
        AllureLifecycle.Instance.UpdateTestCase(tc =>
        {
            tc.name = _scenarioContext.ScenarioInfo.Title;
        });
    }

    private static string? GetTagValue(string[] tags, string prefix)
    {
        string? tag = tags.FirstOrDefault(t =>
            t.StartsWith($"{prefix}:", StringComparison.OrdinalIgnoreCase));

        return tag?.Split(':').LastOrDefault()?.Trim();
    }

    private static SeverityLevel MapSeverity(string? severity)
    {
        return severity?.ToLower() switch
        {
            "blocker" => SeverityLevel.blocker,
            "critical" => SeverityLevel.critical,
            "normal" => SeverityLevel.normal,
            "minor" => SeverityLevel.minor,
            "trivial" => SeverityLevel.trivial,
            _ => SeverityLevel.normal
        };
    }
}