using GovUk.Forms.HostApp.UI.Test.Config.Environments;
using GovUk.Forms.HostApp.UI.Test.Extensions;
using GovUk.Forms.HostApp.UI.Test.Helpers;

namespace GovUk.Forms.HostApp.UI.Test.Config;

public abstract class BaseTestConfig
{
    protected string BasePathForArtifacts { get; private set; } = string.Empty;
    protected string TestOutputDir { get; private set; } = string.Empty;
    protected string TestName { get; private set; } = string.Empty;
    protected TestArtifacts? TestArtifacts { get; private set; }

    public async Task BrowserSetupAsync(
        ScenarioContext scenarioContext,
        IPage page,
        IBrowserContext browserContext)
    {
        ArgumentNullException.ThrowIfNull(scenarioContext);
        ArgumentNullException.ThrowIfNull(page);
        ArgumentNullException.ThrowIfNull(browserContext);

        EnsureArtifactBasePath();
        EnsureTestArtifactsCreated(scenarioContext);

        IEnvironmentConfig config = EnvironmentConfigFactory.EnvironmentConfig;

        LogTestStart();

        await StartTracingSafeAsync(browserContext);
        await NavigateToBaseUrlAsync(config.BaseUrl, page);
    }

    public void TestArtifactsSetup(ScenarioContext scenarioContext)
    {
        ArgumentNullException.ThrowIfNull(scenarioContext);

        EnsureArtifactBasePath();
        EnsureTestArtifactsCreated(scenarioContext);
    }

    public async Task BrowserTearDownAsync(
        ScenarioContext scenarioContext,
        IReqnrollOutputHelper outputHelper,
        IBrowserContext browserContext,
        IPage page)
    {
        ArgumentNullException.ThrowIfNull(scenarioContext);
        ArgumentNullException.ThrowIfNull(outputHelper);
        ArgumentNullException.ThrowIfNull(browserContext);
        ArgumentNullException.ThrowIfNull(page);

        if (TestArtifacts is null)
        {
            outputHelper.WriteLine("[TearDown] TestArtifacts was null. Skipping artifact collection.");
            return;
        }

        IVideo? video = page.Video;
        bool failed = scenarioContext.ScenarioExecutionStatus == ScenarioExecutionStatus.TestError;

        try
        {
            LogTestEnd(outputHelper);

            await page.SaveFinalScreenshotAsync(TestArtifacts, outputHelper, TestName);

            await StopTracingSafeAsync(browserContext, outputHelper);

            if (failed)
            {
                await HandleFailureAsync(outputHelper);
            }

            if (!page.IsClosed)
            {
                await page.CloseAsync();
            }

            if (failed && video is not null)
            {
                await SaveVideoAsync(outputHelper, video);
            }

            AttachArtifacts(outputHelper);
        }
        catch (Exception ex)
        {
            outputHelper.WriteLine($"[TearDown Error] {ex}");
            throw;
        }
    }

    protected async Task SaveVideoAsync(
        IReqnrollOutputHelper outputHelper,
        IVideo video)
    {
        ArgumentNullException.ThrowIfNull(outputHelper);
        ArgumentNullException.ThrowIfNull(video);

        if (TestArtifacts is null)
        {
            outputHelper.WriteLine("[Video] TestArtifacts was null. Skipping video save.");
            return;
        }

        try
        {
            string videoPath = TestArtifacts.GetVideoPath($"{TestName}_failure");

            await video.SaveAsAsync(videoPath);

            outputHelper.WriteLine($"Video saved: {videoPath}");

            if (File.Exists(videoPath))
            {
                outputHelper.AddAttachment(videoPath);
            }
        }
        catch (Exception ex)
        {
            outputHelper.WriteLine($"[Video Error] {ex.Message}");
        }
    }

    protected async Task SaveFailureLogAsync(IReqnrollOutputHelper outputHelper)
    {
        ArgumentNullException.ThrowIfNull(outputHelper);

        if (TestArtifacts is null)
        {
            return;
        }

        string content = BuildFailureMessage();
        await SaveFileAsync(TestArtifacts.FailureLogPath, content, outputHelper);
    }

    protected static async Task SaveFileAsync(
        string filePath,
        string content,
        IReqnrollOutputHelper outputHelper)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(outputHelper);

        string directory = Path.GetDirectoryName(filePath)
            ?? throw new InvalidOperationException("Invalid file path.");

        Directory.CreateDirectory(directory);

        await File.WriteAllTextAsync(filePath, content);

        outputHelper.WriteLine($"File saved: {filePath}");

