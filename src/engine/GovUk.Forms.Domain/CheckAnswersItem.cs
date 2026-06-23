namespace GovUk.Forms.Domain;

public sealed class CheckAnswersItem
{
    public required string Title { get; init; }

    public required string ChangeUrl { get; init; }

    public required string[] Values { get; init; }
}