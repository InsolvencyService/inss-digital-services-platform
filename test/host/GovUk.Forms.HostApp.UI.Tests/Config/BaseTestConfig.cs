using GovUk.Forms.HostApp.UI.Tests.Config.Environments;
using GovUk.Forms.HostApp.UI.Tests.Extensions;
using GovUk.Forms.HostApp.UI.Tests.Helpers;

namespace GovUk.Forms.HostApp.UI.Tests.Config;

public class BaseTestConfig
{
    protected string BasePathForArtifacts { get; set; } = string.Empty;
    protected string TestOutputDir { get; set; } = string.Empty;
    protected string TestName { get; set; } = string.Empty;
    protected TestArtifacts? TestArtifacts { get; set; }

    public async Task BrowserSetupAsync(ScenarioContext scenarioContext, IPage page)
    {
        ArgumentNullException.ThrowIfNull(scenarioContext);
        ArgumentNullException.ThrowIfNull(page);

        InitializePaths();

        if (TestArtifacts == null)
        {
            TestArtifactsSetup(scenarioContext);
        }

        IEnvironmentConfig config = EnvironmentConfigFactory.EnvironmentConfig;

        LogTestStart();

        await NavigateToBaseUrlAsync(config.BaseUrl, page);
    }

    public void TestArtifactsSetup(ScenarioContext scenarioContext)
    {
        ArgumentNullException.ThrowIfNull(scenarioContext);

        InitializePaths();

        TestName = SanitizeFileName(scenarioContext.ScenarioInfo.Title);

        TestEnvironment environment = EnvironmentConfigFactory.CurrentEnvironment;

        TestArtifacts = new TestArtifacts(TestName, environment, BasePathForArtifacts);

        // Single source of truth
        TestOutputDir = TestArtifacts.Folder;
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

        ScenarioExecutionStatus outcome = scenarioContext.ScenarioExecutionStatus;

        try
        {
            await CaptureScreenshotAsync(outputHelper, page);

            LogTestEnd(outputHelper);

            await StopTracingSafeAsync(outputHelper, browserContext);

            AttachArtifacts(outputHelper);

            if (outcome == ScenarioExecutionStatus.TestError)
            {
                await HandleFailureAsync(scenarioContext, outputHelper);
            }
        }
        catch (Exception ex)
        {
            outputHelper.WriteLine($"[TearDown Error] {ex}");
            throw;
        }
    }

    private void InitializePaths()
    {
        BasePathForArtifacts = FileDirectoryExtensions.DirectoryPathCombine(
            TestContext.CurrentContext.WorkDirectory,
            "Screenshots-Report");
    }

    private void LogTestStart()
    {
        Console.WriteLine($"WorkDirectory: {TestContext.CurrentContext.WorkDirectory}");
        Console.WriteLine($"Artifacts folder: {TestArtifacts?.Folder}");
        Console.WriteLine($"=== Test Start: {TestName} | {DateTime.UtcNow:O} ===");
    }

    private static async Task NavigateToBaseUrlAsync(string baseUrl, IPage page)
    {
        IResponse response = await page.GotoAsync(
            baseUrl,
            new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded,
                Timeout = 8000
            }) ?? throw new InvalidOperationException($"Navigation returned null for '{baseUrl}'");

        if (!response.Ok)
        {
            throw new InvalidOperationException(
                $"Navigation failed: {response.Status} {response.StatusText} ({baseUrl})");
        }
    }

    protected async Task CaptureScreenshotAsync(IReqnrollOutputHelper outputHelper, IPage page)
    {
        if (TestArtifacts == null)
        {
            return;
        }

        string fileName = $"{TestName}_Final_{DateTime.UtcNow:HH-mm-ss}.jpg";
        string path = TestArtifacts.FilePath(fileName);

        await page.TakeScreenshotAsync(outputHelper, path);
    }

    private void LogTestEnd(IReqnrollOutputHelper outputHelper)
    {
        outputHelper.WriteLine($"=== Test End: {TestName} | {DateTime.UtcNow:O} ===");
    }

    private async Task StopTracingSafeAsync(IReqnrollOutputHelper outputHelper, IBrowserContext browserContext)
    {
        if (TestArtifacts == null)
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

    private void AttachArtifacts(IReqnrollOutputHelper outputHelper)
    {
        if (TestArtifacts == null)
        {
            return;
        }

        if (File.Exists(TestArtifacts.ConsoleLogPath))
        {
            outputHelper.AddAttachment(TestArtifacts.ConsoleLogPath);
        }

        if (File.Exists(TestArtifacts.FailureLogPath))
        {
            outputHelper.AddAttachment(TestArtifacts.FailureLogPath);
        }
    }

    private async Task HandleFailureAsync(
        ScenarioContext scenarioContext,
        IReqnrollOutputHelper outputHelper)
    {
        ArgumentNullException.ThrowIfNull(scenarioContext);

        Console.WriteLine($"Test '{TestName}' failed. Collecting artifacts...");

        if (TestArtifacts != null)
        {
            string message = BuildFailureMessage();
            await SaveFileAsync(TestArtifacts.FailureLogPath, message, outputHelper);
        }
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

    protected async Task SaveVideoAsync(IReqnrollOutputHelper outputHelper, IPage page)
    {
        if (page.Video == null || TestArtifacts == null)
        {
            return;
        }

        try
        {
            string videoPath = TestArtifacts.FilePath($"{TestName}_failure.webm");

            await page.Video.SaveAsAsync(videoPath);

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

    protected static async Task SaveFileAsync(
        string filePath,
        string content,
        IReqnrollOutputHelper outputHelper)
    {
        string directory = Path.GetDirectoryName(filePath)
            ?? throw new InvalidOperationException("Invalid file path.");

        Directory.CreateDirectory(directory);

        await File.WriteAllTextAsync(filePath, content);

        outputHelper.WriteLine($"File saved: {filePath}");
        outputHelper.AddAttachmentAsLink(filePath);
    }

    protected static string SanitizeFileName(string name)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(c, '_');
        }

        return name;
    }

    public static string Sanitize(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return "empty";
        }

        char[] invalidChars = Path.GetInvalidFileNameChars();

        string cleaned = new(
            name.Select(c => invalidChars.Contains(c) ? '_' : c).ToArray());

        return cleaned.Replace(" ", "_");
    }
}