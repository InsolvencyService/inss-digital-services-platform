namespace GovUk.Forms.HostApp.UI.Test.Models;

public sealed record Rp14aTestFile(
    string FilePath,
    IReadOnlyDictionary<string, string> AppliedMutations,
    int TargetEmployeeIndex,
    DateTime CreatedAt);
