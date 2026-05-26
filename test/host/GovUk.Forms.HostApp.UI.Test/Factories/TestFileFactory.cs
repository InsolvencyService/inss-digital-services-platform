using GovUk.Forms.HostApp.UI.Test.Helpers;
using System.Text;

namespace GovUk.Forms.HostApp.UI.Test.Factories;

public static class TestFileFactory
{
    private const int OneMb = 1024 * 1024;

    private static readonly string _baselineRp14aFilePath = Path.Combine(
        Path.GetDirectoryName(typeof(TestFileFactory).Assembly.Location)!,
        "Resources/Rp14a/rp14A.xml");

    private const string ClosingTag = "</ns1:RP14A>";

    private const string PaddingComment =
        "\n<!-- Padding Padding Padding Padding Padding -->";

    public static string CreateValidXmlFileAboveSize(
        TestArtifacts artifacts,
        int maxSizeMb)
    {
        ArgumentNullException.ThrowIfNull(artifacts);

        return BuildPaddedXmlFile(
            artifacts,
            $"rp14a-above-{maxSizeMb}mb.xml",
            (maxSizeMb * OneMb) + 1);
    }

    public static string CreateFile(
        TestArtifacts artifacts,
        string fileName,
        string content)
    {
        ArgumentNullException.ThrowIfNull(artifacts);

        string filePath = artifacts.FilePath(fileName);

        File.WriteAllText(filePath, content, Encoding.UTF8);

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
            """,
            Encoding.UTF8);

        return filePath;
    }

    private static string BuildPaddedXmlFile(
        TestArtifacts artifacts,
        string fileName,
        int targetBytes)
    {
        string filePath = artifacts.FilePath(fileName);

        string xml = File.ReadAllText(_baselineRp14aFilePath, Encoding.UTF8);

        int closingTagStart = xml.LastIndexOf(ClosingTag, StringComparison.Ordinal);
        string xmlBody = xml[..closingTagStart];
        string xmlClose = xml[closingTagStart..];

        int bodyBytes = Encoding.UTF8.GetByteCount(xmlBody);
        int closeBytes = Encoding.UTF8.GetByteCount(xmlClose);
        int paddingBytes = Encoding.UTF8.GetByteCount(PaddingComment);
        int paddingCount = Math.Max(0, (targetBytes - bodyBytes - closeBytes + paddingBytes - 1) / paddingBytes);

        StringBuilder builder = new(bodyBytes + (paddingCount * PaddingComment.Length) + closeBytes);
        builder.Append(xmlBody);
        for (int i = 0; i < paddingCount; i++)
        {
            builder.Append(PaddingComment);
        }
        builder.Append(xmlClose);

        File.WriteAllText(filePath, builder.ToString(), Encoding.UTF8);

        return filePath;
    }
}