        if (File.Exists(filePath))
        {
            outputHelper.AddAttachmentAsLink(filePath);
        }
    }

    protected static string SanitizeFileName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return "empty";
        }

        foreach (char invalidChar in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(invalidChar, '_');
        }

        return name.Replace(' ', '_');
    }

    private static async Task StartTracingSafeAsync(IBrowserContext browserContext)
    {
        ArgumentNullException.ThrowIfNull(browserContext);

        try
        {
            await browserContext.Tracing.StartAsync(new TracingStartOptions
            {
                Title = TestContext.CurrentContext.Test.Name,
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Tracing Start Error] {ex.Message}");
        }
    }

    private async Task StopTracingSafeAsync(
        IBrowserContext browserContext,
        IReqnrollOutputHelper outputHelper)
    {
        ArgumentNullException.ThrowIfNull(browserContext);
        ArgumentNullException.ThrowIfNull(outputHelper);

        if (TestArtifacts is null)
        {
            return;
        }

        try
        {
            await browserContext.Tracing.StopAsync(new TracingStopOptions
            {
                Path = TestArtifacts.TracePath
            });

            if (File.Exists(TestArtifacts.TracePath))
            {
                outputHelper.AddAttachment(TestArtifacts.TracePath);
            }
        }
        catch (Exception ex)
        {
            outputHelper.WriteLine($"[Tracing Error] {ex.Message}");
        }
    }

    private void EnsureArtifactBasePath()
    {
        if (!string.IsNullOrWhiteSpace(BasePathForArtifacts))
        {
            return;
        }

        BasePathForArtifacts = Path.Combine(
            TestContext.CurrentContext.WorkDirectory,
            "Screenshots-Report");
    }

    private void EnsureTestArtifactsCreated(ScenarioContext scenarioContext)
    {
        if (TestArtifacts is not null)
        {
            return;
        }

        TestName = SanitizeFileName(scenarioContext.ScenarioInfo.Title);

        TestEnvironment environment = EnvironmentConfigFactory.CurrentEnvironment;

        TestArtifacts = new TestArtifacts(
            TestName,
            environment,
            BasePathForArtifacts);

        TestOutputDir = TestArtifacts.Folder;
    }

    private void LogTestStart()
    {
        Console.WriteLine($"WorkDirectory: {TestContext.CurrentContext.WorkDirectory}");
        Console.WriteLine($"Artifacts folder: {TestArtifacts?.Folder}");
        Console.WriteLine($"=== Test Start: {TestName} | {DateTime.UtcNow:O} ===");
    }

    private void LogTestEnd(IReqnrollOutputHelper outputHelper)
    {
        outputHelper.WriteLine($"=== Test End: {TestName} | {DateTime.UtcNow:O} ===");
    }

    private static async Task NavigateToBaseUrlAsync(string baseUrl, IPage page)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(baseUrl);
        ArgumentNullException.ThrowIfNull(page);

        IResponse response = await page.GotoAsync(
            baseUrl,
            new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded,
                Timeout = 8_000
            }) ?? throw new InvalidOperationException(
                $"Navigation returned null for '{baseUrl}'.");

        if (!response.Ok)
        {
            throw new InvalidOperationException(
                $"Navigation failed: {response.Status} {response.StatusText} ({baseUrl})");
        }
    }

    private void AttachArtifacts(IReqnrollOutputHelper outputHelper)
    {
        ArgumentNullException.ThrowIfNull(outputHelper);

        if (TestArtifacts is null)
        {
            return;
        }

        AttachIfExists(outputHelper, TestArtifacts.ConsoleLogPath);
        AttachIfExists(outputHelper, TestArtifacts.FailureLogPath);
        AttachIfExists(outputHelper, TestArtifacts.TracePath);
    }

    private static void AttachIfExists(IReqnrollOutputHelper outputHelper, string path)
    {
        if (File.Exists(path))
        {
            outputHelper.AddAttachment(path);
        }
    }

    private async Task HandleFailureAsync(IReqnrollOutputHelper outputHelper)
    {
        ArgumentNullException.ThrowIfNull(outputHelper);

        outputHelper.WriteLine($"Test '{TestName}' failed. Collecting failure artifacts...");
        await SaveFailureLogAsync(outputHelper);
    }

    private string BuildFailureMessage()
    {
        TestContext.ResultAdapter result = TestContext.CurrentContext.Result;

        return $"""
                Test Failed: {TestName}
                Time: {DateTime.UtcNow:O}
                Environment: {EnvironmentConfigFactory.CurrentEnvironment}

                Message:
                {result.Message}

                Stack Trace:
                {result.StackTrace}
                """;
    }
}