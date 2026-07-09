namespace GovUk.Forms.Domain.Search.Formatting;

public sealed class DefaultFieldValueFormatter : FieldValueFormatter
{
    public override string Format(string? value)
    {
        return value ?? string.Empty;
    }
}