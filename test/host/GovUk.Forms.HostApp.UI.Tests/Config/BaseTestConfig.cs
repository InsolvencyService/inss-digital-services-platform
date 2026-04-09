using GovUk.Forms.HostApp.UI.Tests.Config.Environments;
using GovUk.Forms.HostApp.UI.Tests.Extensions;
using GovUk.Forms.HostApp.UI.Tests.Helpers;
using Microsoft.Playwright;
using Reqnroll;

namespace GovUk.Forms.HostApp.UI.Tests.Config;

public class BaseTestConfig
{
    protected string BasePathForArtifacts { get; private set; } = string.Empty;
    protected string TestOutputDir { get; private set; } = string.Empty;
    protected string TestName { get; private set; } = string.Empty;
    protected TestArtifacts? TestArtifacts { get; private set; }

    /// <summary>
    /// Initializes browser, tracing, logging, and navigates to base URL.
    /// Should be called at the beginning of each scenario.
    /// </summary>
    public async Task BrowserSetupAsync(ScenarioContext scenarioContext, IPage page)
    {
        ArgumentNullException.ThrowIfNull(scenarioContext);

        // Ensure artifact base directory is set
        InitializePaths();

        // Sanitize test name to avoid invalid file path characters
        TestName = SanitizeFileName(scenarioContext.ScenarioInfo.Title);

        // Resolve environment configuration
        TestEnvironment environment = EnvironmentConfigFactory.CurrentEnvironment;
        IEnvironmentConfig config = EnvironmentConfigFactory.EnvironmentConfig;

        // Initialize artifact manager
        TestArtifacts = new TestArtifacts(TestName, environment, BasePathForArtifacts);

        // Create test-specific output directory
        TestOutputDir = GetOutputDirectory(environment, TestName);
        Directory.CreateDirectory(TestOutputDir);

        LogTestStart();

        // Navigate to application under test
        await NavigateToBaseUrlAsync(config.BaseUrl, page);
    }

    /// <summary>
    /// Handles teardown:
    /// - Always attempts to capture artifacts
    /// - Handles failures gracefully
    /// - Ensures tracing is stopped
    /// </summary>
    public async Task BrowserTearDownAsync(
        ScenarioContext scenarioContext,
        IReqnrollOutputHelper outputHelper,
        IBrowserContext browserContext,
        IPage page)
    {
        ArgumentNullException.ThrowIfNull(scenarioContext);
        ArgumentNullException.ThrowIfNull(outputHelper);

        ScenarioExecutionStatus outcome = scenarioContext.ScenarioExecutionStatus;

        try
        {
            // Always attempt final screenshot (even on pass)
            await CaptureFinalScreenshotAsync(outputHelper, page);

            LogTestEnd(outputHelper);

            // Ensure tracing is stopped and saved
            await StopTracingSafeAsync(outputHelper, browserContext);

            // Attach any existing artifacts (logs etc.)
            AttachArtifacts(outputHelper);

            // Only perform heavy diagnostics on failure
            if (outcome == ScenarioExecutionStatus.TestError)
            {
                await HandleFailureAsync(scenarioContext, outputHelper, page);
            }
        }
        catch (Exception ex)
        {
            // Never silently swallow teardown failures
            outputHelper.WriteLine($"[TearDown Error] {ex}");
            throw;
        }
    }


    /// <summary>
    /// Initializes the root artifact directory.
    /// </summary>
    private void InitializePaths()
    {
        BasePathForArtifacts = FileDirectoryExtensions.DirectoryPathCombine(
            TestContext.CurrentContext.WorkDirectory,
            "Screenshots-Report");
    }

    /// <summary>
    /// Logs test start metadata for traceability.
    /// </summary>
    private void LogTestStart()
    {
        Console.WriteLine($"WorkDirectory: {TestContext.CurrentContext.WorkDirectory}");
        Console.WriteLine($"Artifacts folder: {TestArtifacts?.Folder}");
        Console.WriteLine($"=== Test Start: {TestName} | {DateTime.UtcNow:O} ===");
    }


