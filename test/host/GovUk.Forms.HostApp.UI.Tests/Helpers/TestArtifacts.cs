using GovUk.Forms.HostApp.UI.Tests.Config.Environments;
using GovUk.Forms.HostApp.UI.Tests.Extensions;
using System.Globalization;


namespace GovUk.Forms.HostApp.UI.Tests.Helpers;

public class TestArtifacts
{
    public string Folder { get; }
    private readonly string _testName;
    private readonly string _runId;

    public string ConsoleLogPath => FilePath("Console-Log.txt");
    public string TracePath => FilePath($"Trace_{_runId}.zip");
    public string FailureLogPath => FilePath($"Fail_{_runId}.txt");
    public string VideoDirectory => Path.Combine(Folder, "videos");

    public TestArtifacts(string testName, TestEnvironment environmentType, string workDirectory)
    {
        _testName = Sanitize(testName);

        string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HH-mm-ss_fff", CultureInfo.InvariantCulture);
        _runId = $"{_testName}_{timestamp}";

        string date = DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        Folder = FileDirectoryExtensions.DirectoryPathCombine(
            workDirectory,
            "Reports",
            environmentType.ToString(),
            date,
            _runId);

        Directory.CreateDirectory(Folder);
        Directory.CreateDirectory(VideoDirectory);
        Directory.CreateDirectory(Path.Combine(Folder, "screenshots"));
    }

    public string GetScreenshotPath(string name)
    {
        string safeName = Sanitize(name);
        return Path.Combine(Folder, "screenshots", $"{safeName}.png");
    }

    public string FilePath(string fileName) => Folder.FilePathCombine(fileName);

    private static string Sanitize(string value)
    {
        foreach (char invalidChar in Path.GetInvalidFileNameChars())
        {
            value = value.Replace(invalidChar, '_');
        }

        return value.Replace(' ', '_');
    }
}
