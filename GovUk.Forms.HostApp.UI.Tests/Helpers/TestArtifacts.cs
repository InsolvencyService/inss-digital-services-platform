using GovUk.Forms.HostApp.UI.Tests.Config.Environments;
using GovUk.Forms.HostApp.UI.Tests.Extensions;
using System.Globalization;


namespace GovUk.Forms.HostApp.UI.Tests.Helpers;

public class TestArtifacts
{
    public string Folder { get; }
    private readonly string _testName;

    public string ConsoleLogPath => FilePath($"Console-Log.txt");
    public string TracePath => FilePath($"Trace_{TestNameWithDateTimestamp()}.zip");
    public string FailureLogPath => FilePath($"Fail_{TestNameWithDateTimestamp()}.txt");
    public string VideoPath => FilePath($"{TestNameWithDateTimestamp()}.webm");



    public TestArtifacts(string testName, TestEnvironment environmentType, string WorkDrirectory)
    {
        _testName = testName;
        string date = DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        Folder = Path.Combine(WorkDrirectory, "Reports", environmentType.ToString(), date, TestNameWithDateTimestamp());
        Directory.CreateDirectory(Folder);
    }

    private string TestNameWithDateTimestamp()
    {
        string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HH-mm-ss", CultureInfo.InvariantCulture);
        return $"{_testName}_{timestamp}";
    }

    public string FilePath(string fileName) => Folder.FilePathCombine(fileName);


}
