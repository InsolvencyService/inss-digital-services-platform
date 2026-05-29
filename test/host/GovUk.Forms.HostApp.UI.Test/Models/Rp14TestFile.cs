namespace GovUk.Forms.HostApp.UI.Test.Models;

public sealed record Rp14TestFile(
    string FilePath,
    IReadOnlyDictionary<string, string> AppliedMutations,
    DateTime CreatedUtc);
