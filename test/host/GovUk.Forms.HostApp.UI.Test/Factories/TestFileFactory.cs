using GovUk.Forms.HostApp.UI.Test.Helpers;
using System.Text;

namespace GovUk.Forms.HostApp.UI.Test.Factories;

public static class TestFileFactory
{
    private const int OneMb = 1024 * 1024;

    private const string BaselineRp14aFilePath =
        "Resources/Rp14a/rp14A.xml";

    public static string CreateValidXmlFileAtSize(
        TestArtifacts artifacts,
        int targetSizeMb)
    {
        ArgumentNullException.ThrowIfNull(artifacts);

        string filePath = artifacts.FilePath(
            $"rp14a-{targetSizeMb}mb.xml");

        string xml = File.ReadAllText(
            BaselineRp14aFilePath,
            Encoding.UTF8);

        StringBuilder builder = new(xml);

        int targetBytes = targetSizeMb * OneMb;

        while (Encoding.UTF8.GetByteCount(builder.ToString()) < targetBytes)
        {
            builder.Insert(
                builder.Length - "</ns1:RP14A>".Length,
                """
                <!-- Padding Padding Padding Padding Padding -->
                """);
        }

        File.WriteAllText(
            filePath,
            builder.ToString(),
            Encoding.UTF8);

        return filePath;
    }

    public static string CreateValidXmlFileAboveSize(
        TestArtifacts artifacts,
        int maxSizeMb)
    {
        ArgumentNullException.ThrowIfNull(artifacts);

        string filePath = artifacts.FilePath(
            $"rp14a-above-{maxSizeMb}mb.xml");

        string xml = File.ReadAllText(
            BaselineRp14aFilePath,
            Encoding.UTF8);

        StringBuilder builder = new(xml);

        int targetBytes = (maxSizeMb * OneMb) + 1;

        while (Encoding.UTF8.GetByteCount(builder.ToString()) < targetBytes)
        {
            builder.Insert(
                builder.Length - "</ns1:RP14A>".Length,
                """
                <!-- Padding Padding Padding Padding Padding -->
                """);
        }

        File.WriteAllText(
            filePath,
            builder.ToString(),
            Encoding.UTF8);

        return filePath;
    }

    public static string CreateFile(
        TestArtifacts artifacts,
        string fileName,
        string content)
    {
        ArgumentNullException.ThrowIfNull(artifacts);

        string filePath = artifacts.FilePath(fileName);

        File.WriteAllText(filePath, content);

        return filePath;
    }

    public static string CreateUnsupportedFile(
        TestArtifacts artifacts,
        string extension)
    {
        return CreateFile(
            artifacts,
            $"unsupported-file{extension}",
            "This is not a valid RP14A XML file.");
    }

    public static string CreateXmlWithWrongContent(
        TestArtifacts artifacts)
    {
        ArgumentNullException.ThrowIfNull(artifacts);

        string filePath = artifacts.FilePath(
            "invalid-rp14a-structure.xml");

        File.WriteAllText(
            filePath,
            """
            <InvalidRP14A>
              <Employee>
                <Header>
                  <CaseReference>CN12445678</CaseReference>
                </Header>
                <EmployerName>Employer Test</EmployerName>
              </Employee>
            </InvalidRP14A>
            """);

        return filePath;
    }
}
