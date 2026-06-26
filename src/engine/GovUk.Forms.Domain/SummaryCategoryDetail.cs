namespace GovUk.Forms.Domain;

public sealed class SummaryCategoryDetail
{
    public string Label { get; init; }
    
    public string[] Values { get; init; }

    public SummaryAction[] Actions { get; init; } = [];
}