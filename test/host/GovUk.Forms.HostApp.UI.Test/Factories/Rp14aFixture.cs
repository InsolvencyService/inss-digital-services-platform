using System.Xml;
using System.Xml.Linq;

namespace GovUk.Forms.HostApp.UI.Test.Factories;

/// <summary>
/// Represents a created RP14A fixture with metadata about applied mutations.
/// </summary>
public record Rp14aFixture(
    string FilePath,
    IReadOnlyDictionary<string, string> AppliedMutations,
    int TargetEmployeeIndex,
    DateTime CreatedAt)
{
    public void Validate()
    {
        if (!File.Exists(FilePath))
        {
            throw new InvalidOperationException($"Fixture file does not exist: {FilePath}");
        }

        try
        {
            XDocument document = XDocument.Load(FilePath, LoadOptions.PreserveWhitespace);
            if (document.Root == null)
            {
                throw new InvalidOperationException("Fixture XML is empty");
            }
        }
        catch (XmlException ex)
        {
            throw new InvalidOperationException($"Fixture XML is not well-formed: {ex.Message}", ex);
        }
    }

    public override string ToString()
    {
        string mutations = string.Join(", ", AppliedMutations.Select(m => $"{m.Key}={m.Value}"));
        return $"RP14A Fixture [Employee={TargetEmployeeIndex}, Mutations={mutations}]";
    }
}

