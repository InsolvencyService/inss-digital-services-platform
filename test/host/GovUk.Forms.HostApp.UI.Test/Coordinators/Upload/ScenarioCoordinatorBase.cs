using GovUk.Forms.HostApp.UI.Test.Helpers;
using System.Diagnostics;
using System.Globalization;

namespace GovUk.Forms.HostApp.UI.Test.Coordinators.Upload;

public abstract class ScenarioCoordinatorBase
{
    private readonly string _logTag;

    protected IFileUploadCoordinator FileUploadCoordinator { get; }
    protected ScenarioContext ScenarioContext { get; }
    protected TestArtifacts TestArtifacts { get; }
    protected string? BaselineFilePath { get; }

    protected ScenarioCoordinatorBase(
        IFileUploadCoordinator fileUploadCoordinator,
        ScenarioContext scenarioContext,
        TestArtifacts testArtifacts,
        string logTag,
        string? defaultBaselineFilePath = null,
        string? baselineFilePath = null)
    {
        FileUploadCoordinator = fileUploadCoordinator
            ?? throw new ArgumentNullException(nameof(fileUploadCoordinator));

        ScenarioContext = scenarioContext
            ?? throw new ArgumentNullException(nameof(scenarioContext));

        TestArtifacts = testArtifacts
            ?? throw new ArgumentNullException(nameof(testArtifacts));

        _logTag = logTag;
        BaselineFilePath = ResolveBaselineFilePath(baselineFilePath, defaultBaselineFilePath, logTag);
    }

    protected string ScenarioName =>
        ScenarioContext.ScenarioInfo?.Title ?? "Unknown Scenario";

    protected async Task UploadFileAsync(string filePath, string description)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                throw new InvalidOperationException(
                    $"{_logTag} test file was not created at expected location: {filePath}");
            }

            LogInfo($"Uploading {description} from {filePath}");

            await FileUploadCoordinator.UploadFileAsync(filePath);

            LogInfo($"Successfully uploaded {description}");
        }
        catch (Exception ex)
        {
            LogError($"Failed to upload {description}: {ex.GetType().Name} - {ex.Message}");
            throw;
        }
    }

    protected void LogInfo(string message) => LogMessage("INFO", message);

    protected void LogError(string message) => LogMessage("ERROR", message);

    private void LogMessage(string level, string message)
    {
        string timestamp = DateTime.UtcNow.ToString(
            "yyyy-MM-dd HH:mm:ss.fff",
            CultureInfo.InvariantCulture);

        Debug.WriteLine($"[{timestamp}] [{level}] [{_logTag}] {message}");
    }

    protected static string FormatDate(DateOnly? date) =>
        date?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? string.Empty;

    protected static string ToLogValue(string? value) =>
        value switch
        {
            null => "<null>",
            "" => "<empty>",
            _ when string.IsNullOrWhiteSpace(value) => "<whitespace>",
            _ => value
        };

    protected static void ValidateDateOrder(DateOnly? startDate, DateOnly? endDate)
    {
        if (startDate.HasValue &&
            endDate.HasValue &&
            endDate < startDate)
        {
            throw new ArgumentException(
                $"End date ({endDate:yyyy-MM-dd}) cannot be before start date ({startDate:yyyy-MM-dd})",
                nameof(endDate));
        }
    }

    protected static void ValidatePositiveNumber(int value, string parameterName)
    {
        if (value <= 0)
        {
            throw new ArgumentException(
                $"Parameter '{parameterName}' must be greater than 0. Received: {value}",
                parameterName);
        }
    }

    protected static void ValidateNonNegativeNumber(int value, string parameterName)
    {
        if (value < 0)
        {
            throw new ArgumentException(
                $"Parameter '{parameterName}' must be non-negative. Received: {value}",
                parameterName);
        }
    }

    protected static void ValidateRange(int value, int min, int max, string parameterName)
    {
        if (value < min || value > max)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                $"Parameter '{parameterName}' must be between {min} and {max}. Received: {value}");
        }
    }

    private static string? ResolveBaselineFilePath(
        string? baselineFilePath,
        string? defaultPath,
        string logTag)
    {
        string? effectivePath = baselineFilePath ?? defaultPath;

        if (effectivePath is null)
        {
            return null;
        }

        string absolutePath = Path.IsPathRooted(effectivePath)
            ? effectivePath
            : Path.Join(AppContext.BaseDirectory, effectivePath);

        if (!File.Exists(absolutePath))
        {
            throw new FileNotFoundException(
                $"Baseline {logTag} file not found at: {absolutePath}",
                absolutePath);
        }

        return absolutePath;
    }
}
