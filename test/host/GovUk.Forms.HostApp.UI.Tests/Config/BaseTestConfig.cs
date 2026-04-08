using GovUk.Forms.HostApp.UI.Tests.Config.Environments;
using GovUk.Forms.HostApp.UI.Tests.Extensions;
using GovUk.Forms.HostApp.UI.Tests.Helpers;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using Reqnroll;

namespace GovUk.Forms.HostApp.UI.Tests.Config;

public class BaseTestConfig : PageTest
{
    protected string BasePathForArtifacts { get; private set; } = string.Empty;
    protected string TestOutputDir { get; private set; } = string.Empty;
    protected string TestName { get; private set; } = string.Empty;
    protected TestArtifacts? TestArtifacts { get; private set; }

    /// <summary>
    /// Initializes browser, tracing, logging, and navigates to base URL.
    /// Should be called at the beginning of each scenario.
    /// </summary>
    public async Task BrowserSetupAsync(ScenarioContext scenarioContext)
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

        // Attach browser console logging for debugging frontend issues
        AttachConsoleLogging();

        // Start Playwright tracing (screenshots + DOM snapshots)
        await StartTracingSafeAsync();

        // Navigate to application under test
        await NavigateToBaseUrlAsync(config.BaseUrl);
    }

    /// <summary>
    /// Handles teardown:
    /// - Always attempts to capture artifacts
    /// - Handles failures gracefully
    /// - Ensures tracing is stopped
    /// </summary>
    public async Task BrowserTearDownAsync(
        ScenarioContext scenarioContext,
        IReqnrollOutputHelper outputHelper)
    {
        ArgumentNullException.ThrowIfNull(scenarioContext);
        ArgumentNullException.ThrowIfNull(outputHelper);

        ScenarioExecutionStatus outcome = scenarioContext.ScenarioExecutionStatus;

        try
        {
            // Always attempt final screenshot (even on pass)
            await CaptureFinalScreenshotAsync(outputHelper);

            LogTestEnd(outputHelper);

            // Ensure tracing is stopped and saved
            await StopTracingSafeAsync(outputHelper);

            // Attach any existing artifacts (logs etc.)
            AttachArtifacts(outputHelper);

            // Only perform heavy diagnostics on failure
            if (outcome == ScenarioExecutionStatus.TestError)
            {
                await HandleFailureAsync(scenarioContext, outputHelper);
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
    /// Captures browser console messages and pipes them to test output.
    /// Useful for debugging client-side issues.
    /// </summary>
    private void AttachConsoleLogging()
    {
        Page.Console += (_, msg) =>
        {
            Console.WriteLine($"[Browser Console] {msg.Type}: {msg.Text}");
        };
    }

    /// <summary>
    /// Starts Playwright tracing safely.
    /// Will not throw if context is unavailable.
    /// </summary>
    private async Task StartTracingSafeAsync()
    {
        if (Context == null)
        {
            return;
        }

        await Context.Tracing.StartAsync(new()
        {
            Title = TestName,
            Screenshots = true,
            Snapshots = true,
            Sources = true
        });
    }

    /// <summary>
    /// Navigates to base URL and validates response.
    /// </summary>
    private async Task NavigateToBaseUrlAsync(string baseUrl)
    {
        IResponse response = await Page.GotoAsync(baseUrl)
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
    private async Task CaptureFinalScreenshotAsync(IReqnrollOutputHelper outputHelper)
    {
        if (Page == null || TestArtifacts == null)
        {
            return;
        }

        string fileName = $"{TestName}_Final_{DateTime.UtcNow:HH-mm-ss}.jpg";
        string path = TestArtifacts.FilePath(fileName);

        await Page.TakeScreenshotAsync(outputHelper, path);
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
    private async Task StopTracingSafeAsync(IReqnrollOutputHelper outputHelper)
    {
        if (Context == null || TestArtifacts == null)
        {
            return;
        }

        try
        {
            await Context.Tracing.StopAsync(new()
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
        IReqnrollOutputHelper outputHelper)
    {
        ArgumentNullException.ThrowIfNull(scenarioContext);
        Console.WriteLine($"Test '{TestName}' failed. Collecting artifacts...");

        if (TestArtifacts != null)
        {
            string message = BuildFailureMessage();
            await SaveFileAsync(TestArtifacts.FailureLogPath, message, outputHelper);
        }

        await SaveVideoAsync(outputHelper);
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
    private async Task SaveVideoAsync(IReqnrollOutputHelper outputHelper)
    {
        if (Page?.Video == null || TestArtifacts?.VideoPath == null)
        {
            return;
        }

        await Page.Video.SaveAsAsync(TestArtifacts.VideoPath);

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