    /// <summary>
    /// Navigates to base URL and validates response.
    /// </summary>
    private static async Task NavigateToBaseUrlAsync(string baseUrl, IPage page)
    {
        IResponse response = await page.GotoAsync(baseUrl, new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded, Timeout = 8000 })
            ?? throw new InvalidOperationException($"Navigation returned null for '{baseUrl}'");

        if (!response.Ok)
        {
            throw new InvalidOperationException(
                $"Navigation failed: {response.Status} {response.StatusText} ({baseUrl})");
        }
    }


    /// <summary>
    /// Captures a final screenshot regardless of test outcome.
    /// </summary>
    private async Task CaptureFinalScreenshotAsync(IReqnrollOutputHelper outputHelper, IPage page)
    {
        if (page == null || TestArtifacts == null)
        {
            return;
        }

        string fileName = $"{TestName}_Final_{DateTime.UtcNow:HH-mm-ss}.jpg";
        string path = TestArtifacts.FilePath(fileName);

        await page.TakeScreenshotAsync(outputHelper, path);
    }

    /// <summary>
    /// Logs test end timestamp.
    /// </summary>
    private void LogTestEnd(IReqnrollOutputHelper outputHelper)
    {
        outputHelper.WriteLine($"=== Test End: {TestName} | {DateTime.UtcNow:O} ===");
    }

    /// <summary>
    /// Stops tracing and safely attaches trace file if available.
    /// </summary>
    private async Task StopTracingSafeAsync(IReqnrollOutputHelper outputHelper, IBrowserContext browserContext)
    {
        if (browserContext == null || TestArtifacts == null)
        {
            return;
        }

        try
        {
            await browserContext.Tracing.StopAsync(new()
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
            // Do not fail test due to tracing issues
            outputHelper.WriteLine($"[Tracing Error] {ex.Message}");
        }
    }

    /// <summary>
    /// Attaches any existing artifacts (e.g., console logs).
    /// </summary>
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
    }

    /// <summary>
    /// Handles failure-specific diagnostics:
    /// - failure log
    /// - video capture
    /// </summary>
    private async Task HandleFailureAsync(
        ScenarioContext scenarioContext,
        IReqnrollOutputHelper outputHelper,
        IPage page)
    {
        ArgumentNullException.ThrowIfNull(scenarioContext);
        Console.WriteLine($"Test '{TestName}' failed. Collecting artifacts...");

        if (TestArtifacts != null)
        {
            string message = BuildFailureMessage();
            await SaveFileAsync(TestArtifacts.FailureLogPath, message, outputHelper);
        }

        await SaveVideoAsync(outputHelper, page);
    }

    /// <summary>
    /// Builds a detailed failure message using NUnit test context.
    /// </summary>
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

    /// <summary>
    /// Saves Playwright video if available.
    /// </summary>
    private async Task SaveVideoAsync(IReqnrollOutputHelper outputHelper, IPage page)
    {
        if (page?.Video == null || TestArtifacts?.VideoPath == null)
        {
            return;
        }

        await page.Video.SaveAsAsync(TestArtifacts.VideoPath);

        outputHelper.WriteLine($"Video saved: {TestArtifacts.VideoPath}");
        outputHelper.AddAttachmentAsLink(TestArtifacts.VideoPath);
    }

    /// <summary>
    /// Writes content to file and attaches it to test output.
    /// Ensures directory exists.
    /// </summary>
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

    /// <summary>
    /// Builds structured output directory:
    /// /Artifacts/{Env}/{Date}/{TestName}
    /// </summary>
    protected string GetOutputDirectory(TestEnvironment environment, string testName) =>
        FileDirectoryExtensions.DirectoryPathCombine(
            BasePathForArtifacts,
            environment.ToString(),
            DateTime.UtcNow.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
            testName);

    /// <summary>
    /// Removes invalid file system characters from test name.
    /// </summary>
    private static string SanitizeFileName(string name)
    {
        foreach (char c in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(c, '_');
        }
        return name;
    }
}