namespace GovUk.Forms.Domain;

public sealed class SummaryCategory
{
    public string Label { get; init; }

    public SummaryCategoryDetail[] Details { get; init; } = [];
}