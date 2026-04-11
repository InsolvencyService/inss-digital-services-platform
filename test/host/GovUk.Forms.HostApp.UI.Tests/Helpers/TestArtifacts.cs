using GovUk.Forms.HostApp.UI.Tests.Config.Environments;
using System.Globalization;


namespace GovUk.Forms.HostApp.UI.Tests.Helpers;

public sealed class TestArtifacts
{
    public string Folder { get; }
    public string TestName { get; }
    public string RunId { get; }

    public string ScreenshotsDirectory => Path.Combine(Folder, "screenshots");
    public string VideoDirectory => Path.Combine(Folder, "videos");

    public string ConsoleLogPath => FilePath("Console-Log.txt");
    public string TracePath => FilePath($"Trace_{RunId}.zip");
    public string FailureLogPath => FilePath($"Fail_{RunId}.txt");

    public TestArtifacts(
        string testName,
        TestEnvironment environmentType,
        string workDirectory)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(testName);
        ArgumentException.ThrowIfNullOrWhiteSpace(workDirectory);

        TestName = Sanitize(testName);

        DateTime now = DateTime.UtcNow;

        string timestamp = now.ToString("yyyyMMdd_HH-mm-ss_fff", CultureInfo.InvariantCulture);
        string date = now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        RunId = $"{TestName}_{timestamp}";

        Folder = Path.Combine(
            workDirectory,
            "Reports",
            environmentType.ToString(),
            date,
            RunId);

        // Ensure directories exist (parallel-safe)
        Directory.CreateDirectory(Folder);
        Directory.CreateDirectory(VideoDirectory);
        Directory.CreateDirectory(ScreenshotsDirectory);
    }


    public string GetScreenshotPath(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return Path.Combine(
            ScreenshotsDirectory,
            $"{Sanitize(name)}.png");
    }

    public string GetVideoPath(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return Path.Combine(
            VideoDirectory,
            $"{Sanitize(name)}.webm");
    }

    public string FilePath(string fileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

        return Path.Combine(Folder, fileName);
    }



    private static string Sanitize(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        foreach (char invalid in Path.GetInvalidFileNameChars())
        {
            value = value.Replace(invalid, '_');
        }

        value = value.Replace(' ', '_');

        return string.IsNullOrWhiteSpace(value)
            ? "artifact"
            : value;
    }
}
