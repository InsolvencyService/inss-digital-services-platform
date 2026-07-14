namespace GovUk.Forms.Domain.Types;

public sealed class SingleLineText : TypeBase
{
    public string Value { get; set; }
    
    public string Label { get; init; } = "Enter text";

    public string? Hint { get; init; } = "Enter your text";

    public LabelSizes LabelSize { get; init; } = LabelSizes.Small;
}