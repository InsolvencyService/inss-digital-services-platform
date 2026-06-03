using System.Diagnostics;
using System.Globalization;
using System.Security;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace GovUk.Forms.HostApp.UI.Test.Builders;

internal static class XmlFixtureHelper
{
    internal static string FormatDate(DateOnly? date) =>
        date?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? string.Empty;

    internal static XNamespace ExtractNamespace(XDocument document)
    {
        XElement root = document.Root
            ?? throw new InvalidOperationException("XML document is empty.");

        return root.Name.NamespaceName;
    }

    internal static XDocument LoadXmlDocument(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        try
        {
            return XDocument.Load(filePath, LoadOptions.PreserveWhitespace);
        }
        catch (Exception ex) when (ex is IOException or XmlException)
        {
            throw new InvalidOperationException(
                $"Failed to load XML file: {filePath}",
                ex);
        }
    }

    internal static void SaveXmlDocument(XDocument document, string filePath)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        try
        {
            using StreamWriter writer = new(
                filePath,
                append: false,
                encoding: new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

            document.Save(writer, SaveOptions.DisableFormatting);
        }
        catch (Exception ex) when (IsFileException(ex))
        {
            throw new InvalidOperationException(
                $"Failed to save XML to '{filePath}': {ex.Message}",
                ex);
        }
    }

    internal static string ResolveBaselinePath(string baselineFilePath, string label)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(baselineFilePath);

        string absolutePath = Path.GetFullPath(
            baselineFilePath,
            AppContext.BaseDirectory);

        if (!File.Exists(absolutePath))
        {
            throw new FileNotFoundException(
                $"{label} baseline XML file not found: {absolutePath}",
                absolutePath);
        }

        return absolutePath;
    }

    internal static bool IsFileException(Exception ex) =>
        ex is IOException
            or UnauthorizedAccessException
            or DirectoryNotFoundException
            or PathTooLongException
            or NotSupportedException
            or SecurityException;

    internal static void Log(string tag, string level, string message)
    {
        string timestamp = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture);

        Debug.WriteLine($"[{timestamp}] [{level}] [{tag}] {message}");
    }
}
